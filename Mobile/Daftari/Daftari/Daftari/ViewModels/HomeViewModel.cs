using Daftari.Models;
using Daftari.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Daftari.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ObservableCollection<CalendarDate> Calendar_Dates { get; private set; }
        public List<string> Week_Codes { get; set; }
        public CalendarDate SelectedItem { get; set; }
        //Filter Date
        public DateTime StartAt { get; set; }
        //public int _Year { get; set; }
        //public int _Month { get; set; }
        public bool IsRunning { get; set; }
        public ICommand TapCommand => new Command<string>(ButtonPressed);
        public ICommand ItemChangedCommand => new Command<CalendarDate>(ItemChanged);
        public HomeViewModel()
        {
            //Calendar_Dates = new List<CalendarDate>(); 

            StartAt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var c_dates = GetCalendarDates(StartAt);
            Calendar_Dates = new ObservableCollection<CalendarDate>(c_dates);

            //var start = new DateTime(StartAt.Year, StartAt.Month, 1);
            //var end = start.AddMonths(1).AddDays(-1);

            //foreach (var date in start.To(end))
            //{
            //    // Map to event occurrences
            //    //Insert
            //    Calendar_Dates.Add(new CalendarDate
            //    {
            //        StartAt = date,
            //        EndAt = date,
            //        HasEvent = date.Day == 11,
            //        ServiceName = date.Day == 11 ? "4 - Seahorses" : ""
            //    });
            //}

            //try
            //{
            //    //Week_Codes = new List<string> { "S", "M", "T", "W", "T", "F", "S" };
            //    var culture = CultureInfo.CurrentCulture;
            //    var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

            //    var first_start_date_of_week = Calendar_Dates.Where(x => x.StartAt.DayOfWeek == firstDayOfWeek).Min(x => x.StartAt);
            //    Week_Codes = Calendar_Dates.Where(x => x.StartAt >= first_start_date_of_week).Take(7).Select(x => culture.DateTimeFormat.GetAbbreviatedDayName(x.StartAt.DayOfWeek)[0].ToString()).ToList();

            //    //Saturate
            //    var first_day_of_week = (int)start.DayOfWeek;
            //    var prev_end = start.AddDays(-first_day_of_week);

            //    Calendar_Dates.InsertRange(0, prev_end.To(start.AddDays(-1)).Select(date => new CalendarDate
            //    {
            //        StartAt = date,
            //        EndAt = date,
            //        TextColor = "#BFBFBF"
            //    }));

            //    var last_day_of_week = (int)end.DayOfWeek;
            //    var next_end = end.AddDays(6 - last_day_of_week);
            //    if ((next_end.Subtract(prev_end).Days + 1) / 7 < 6)
            //    {
            //        next_end = next_end.AddDays(7);
            //    }

            //    Calendar_Dates.AddRange(end.AddDays(1).To(next_end).Select(date => new CalendarDate
            //    {
            //        StartAt = date,
            //        EndAt = date,
            //        TextColor = "#BFBFBF"
            //    }));

            //} catch { }

            SelectedItem = Calendar_Dates[11];
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
                    Calendar_Dates.Clear();
                    GetCalendarDates(StartAt).ForEach(obj => Calendar_Dates.Add(obj));
                    OnPropertyChanged("StartAt");
                    break;
                case "Calendar_Forward":
                    StartAt = StartAt.AddMonths(1);
                    Calendar_Dates.Clear();
                    GetCalendarDates(StartAt).ForEach(obj => Calendar_Dates.Add(obj));
                    OnPropertyChanged("StartAt");
                    break;
            }
        }

        void ItemChanged(CalendarDate item)
        {
            OnPropertyChanged("SelectedItem");
            if (item != null)
            {
                //var page = Application.Current.MainPage.Navigation?.NavigationStack.Last() ?? Application.Current.MainPage;
                //var target = (IChapterClickListener)page;
                //_Context.OnChapterClick(item);

                //SelectedItem = null;
                //OnPropertyChanged("SelectedItem");
            }
        }

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
    }
}
