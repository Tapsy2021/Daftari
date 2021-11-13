using Daftari.SMSHandling.Enums;
using LukeApps.TrackingExtended;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.SMSHandling.Models
{
    [TrackChanges]
    internal class SMSConfig : IAuditDetail
    {
        public SMSConfig()
        {
            this.AuditDetail = new AuditDetail();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SMSConfigID { get; set; }
        public SMSAPI SMSAPI { get; set; }
        public string UserID { get; set; }
        public string password { get; set; }
        public AuditDetail AuditDetail { get; set; }

        public bool IsDeleted { get; set; }
    }
}