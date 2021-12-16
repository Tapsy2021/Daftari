using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Models
{
    public class EventOccurrence
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Service_Name { get; set; }
    }
}
