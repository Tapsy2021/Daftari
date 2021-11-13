using LukeApps.TrackingExtended;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.Pike13Api.Models
{
    public class TopicSubsription : IAuditDetail
    {
        public TopicSubsription()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Subsription ID")]
        public long TopicSubsriptionID { get; set; }
        [Display(Name = "Topic")]
        public string Topic { get; set; }
        public long? TopicID { get; set; }
        public string Subdomain { get; set; }
        public long? BusinessID { get; set; }
        public string Link { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }
}
