using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Models
{
    public class StudentCardDetail
    {
        public virtual Skill Skill { get; set; }
        public bool IsComplete { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string CompletedBy { get; set; }
    }
}
