using Daftari.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public DateTime? LocalStartAt => StartAt?.ToLocalTime();
        public DateTime? LocalEndAt => EndAt?.ToLocalTime();
        public string Name { get; set; }
        public string StaffMembers { get; set; }

        private string _levelImage;
        public string LevelImage {
            get
            {
                if (!string.IsNullOrEmpty(_levelImage))
                    return _levelImage;

                var levels = Enum.GetValues(typeof(SkillLevel)).Cast<SkillLevel>().Select(x => x.GetDisplay().ToLower()).ToList();
                
                if (levels.Contains(ServiceName?.ToLower()))
                {
                    _levelImage = $"level_{ServiceName?.Replace(" ", "").Replace("-", "_")}.png".ToLower();
                    return _levelImage;
                }
                if (Level.HasValue)
                {
                    _levelImage = $"level_{((SkillLevel)Level.Value).GetDisplay().Replace(" ", "").Replace("-", "_")}.png".ToLower();
                    return _levelImage;
                }
                return "";
            }
        }
        public int? Day => StartAt?.Day;
        public string WeekName => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(StartAt?.DayOfWeek ?? DayOfWeek.Monday);
        public string Month => StartAt?.ToString("MMM");

        public int? Level { get; set; }

        //public string AlternativeLevelImage => Level.HasValue ? $"level_{((SkillLevel)Level.Value).GetDisplay().Replace(" ", "").Replace("-", "_")}.png".ToLower() : "";
    }
}
