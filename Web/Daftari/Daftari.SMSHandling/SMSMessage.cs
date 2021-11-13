using Daftari.SMSHandling.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Daftari.SMSHandling
{
    public class SMSMessage
    {
        public SMSMessage()
        {
            this.Mobiles = new List<Mobile>();
        }

        public SMSAPI SMSAPI { get; set; }
        public Language Language { get; set; }

        public List<Mobile> Mobiles { get; set; }

        [Display(Name = "Mobiles CSV")]
        [DataType(DataType.MultilineText)]
        public string MobilesCSV { get; set; }

        public void setMobileNumbersFromCSV()
        {
            var mbs = MobilesCSV?.Split(',').Select(m => new Mobile(m.Trim())).ToList();
            if (mbs != null)
                Mobiles.AddRange(mbs);
        }

        [Required]
        [StringLength(480)]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public RecipientType RecipientType { get; set; } = RecipientType.Default;

        public DateTime ScheduleTime { get; set; } = DateTime.Now;

        public void AddPhone(string number)
        {
            Mobiles.Add(new Mobile(number));
        }
    }
}