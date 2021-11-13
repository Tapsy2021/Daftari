using Daftari.SMSHandling.Enums;
using LukeApps.TrackingExtended;
using LukeApps.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.SMSHandling.Models
{
    public class SMSLog
    {
        public SMSLog()
        {
            this.AuditDetail = new AuditDetail();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SMSID { get; set; }
        public SMSAPI SMSAPI { get; set; }
        public string Message { get; set; }
        public Language Language { get; set; }

        [Display(Name = "Schedule Date and Time")]
        public DateTime ScheddateTime { get; set; }

        public string Recipients { get; set; }

        [Display(Name = "Recipient Type")]
        public RecipientType RecipientType { get; set; }

        [Display(Name = "Time Sent")]
        public DateTime TimeSent { get; set; } = DateTime.Now;

        [Display(Name = "SMS Push Result")]
        public int SMSPushResult { get; set; }

        public string SMSStatus => SMSAPI == SMSAPI.Omantel ? ((OmantelSMSPushResult)SMSPushResult).GetDisplay() : ((OoredooSMSPushResult)SMSPushResult).GetDisplay();

        public string optValue { get; set; } = "";
        public AuditDetail AuditDetail { get; set; }
    }
}