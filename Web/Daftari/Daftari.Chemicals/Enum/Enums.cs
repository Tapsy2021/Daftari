using System.ComponentModel.DataAnnotations;

namespace Daftari.Chemicals.Enum
{  
    public enum IncreaseChlorine
    {
        [Display(Name = "Chlorine Gas")]
        Chlorine_Gas,
        [Display(Name = "Calcium Hypochlorite (67%)")]
        Calcium_Hypochlorite_67,
        [Display(Name = "Sodium Hypochlorite (12%)")]
        Sodium_Hypochlorite_12,
        [Display(Name = "Lihium Hypochlorite")]
        Lihium_Hypochlorite,
        [Display(Name = "Dichlor (62%)")]
        Dichlor_62,
        [Display(Name = "Dichlor (56%)")]
        Dichlor_56,
        [Display(Name = "Trichlor")]
        Trichlor
    }

    public enum IncreaseAlkalinity
    {
        [Display(Name = "Sodium Bicarbonate")]
        Sodium_Bicarbonate,
        [Display(Name = "Sodium Carbonate")]
        Sodium_Carbonate,
        [Display(Name = "Sodium Sesquicarbonate")]
        Sodium_Sesquicarbonate
    }
    public enum DecreaseAlkalinity
    {
        [Display(Name = "Muriatic Acid (31.4%)")]
        Muriatic_Acid,
        [Display(Name = "Sodium Bisulfate")]
        Sodium_Bisulfate
    }
    public enum IncreaseCalciumHardness
    {
        [Display(Name = "Calcium Chloride (100%)")]
        Calcium_Chloride_100,
        [Display(Name = "Calcium Chloride (77%)")]
        Calcium_Chloride_77
    }
    public enum IncreaseStabilizer
    {
        [Display(Name = "Cyanuric Acid")]
        Cyanuric_Acid
    }
    public enum NeutralizeChlorine
    {
        [Display(Name = "Sodium Thiosulfate Acid")]
        Sodium_Thiosulfate_Acid,
        [Display(Name = "Sodium Sulfite")]
        Sodium_Sulfite
    }
    public enum IncreasepH
    {
        [Display(Name = "Sodium Carbonate")]
        Sodium_Carbonate,
        [Display(Name = "Sodium Hydroxide (50%)")]
        Sodium_Hydroxide_50,
    }
    public enum DecreasepH
    {
        [Display(Name = "Muriatic Acid")]
        Muriatic_Acid,
        [Display(Name = "Sodium Bisulfate")]
        Sodium_Bisulfate,
        [Display(Name = "Carbon Dioxide")]
        Carbon_Dioxide
    }
    public enum Visibility
    {
        Hidden,
        Visible
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

    public enum WaterClarity
    {
        Clear,
        Cloudy
    }
    public enum YesNo
    {
        Yes,
        No
    }

    public enum ChemicalType
    {
        [Display(Name = "Sodium")]
        Sodium,
        [Display(Name = "Calcium Hypochlorite")]
        Calcium_Hypochlorite,
        [Display(Name = "Calcium Bicarbonate")]
        Calcium_Bicarbonate,
        [Display(Name = "Algaecide")]
        Algaecide,
        [Display(Name = "Not Added")]
        Not_Added
    }
    // Metric: metre (length), gram (weight) and litre (volume)
    // Imperial inch / foot / yard / mile (length), the ounce / pound / stone / hundredweight (weight / mass) and the fluid ounce / pint / quart / gallon (volume)
    /// <summary>
    /// Determines different Measure Systems
    /// </summary>
    public enum UnitOfMeasurement
    {
        /// <summary>
        /// Imperial: inch / foot / yard / mile (length), the ounce / pound / stone / hundredweight (weight / mass) and the fluid ounce / pint / quart / gallon (volume)
        /// </summary>
        Imperial,
        /// <summary>
        /// Metric: metre (length), gram (weight) and litre (volume)
        /// </summary>
        Metric
    }
}