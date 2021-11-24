using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daftari.Pike13Api.Models
{
    public class Pike13StaffMember
    {
        public string bio { get; set; }
        public string first_name { get; set; }
        public long id { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public Pike13ProfilePhoto profile_photo { get; set; }
        public string role { get; set; }
    }
}
