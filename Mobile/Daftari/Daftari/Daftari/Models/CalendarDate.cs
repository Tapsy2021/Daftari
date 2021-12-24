using Daftari.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Daftari.Models
{
    public class CalendarDate : ViewModelBase
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string ServiceName { get; set; }

        public int Day
        {
            get { return StartAt.Day; }
        }

        public string TextColor { get; set; }

        private bool _hasEvent;
        public bool HasEvent 
        {
            get => _hasEvent;
            set
            {
                _hasEvent = value;
                OnPropertyChanged("HasEvent");
                OnPropertyChanged("DateColor");
            }
        }

        public string DateColor
        {
            get => HasEvent ? "#ff4a4a" : "transparent";
        }
        public List<Visit> Visits { get; set; }
        public CalendarDate()
        {
            TextColor = "#1E3565";
            Visits = new List<Visit>();
        }

        public string WeekName
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(StartAt.DayOfWeek); }
        }

        public string Month
        {
            get { return StartAt.ToString("MMMM"); }
        }

        public void OnNotify(string PropertyName)
        {
            OnPropertyChanged(PropertyName);
        }
    }
}
