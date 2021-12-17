using Daftari.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Daftari.Models
{
    public class StudentCard
    {
        public string Initial { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Student Name")]
        public string StudentName { get; set; }

        public DateTime? BirthDate { get; set; }

        public SkillLevel Level { get; set; }
    }
}
