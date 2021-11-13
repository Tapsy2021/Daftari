using System.ComponentModel.DataAnnotations;

namespace Daftari.Forms.Enum
{  
    public enum AccessLevel
    {
        [Display(Name = "All Employees")]
        All_Employees,

        [Display(Name = "Admin Only")]
        Admin_Only
    }

    public enum InputType
    {
        [Display(Name = "Text")]
        Text = 0,

        [Display(Name = "Number")]
        Number = 1,

        [Display(Name = "Date")]
        Date = 2,

        [Display(Name = "Select")]
        Select = 3,

        [Display(Name = "Checkbox")]
        Checkbox = 4,

        [Display(Name = "RadioButton")]
        RadioButton= 5,

        //[Display(Name = "Select - Locations")]
        //Select_Locations = 6,

        [Display(Name = "Select - Positions")]
        Select_Positions = 7,

        //[Display(Name = "Select - Pools")]
        //Select_Pools = 8,
        [Display(Name = "Select - Employees")]
        Select_Employees = 9,
        [Display(Name = "Text Area")]
        Text_Area = 10,
        [Display(Name = "Time")]
        Time = 11,
        [Display(Name = "Section")]
        Section = 12
    }

    public enum YesNo
    {
        Yes,
        No
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }
}