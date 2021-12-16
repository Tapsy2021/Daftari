﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Daftari.Models
{
    public class CalendarDate
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string ServiceName { get; set; }

        public int Day
        {
            get { return StartAt.Day; }
        }

        public string TextColor { get; set; }

        public bool HasEvent { get; set; }

        public CalendarDate()
        {
            TextColor = "#1E3565";
        }  
        
        public string WeekName
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(StartAt.DayOfWeek); }
        }

        public string Month
        {
            get { return StartAt.ToString("MMMMM"); }
        }
    }
}
