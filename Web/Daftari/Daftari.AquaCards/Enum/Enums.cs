using System.ComponentModel.DataAnnotations;

namespace Daftari.AquaCards.Enum
{
    public enum CustomerStatus
    {
        New,
        Returning
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Session
    {
        [Display(Name = "Morning")]
        Morning,

        [Display(Name = "Afternoon")]
        Afternoon,

        [Display(Name = "Evening")]
        Evening,
    }
}