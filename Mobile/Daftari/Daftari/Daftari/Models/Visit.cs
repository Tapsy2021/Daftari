using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Daftari.Models
{
    public class Visit
    {
        public long VisitID { get; set; }
        public long? EventOccurrenceID { get; set; }
        public long? PersonID { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string ServiceName { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Name { get; set; }
        public string StaffMembers { get; set;  }
        public string LevelImage => $"level_{ServiceName?.Replace(" ", "").Replace("-", "_")}.png".ToLower();
        public int? Day => StartAt?.Day;
        public string WeekName => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(StartAt?.DayOfWeek ?? DayOfWeek.Monday);
        public string Month => StartAt?.ToString("MMM");
    }
}
