using System.ComponentModel.DataAnnotations;

namespace Daftari.AquaCards.Enum
{
    public enum SkillLevel
    {
        [Display(Name = "1 - Tadpoles", Description = "Water Adjustment")]
        One,

        [Display(Name = "2 - Minnows", Description = "Boundaries")]
        Two,

        [Display(Name = "3 - Leapfrog", Description = "Comfort")]
        Three,

        [Display(Name = "4 - Seahorses", Description = "Control")]
        Four,

        [Display(Name = "5 - Starfish", Description = "Independence")]
        Five,

        [Display(Name = "6 - Seals", Description = "Tempo")]
        Six,

        [Display(Name = "7 - Sharks", Description = "Technique")]
        Seven,

        [Display(Name = "8 - Stingrays", Description = "Proficiency")]
        Eight
    }
}