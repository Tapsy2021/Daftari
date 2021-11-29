using LukeApps.TrackingExtended;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Daftari.Pike13Api.Models
{
    public class Visit : IAuditDetail
    {
        public Visit()
        {
            AuditDetail = new AuditDetail();
        }
        [JsonProperty("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long VisitID { get; set; }

        [JsonProperty("person_id")]
        public long? PersonID { get; set; }

        [JsonProperty("event_occurrence_id")]
        public long? EventOccurrenceID { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("registered_at")]
        public DateTime? RegisteredAt { get; set; }

        [JsonProperty("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [JsonProperty("noshow_at")]
        public DateTime? NoshowAt { get; set; }

        [JsonProperty("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("paid")]
        public bool? Paid { get; set; }

        [JsonProperty("paid_for_by")]
        public string PaidForBy { get; set; }

        [JsonProperty("punch_id")]
        public long? PunchID { get; set; }

        [JsonProperty("only_staff_can_cancel")]
        public bool? OnlyStaffCanCancel { get; set; }
        /// <summary>
        /// Local variable, used to mark unpaid in advance
        /// </summary>
        public bool? Unpaid { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
        public virtual EventOccurrance EventOccurrance { get; set; }
        //[NotMapped]
        //[JsonProperty("person")]
        //public Person Person { get; set; }
        [NotMapped]
        public DateTime? LastModified => (new System.Collections.Generic.List<DateTime?>
        {
            RegisteredAt, CompletedAt, NoshowAt, CancelledAt, CreatedAt, UpdatedAt
        }).Max();
    }
}
