namespace LukeApps.BugsTracker.Models
{
    using LukeApps.TrackingExtended;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Bug : IAuditDetail
    {
        public Bug()
        {
            this.AuditDetail = new AuditDetail();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Source { get; set; }
        public string FilePath { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public string Category { get; set; }

        public string ReportedBy { get; set; }

        public int? ApplicationID { get; set; }

        [StringLength(50)]
        public string Priorty { get; set; }

        public string Comments { get; set; }

        [StringLength(50)]
        public string AssignedTo { get; set; }

        [Column(TypeName = "ntext")]
        public string StackTrace { get; set; }

        public int ErrorCode { get; set; }

        public string Url { get; set; }

        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }
}