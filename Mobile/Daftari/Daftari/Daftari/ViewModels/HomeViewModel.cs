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
        public Customer SelectedDependant { get; set; }

        public ObservableCollection<CalendarDate> Calendar_Dates { get; private set; }
        private List<CalendarDate> _calendar_dates { get; set; }
        private List<Visit> _visits { get; set; } = new List<Visit>();
        public List<Visit> Visits
        {
            get => _visits;
            set
            {
                _visits = value;
                OnPropertyChanged("Visits");
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

        public bool IsRunning { get; set; }
        public ICommand TapCommand => new Command<string>(ButtonPressed);
        public ICommand DateChangedCommand => new Command<CalendarDate>((item) =>
        {
            OnPropertyChanged("SelectedDate");
        });
        public ICommand DependantChangedCommand => new Command<Customer>((item) =>
        {
            OnPropertyChanged("SelectedDependant");
            //open right tab (requires listener in ui)
            _Context.OpenSchedule();
            //FetchVisits();
            SimulateVisits();

        });
        public ICommand LoadDependantsCommand { get; private set; }
        public HomeViewModel(IHomeBindingContextListener Context)
        {
            LoadDependantsCommand = new Command(async () => await FetchDependants());
            _Context = Context;
            //_visit_ct = new CancellationTokenSource();
            //_dependants = GetDependants();
            //DbHelper.Instance.SaveDependants(_dependants);
            //Calendar_Dates = new List<CalendarDate>(); 
            _dependants = DbHelper.Instance.GetDependants().Result.OrderBy(x => x.CustomerID).ToList();
            Dependants = new ObservableCollection<Customer>(_dependants);

            StartAt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _calendar_dates = GetCalendarDates(StartAt);
            Calendar_Dates = new ObservableCollection<CalendarDate>(_calendar_dates);

            SelectedDate = _calendar_dates.FirstOrDefault(x => x.StartAt == DateTime.Today);
        }

        public async Task FetchDependants()
        {
            //IF NO DEPENDANTS YET THEN USE LOADING / GIF
            //if (IsRunning)
            //{
            //    return;
            //}
            //IsRunning = true;
            //OnPropertyChanged("IsRunning");

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
            //IsRunning = false;
            //OnPropertyChanged("IsRunning");
        }

        public void FetchVisits()
        {
            if (SelectedDependant == null)
                return;
            if (!_visit_ct.IsCancellationRequested)
                _visit_ct.Cancel();
            //IF NO DEPENDANTS YET THEN USE LOADING / GIF
            //if (IsRunning)
            //{
            //    return;
            //}
            //IsRunning = true;
            //OnPropertyChanged("IsRunning");
            try
            {
                var startAt = StartAt;
                var _visits_data = Pike13AccessHelper.GetVisitsAsync(new { StartAt.Year, StartAt.Month, SelectedDependant.PersonID }, _visit_ct).Result?.OrderBy(x => x.StartAt).ToList();

                if (startAt == StartAt && (_visits_data?.Any() ?? false))
                {
                    Visits = _visits_data.OrderBy(x => x.StartAt).ToList();
                    var visits = _visits_data.GroupBy(x => x.StartAt.Value.Date).ToList();

                    foreach (var obj in Calendar_Dates)
                    {
                        var date_visit = visits.Where(x => x.Key == obj.StartAt.Date).SelectMany(x => x.ToList()).ToList();
                        if (date_visit.Any())
                        {
                            obj.Visits = date_visit;
                            if (obj.StartAt.Date == SelectedDate?.StartAt.Date)
                            {
                                obj.OnNotify("Visits");
                            }
                            obj.HasEvent = true;
                        }
                    }
                }

                if (_visits_data == null)
                {
                    DependencyService.Get<IMessage>().LongAlert("Failed to fetch visits..");
                }
            }
            catch { }
            //IsRunning = false;
            //OnPropertyChanged("IsRunning");
        }

        void SimulateVisits()
        {
            var _visits_data = GetVisits();
            Visits = _visits_data.OrderBy(x => x.StartAt).ToList();
            var visits = _visits_data.GroupBy(x => x.StartAt.Value.Date).ToList();
            foreach (var obj in Calendar_Dates)
            {
                var date_visit = visits.Where(x => x.Key == obj.StartAt.Date).SelectMany(x => x.ToList()).ToList();
                if (date_visit.Any())
                {
                    obj.Visits = date_visit;
                    if (obj.StartAt.Date == SelectedDate?.StartAt.Date)
                    {
                        obj.OnNotify("Visits");
                    }
                    obj.HasEvent = true;
                }
            }
        }

        private List<CalendarDate> GetCalendarDates(DateTime startDate)
        {
            var start = new DateTime(startDate.Year, startDate.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            var calendar_dates = new List<CalendarDate>();

            foreach (var date in start.To(end))
            {
                // Map to event occurrences
                //Insert
                calendar_dates.Add(new CalendarDate
                {
                    StartAt = date,
                    EndAt = date
                    //HasEvent = date.Day == 11,
                    //ServiceName = date.Day == 11 ? "4 - Seahorses" : ""
                });
            }

            try
            {
                //Week_Codes = new List<string> { "S", "M", "T", "W", "T", "F", "S" };
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
        //public int Year
        //{
        //    get { return _Year; }
        //    set { _Year = value; OnPropertyChanged("Year"); }
        //}

        //public string Year
        //{
        //    get { return _Year.ToString(); }
        //}

        //public int Month
        //{
        //    get { return _Month; }
        //    set { _Year = value; OnPropertyChanged("Month"); }
        //}

        //public string Month
        //{
        //    get { return new DateTime(_Year, _Month, 1).ToString("MMMM"); }
        //}

        void ButtonPressed(string btnId)
        {
            switch (btnId)
            {
                case "Calendar_Back":
                    StartAt = StartAt.AddMonths(-1);
                    //Calendar_Dates.Clear();
                    var prev_month = GetCalendarDates(StartAt);
                    Calendar_Dates.Clear();
                    prev_month.ForEach(obj => Calendar_Dates.Add(obj));
                    SimulateVisits();
                    //FetchVisits();
                    //do sync
                    //var visits = GetVisits().GroupBy(x => x.StartAt.Value.Date).ToList();
                    //month_data.ForEach(obj =>
                    //{
                    //    var date_visit = visits.Where(x => x.Key == obj.StartAt.Date).SelectMany(x => x.ToList()).ToList();
                    //    if (date_visit.Any())
                    //    {
                    //        obj.Visits = date_visit;
                    //        if (obj.StartAt.Date == SelectedDate?.StartAt.Date)
                    //        {
                    //            obj.OnNotify("Visits");
                    //        }
                    //        obj.HasEvent = true;
                    //    }
                    //});

                    //OnPropertyChanged("StartAt");
                    break;
                case "Calendar_Forward":
                    StartAt = StartAt.AddMonths(1);                    
                    var next_month = GetCalendarDates(StartAt);
                    Calendar_Dates.Clear();
                    next_month.ForEach(obj => Calendar_Dates.Add(obj));
                    SimulateVisits();
                    //FetchVisits();
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

        //void ItemChanged(CalendarDate item)
        //{
        //    OnPropertyChanged("SelectedDate");
        //    if (item != null)
        //    {
        //        //var page = Application.Current.MainPage.Navigation?.NavigationStack.Last() ?? Application.Current.MainPage;
        //        //var target = (IChapterClickListener)page;
        //        //_Context.OnChapterClick(item);

        //        //SelectedDate = null;
        //        //OnPropertyChanged("SelectedDate");
        //    }
        //}

        //public List<Calendar_Date> GetCalendar_Dates(int Start, int Max) => (new List<Calendar_Date>
        //{
        //    new Calendar_Date
        //    {
        //        Date = DateTime.Today
        //    },
        //    new Calendar_Date
        //    {
        //        Date = DateTime.Today.AddDays(1)
        //    }
        //});

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
