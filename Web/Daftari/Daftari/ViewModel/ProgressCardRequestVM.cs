using Daftari.AquaCards.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace Daftari.ViewModel
{
    public class ProgressCardRequestVM
    {
        [Required]
        [Display(Name = "Customer (from Pike13)")]
        public string CustomerID { get; set; }

        [Display(Name = "Card Type")]
        public CardType CardType { get; set; }

        [Display(Name = "Skill Level")]
        public SkillLevel Level { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; } = DateTime.Now;

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; } = DateTime.Now;

        public string Instructors { get; set; }

        public string Plan { get; set; }
    }

    public enum CardLevel
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }

    public enum CardType
    {
        [Display(Name = "Progress Card")]
        Progress,

        [Display(Name = "Lessons for life Certificate")]
        Certificate,

        [Display(Name = "SNAP Certificate")]
        SNAP,

        [Display(Name = "Team Barracuda Certificate")]
        BarracudaTeam,

        [Display(Name = "Swim Club Certificate")]
        AquaTotsTeam
    }
}