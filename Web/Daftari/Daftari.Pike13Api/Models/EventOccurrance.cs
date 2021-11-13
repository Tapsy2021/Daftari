using LukeApps.TrackingExtended;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Daftari.Pike13Api.Models
{
    public class EventOccurrance : IAuditDetail
    {
        public EventOccurrance()
        {
            AuditDetail = new AuditDetail();
            Visits = new HashSet<Visit>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("id")]
        public long EventOccurrenceID { get; set; }

        [JsonProperty("event_id")]
        public long EventID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [JsonProperty("description")]
        public string Description { get; set; }
        public long? ServiceID { get; set; }
        public long? LocationID { get; set; }

        [Display(Name = "Session Start")]
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Timezone { get; set; }

        [Display(Name = "Attendance Complete")]
        public bool? AttendanceComplete { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Full")]
        public bool? Full { get; set; }

        [Display(Name = "Capacity Remaining")]
        public int? CapacityRemaining { get; set; }

        [Display(Name = "Staff Members")]
        public string StaffMembers { get; set; }

        [Display(Name = "People")]
        public string people { get; set; }

        [Display(Name = "Visits")]
        public ICollection<Visit> Visits { get; set; }
        public string SubDomain { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Visits Count")]
        [JsonProperty("visits_count")]
        public int? VisitsCount { get; set; }

        public override bool Equals(object obj)
        {
            return ((EventOccurrance)obj).EventOccurrenceID == EventOccurrenceID &&
                ((EventOccurrance)obj).EventID == EventID &&
                ((EventOccurrance)obj).Name == Name &&
                ((EventOccurrance)obj).Description == Description &&
                ((EventOccurrance)obj).ServiceID == ServiceID &&
                ((EventOccurrance)obj).LocationID == LocationID &&
                ((EventOccurrance)obj).StartAt == StartAt &&
                ((EventOccurrance)obj).EndAt == EndAt &&
                ((EventOccurrance)obj).Timezone == Timezone &&
                ((EventOccurrance)obj).AttendanceComplete == AttendanceComplete &&
                ((EventOccurrance)obj).State == State &&
                ((EventOccurrance)obj).Full == Full &&
                ((EventOccurrance)obj).CapacityRemaining == CapacityRemaining &&
                ((EventOccurrance)obj).StaffMembers == StaffMembers &&
                ((EventOccurrance)obj).people == people &&
                ((EventOccurrance)obj).VisitsCount == VisitsCount;
        }
    
        public bool EqualsRaw(Pike13Event obj)
        {
            return obj.id == EventOccurrenceID &&
                    obj.event_id == EventID &&
                    obj.name == Name &&
                    obj.description == Description &&
                    obj.service_id == ServiceID &&
                    obj.location_id == LocationID &&
                    obj.start_at == StartAt &&
                    obj.end_at == EndAt &&
                    obj.timezone == Timezone &&
                    obj.attendance_complete == AttendanceComplete &&
                    obj.state == State &&
                    obj.full == Full &&
                    obj.capacity_remaining == CapacityRemaining &&
                    string.Join(",", obj.staff_members.Select(x => x.StaffID).OrderBy(x => x)) == StaffMembers &&
                    string.Join(",", obj.people.Select(x => x.id).OrderBy(x => x)) == people &&
                    obj.visits_count == VisitsCount;
        }
    }
}