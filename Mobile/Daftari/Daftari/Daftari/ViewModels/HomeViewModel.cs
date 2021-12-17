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
        public List<CalendarDate> Calendar_Dates { get; private set; }
        public List<string> Week_Codes { get; set; }
        public CalendarDate SelectedItem { get; set; }
        public int _Year { get; set; }
        public int _Month { get; set; }
        public bool IsRunning { get; set; }
        public ICommand TapCommand => new Command<string>(ButtonPressed);
        public ICommand ItemChangedCommand => new Command<CalendarDate>(ItemChanged);
        public HomeViewModel()
        {
            Calendar_Dates = new List<CalendarDate>();

            _Year = DateTime.Today.Year;
            _Month = DateTime.Today.Month;

            var start = new DateTime(_Year, _Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            //Month = start.ToString("MMMM");

            foreach (var date in start.To(end))
            {
                // Map to event occurrences
                //Insert
                Calendar_Dates.Add(new CalendarDate
                {
                    StartAt = date,
                    EndAt = date,
                    HasEvent = date.Day == 11,
                    ServiceName = date.Day == 11 ? "4 - Seahorses" : ""
                });
            }

            try
            {
                //Week_Codes = new List<string> { "S", "M", "T", "W", "T", "F", "S" };
                var culture = CultureInfo.CurrentCulture;
                var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
             
                var first_start_date_of_week = Calendar_Dates.Where(x => x.StartAt.DayOfWeek == firstDayOfWeek).Min(x => x.StartAt);
                Week_Codes = Calendar_Dates.Where(x => x.StartAt >= first_start_date_of_week).Take(7).Select(x => culture.DateTimeFormat.GetAbbreviatedDayName(x.StartAt.DayOfWeek)[0].ToString()).ToList();
          
                //Saturate
                var first_day_of_week = (int)start.DayOfWeek;
                var prev_end = start.AddDays(-first_day_of_week);

                Calendar_Dates.InsertRange(0, prev_end.To(start.AddDays(-1)).Select(date => new CalendarDate
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

                Calendar_Dates.AddRange(end.AddDays(1).To(next_end).Select(date => new CalendarDate
                {
                    StartAt = date,
                    EndAt = date,
                    TextColor = "#BFBFBF"
                }));

            } catch { }

            SelectedItem = Calendar_Dates[11];
        }

        public string Year
        {
            get { return _Year.ToString(); }
        }

        public string Month
        {
            get { return new DateTime(_Year, _Month, 1).ToString("MMMM"); }
        }

        async void ButtonPressed(string btnId)
        {

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
