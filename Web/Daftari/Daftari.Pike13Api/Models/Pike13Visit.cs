using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Daftari.Pike13Api.Models
{
    public class Pike13Visit
    {
        public long id { get; set; }

        public long? person_id { get; set; }

        public long? event_occurrence_id { get; set; }

        public string state { get; set; }

        public string status { get; set; }

        public DateTime? registered_at { get; set; }

        public DateTime? completed_at { get; set; }

        public DateTime? noshow_at { get; set; }

        public DateTime? cancelled_at { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public bool? paid { get; set; }

        public string paid_for_by { get; set; }

        public long? punch_id { get; set; }

        public bool? only_staff_can_cancel { get; set; }

        public Pike13Event event_occurrence { get; set; }
        //[NotMapped]
        //[JsonProperty("person")]
        //public Person Person { get; set; }
        public DateTime? LastModified => (new System.Collections.Generic.List<DateTime?>
        {
            registered_at, completed_at, noshow_at, cancelled_at, created_at, updated_at
        }).Max();
    }
}
