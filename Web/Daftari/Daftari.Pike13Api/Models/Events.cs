using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Daftari.Pike13Api.Models
{
    public class Pike13Event
    {
        public long id { get; set; }

        public long event_id { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        public long service_id { get; set; }

        public long location_id { get; set; }

        [Display(Name = "Session Start")]
        public DateTime start_at { get; set; }

        public DateTime end_at { get; set; }

        public string timezone { get; set; }

        [Display(Name = "Attendance Complete")]
        public bool attendance_complete { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Full")]
        public bool full { get; set; }

        [Display(Name = "Visits Count")]
        public int visits_count { get; set; }

        [Display(Name = "Capacity Remaining")]
        public int capacity_remaining { get; set; }

        [Display(Name = "Staff Members")] 
        public List<Staff> staff_members { get; set; }

        [Display(Name = "People")]
        public List<Pike13Person> people { get; set; }
        public List<Pike13Visit> visits { get; set; }
        public Pike13Event()
        {
            people = new List<Pike13Person>();
            visits = new List<Pike13Visit>();
        }
    }
}