using System.ComponentModel.DataAnnotations;

namespace Daftari.Utils
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

    public enum SkillDifficulty
    {
        [Display(Name = "BEGINNER SKILLS")]
        BEGINNER,

        [Display(Name = "INTERMEDIATE SKILLS")]
        INTERMEDIATE,

        [Display(Name = "ADVANCED SKILLS")]
        ADVANCED
    }
}
