using Daftari.Models;
using Daftari.Services.Database;
using Daftari.Services.Depedencies;
using Daftari.Services.REST.Helpers;
using Daftari.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Daftari.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private IHomeBindingContextListener _Context;
        public ObservableCollection<Customer> Dependants { get; set; }
        private List<Customer> _dependants { get; set; }
        public Customer _selectedDependant { get; set; }
        public Customer SelectedDependant 
        {
            get => _selectedDependant;
            set
            {
                _selectedDependant = value;
                OnPropertyChanged("SelectedDependant");
                OnPropertyChanged("IsDependantSelected");
            }
        }

        public bool IsDependantSelected
        {
            get => _selectedDependant != null;
        }

        public ObservableCollection<CalendarDate> Calendar_Dates { get; private set; }
        private List<CalendarDate> _calendar_dates { get; set; }
        
        private List<Visit> _visits { get; set; } = new List<Visit>();
        public ObservableCollection<Visit> Visits { get; private set; } = new ObservableCollection<Visit>();

        private Visit PrevVisit { get; set; }

        //public Visit SelectedVisit { get; set; }
        public Visit _selectedVisit { get; set; }
        public Visit SelectedVisit
        {
            get => _selectedVisit;
            set
            {
                _selectedVisit = value;
                OnPropertyChanged("SelectedVisit");
            }
        }

        public List<string> Week_Codes { get; set; }
        public CalendarDate SelectedDate { get; set; }
        private CancellationTokenSource _visit_ct { get; set; } = new CancellationTokenSource();
        //Filter Date
        private DateTime _startAt { get; set; }
        public DateTime StartAt 
        {
            get => _startAt;
            set
            {
                _startAt = value;
                OnPropertyChanged("StartAt");
            }
        }

        private bool _isRunning { get; set; }
        public bool IsRunning 
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        private bool _calendarRunning { get; set; }
        public bool CalendarRunning
        {
            get => _calendarRunning;
            set
            {
                _calendarRunning = value;
                OnPropertyChanged("CalendarRunning");
            }
        }
        
        public ICommand TapCommand => new Command<string>(ButtonPressed);
        public ICommand DateChangedCommand => new Command<CalendarDate>((item) =>
        {
            OnPropertyChanged("SelectedDate");
        });
        public ICommand DependantChangedCommand => new Command<Customer>((item) =>
        {
            //open right tab (requires listener in ui)
            _Context.OpenSchedule();
            LoadScheduleCommand.Execute(null);
        });

        public ICommand VisitChangedCommand => new Command<Visit>((item) => 
        {
            if (SelectedVisit != null)
            {
                if (PrevVisit == SelectedVisit)
                {
                    SelectedVisit.IsVisible = !SelectedVisit.IsVisible;
                }
                else
                {
                    if (PrevVisit != null)
                    {
                        PrevVisit.IsVisible = false;
                    }
                    SelectedVisit.IsVisible = true;
                }
                PrevVisit = SelectedVisit;
                SelectedVisit = null;
            }            
        });

        public ICommand OnCancelCommand => new Command<Visit>(async (item) =>
        {
            if (item != null)
            {
                var message = $"Cancel {SelectedDependant?.FirstName}'s Class for {item.LocalStartAt?.ToString("dd MMMM")} at {item.LocalStartAt?.ToString("h:mm tt")}?";
                bool answer = await Application.Current.MainPage.DisplayAlert("Cancel Session", message, "Yes", "No");
                if (answer)
                {
                    await CancelSession(item);
                }
            }
        });

        public ICommand LoadDependantsCommand { get; private set; }
        public ICommand LoadScheduleCommand { get; private set; }
        public HomeViewModel(IHomeBindingContextListener Context)
        {
            LoadDependantsCommand = new Command(async () => await FetchDependants());
            LoadScheduleCommand = new Command(async () => await FetchVisits());
            _Context = Context;

            _dependants = DbHelper.Instance.GetDependants().Result.OrderBy(x => x.CustomerID).ToList();
            Dependants = new ObservableCollection<Customer>(_dependants);

            StartAt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _calendar_dates = GetCalendarDates(StartAt);
            Calendar_Dates = new ObservableCollection<CalendarDate>(_calendar_dates);

            SelectedDate = _calendar_dates.FirstOrDefault(x => x.StartAt == DateTime.Today);
        }

        public async Task FetchDependants()
        {
            _dependants = await CustomerHelper.GetDependantsAsync(new CancellationTokenSource());//?.OrderBy(x => x.CustomerID).ToList();

            var a1 = _dependants?.Select(x => x.CustomerID).Distinct().OrderBy(x => x).ToList() ?? new List<Guid>();
            var a2 = Dependants?.Select(x => x.CustomerID).Distinct().OrderBy(x => x).ToList() ?? new List<Guid>();

            if (!a1.SequenceEqual(a2) && _dependants != null && _dependants.Any())
            {
                DbHelper.Instance.SaveDependants(_dependants);
                if (Dependants.Any())
                    Dependants.Clear();

                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        _dependants = _dependants.OrderBy(x => x.CustomerID).ToList();
                        _dependants.ForEach(obj =>
                        {
                            Dependants.Add(obj);
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }

            if ((_dependants == null || !_dependants.Any()) && !Dependants.Any())
            {
                DependencyService.Get<IMessage>().LongAlert("Failed to fetch dependants..");
            }
        }

        public async Task FetchVisits()
        {
            if (SelectedDependant == null)
                return;
            if (!_visit_ct.IsCancellationRequested)
                _visit_ct.Cancel();
            _visit_ct = new CancellationTokenSource();
            CalendarRunning = true;

            try
            {
                var startAt = StartAt;
                var _visits_data = await Pike13AccessHelper.GetVisitsAsync(new { StartAt.Year, StartAt.Month, SelectedDependant.PersonID }, _visit_ct);//.Result?.OrderBy(x => x.StartAt).ToList();
                CalendarRunning = false;
                if (startAt == StartAt && (_visits_data?.Any() ?? false))
                {
                    Visits.Clear();
                    foreach (var obj in _visits_data.OrderBy(x => x.StartAt))
                    {
                        Visits.Add(obj);
                    }
                    var visits = _visits_data.GroupBy(x => x.StartAt.Value.Date).ToList();

                    foreach (var obj in Calendar_Dates)
                    {
                        var date_visit = visits.Where(x => x.Key == obj.StartAt.Date).SelectMany(x => x.ToList()).ToList();
                        if (date_visit.Any())
                        {
                            obj.Visits = date_visit;
                            obj.HasEvent = true;                            
                        }
                    }
                }

                if (_visits_data == null)
                {
                    DependencyService.Get<IMessage>().LongAlert("Failed to fetch visits..");
                }
            }
            catch 
            {
                CalendarRunning = false;
            }
        }

        public async Task CancelSession(Visit item)
        {
            try
            {
                IsRunning = true;
                var response = await Pike13AccessHelper.CancelVisit(new { item.VisitID }, new CancellationTokenSource());
                IsRunning = false;

                if (response.IsSuccess)
                {
                    Visits.Remove(item);
                    OnPropertyChanged("Visits");
                    foreach (var obj in Calendar_Dates)
                    {
                        if ((obj.Visits?.Any() ?? false) && obj.Visits.Contains(item))
                        {
                            obj.Visits.Remove(item);
                            if (!obj.Visits.Any())
                            {
                                obj.HasEvent = false;
                            }
                            break;
                        }
                    }
                }

                DependencyService.Get<IMessage>().LongAlert(response.Message);

            } catch (Exception ex)
            {

            }
            //check in invest (Dero)
        }

        private List<CalendarDate> GetCalendarDates(DateTime startDate)
        {
            var start = new DateTime(startDate.Year, startDate.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var calendar_dates = new List<CalendarDate>();

            foreach (var date in start.To(end))
            {
                calendar_dates.Add(new CalendarDate
                {
                    StartAt = date,
                    EndAt = date
                });
            }

            try
            {
                var culture = CultureInfo.CurrentCulture;
                var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

                var first_start_date_of_week = calendar_dates.Where(x => x.StartAt.DayOfWeek == firstDayOfWeek).Min(x => x.StartAt);
                Week_Codes = calendar_dates.Where(x => x.StartAt >= first_start_date_of_week).Take(7).Select(x => culture.DateTimeFormat.GetAbbreviatedDayName(x.StartAt.DayOfWeek)[0].ToString()).ToList();

                //Saturate
                var first_day_of_week = (int)start.DayOfWeek;
                var prev_end = start.AddDays(-first_day_of_week);

                calendar_dates.InsertRange(0, prev_end.To(start.AddDays(-1)).Select(date => new CalendarDate
                {
                    StartAt = date,
                    EndAt = date,
                    TextColor = "#BFBFBF"
                }));

                var last_day_of_week = (int)end.DayOfWeek;
                var next_end = end.AddDays(6 - last_day_of_week);
                if ((next_end.Subtract(prev_end).Days + 1) / 7 < 6)
                {
                    next_end = next_end.AddDays(7);
                }

                calendar_dates.AddRange(end.AddDays(1).To(next_end).Select(date => new CalendarDate
                {
                    StartAt = date,
                    EndAt = date,
                    TextColor = "#BFBFBF"
                }));

            }
            catch { }

            return calendar_dates;
        }

        private List<Customer> GetDependants() => new List<Customer>
        {
            new Customer
            {
                CustomerID = Guid.Parse("f71ff2ad-8911-eb11-a942-976f80566d55"),
                PersonID = 4234126,
                FirstName = "Safanah",
                LastName = "Al Habsi",
                PhotoMD = "https://d1nqv8xdwxria6.cloudfront.net/uploads/profile_photo/image/6a39b8cc-caa0-4516-9725-6d9d29e2f495/image_profile_x200.jpg"
            },
            new Customer
            {
                CustomerID = Guid.Parse("f81ff2ad-8911-eb11-a942-976f80566d55"),
                PersonID = 4234129,
                FirstName = "Samayah",
                LastName = "Al Habsi",
                PhotoMD = "https://d1nqv8xdwxria6.cloudfront.net/uploads/profile_photo/image/67f74ec6-d876-43ed-8f54-861d5d6ef654/image_profile_x200.jpg"
            },
            new Customer
            {
                CustomerID = Guid.Parse("1b82bf67-b930-eb11-a942-976f80566d55"),
                PersonID = 6385202,
                FirstName = "Saja",
                LastName = "Al Habsi",
                PhotoMD = "https://d1nqv8xdwxria6.cloudfront.net/uploads/profile_photo/image/bc52be14-7e9c-4137-aac0-cd41dc84461c/image_profile_x200.jpg"
            }
        }.ToList();

        void ButtonPressed(string btnId)
        {
            switch (btnId)
            {
                case "Calendar_Back":
                    StartAt = StartAt.AddMonths(-1);
                    var prev_month = GetCalendarDates(StartAt);
                    Calendar_Dates.Clear();
                    prev_month.ForEach(obj => Calendar_Dates.Add(obj));
                    LoadScheduleCommand.Execute(null);
                    break;
                case "Calendar_Forward":
                    StartAt = StartAt.AddMonths(1);                    
                    var next_month = GetCalendarDates(StartAt);
                    Calendar_Dates.Clear();
                    next_month.ForEach(obj => Calendar_Dates.Add(obj));
                    LoadScheduleCommand.Execute(null);
                    break;
            }
        }

        private List<Visit> GetVisits() => new List<Visit>
        {
            new Visit
            {
                ServiceName = "5 - Starfish",
                StaffMembers = "Rufaro Jena",
                StartAt = new DateTime(2021, 12, 2).AddHours(4),
                EndAt = new DateTime(2021, 12, 11).AddHours(4).AddMinutes(30)
            },
            new Visit
            {
                ServiceName = "5 - Starfish",
                StaffMembers = "Rufaro Jena",
                StartAt = new DateTime(2021, 12, 7).AddHours(6),
                EndAt = new DateTime(2021, 12, 11).AddHours(6).AddMinutes(30)
            },
            new Visit
            {
                ServiceName = "5 - Starfish",
                StaffMembers = "Rufaro Jena",
                StartAt = new DateTime(2021, 12, 15).AddHours(8),
                EndAt = new DateTime(2021, 12, 11).AddHours(8).AddMinutes(30)
            },
            new Visit
            {
                ServiceName = "5 - Starfish",
                StaffMembers = "Rufaro Jena",
                StartAt = new DateTime(2021, 12, 28).AddHours(14),
                EndAt = new DateTime(2021, 12, 11).AddHours(14).AddMinutes(30)
            }
        };

        public List<CalendarDate> GetCalendar_Dates(int Start, int Max) => (new List<CalendarDate>
        {
            new CalendarDate
            {
                StartAt = DateTime.Today
            },
            new CalendarDate
            {
                StartAt = DateTime.Today.AddDays(1)
            }
        });

        public interface IHomeBindingContextListener
        {
            void OpenSchedule();
        }
    }
}
