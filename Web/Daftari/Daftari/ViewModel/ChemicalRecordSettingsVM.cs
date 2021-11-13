using Daftari.AquaCards.Enum;
using Daftari.Chemicals.Enum;
using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.ViewModel
{
    //[Validator(typeof(ChemicalSettingsVMValidatior))]
    public class ChemicalSettingsVM
    {
        public ChemicalSettingsVM()
        {
            ChemicalCustomFields = new System.Collections.Generic.List<ChemicalCustomFieldVM>();
        }
        public Guid? ChemicalRecordSettingsID { get; set; }

        //(gal) : L
        [Display(Name = "Volume")]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public double Volume { get; set; }

        [Display(Name = "Increase Chlorine Chemical")]
        public IncreaseChlorine? IncreaseChlorine { get; set; }

        [Display(Name = "Neutralize Chlorine Chemical")]
        public NeutralizeChlorine? NeutralizeChlorine { get; set; }

        [Display(Name = "Increase Total Alkalinity Chemical")]
        public IncreaseAlkalinity? IncreaseAlkalinity { get; set; }

        [Display(Name = "Decrease Total Alkalinity Chemical")]
        public DecreaseAlkalinity? DecreaseAlkalinity { get; set; }

        [Display(Name = "Increase Calcium Hardness Chemical")]
        public IncreaseCalciumHardness? IncreaseCalciumHardness { get; set; }

        [Display(Name = "Increase Stabilizer Chemical")]
        public IncreaseStabilizer? IncreaseStabilizer { get; set; }

        [Display(Name = "Increase pH Chemical")]
        public IncreasepH? IncreasepH { get; set; }

        [Display(Name = "Decrease pH Chemical")]
        public DecreasepH? DecreasepH { get; set; }

        //Default Fields
        [Display(Name = "Free Chlorine (ppm)")]
        public Visibility FreeChlorine { get; set; }

        [Display(Name = "Free Chlorine Low Alert Setpoint (ppm)")]
        public double FreeChlorineLowAlert { get; set; }

        [Display(Name = "Free Chlorine High Alert Setpoint (ppm)")]
        public double FreeChlorineHighAlert { get; set; }

        [Display(Name = "Total Chlorine (ppm)")]
        public Visibility TotalChlorine { get; set; }

        [Display(Name = "Total Chlorine Low Alert Setpoint (ppm)")]
        public double TotalChlorineLowAlert { get; set; }

        [Display(Name = "Total Chlorine High Alert Setpoint (ppm)")]
        public double TotalChlorineHighAlert { get; set; }

        [Display(Name = "Total Bromine (ppm)")]
        public Visibility TotalBromine { get; set; }

        [Display(Name = "Total Bromine Low Alert Setpoint (ppm)")]
        public double TotalBromineLowAlert { get; set; }

        [Display(Name = "Total Bromine High Alert Setpoint (ppm)")]
        public double TotalBromineHighAlert { get; set; }

        [Display(Name = "pH")]
        public Visibility pH { get; set; }

        [Display(Name = "pH Low Alert Setpoint")]
        public double pHLowAlert { get; set; }

        [Display(Name = "pH High Alert Setpoint")]
        public double pHHighAlert { get; set; }

        //(F)
        [Display(Name = "Pool Temp")]
        public Visibility PoolTemp { get; set; }

        [Display(Name = "Ideal Pool Temperature")]
        public double IdealPoolTemp { get; set; }
        //(F)
        [Display(Name = "Pool Temp Low Alert Setpoint")]
        public double PoolTempLowAlert { get; set; }
        //(F)
        [Display(Name = "Pool Temp High Alert Setpoint")]
        public double PoolTempHighAlert { get; set; }

        //(F)
        [Display(Name = "Air Temp")]
        public Visibility AirTemp { get; set; }

        [Display(Name = "Water Clarity")]
        public Visibility WaterClarity { get; set; }


        [Display(Name = "Alkalinity")]
        public Visibility Alkalinity { get; set; }

        [Display(Name = "Alkalinity Low Alert Setpoint (ppm)")]
        public double AlkalinityLowAlert { get; set; }

        [Display(Name = "Alkalinity High Alert Setpoint (ppm)")]
        public double AlkalinityHighAlert { get; set; }

        [Display(Name = "Calcium Hardness")]
        public Visibility CalciumHardness { get; set; }
        [Display(Name = "Calcium Hardness Low Alert Setpoint (ppm)")]
        public double CalciumHardnessLowAlert { get; set; }

        [Display(Name = "Calcium Hardness High Alert Setpoint (ppm)")]
        public double CalciumHardnessHighAlert { get; set; }

        [Display(Name = "HRR/ORP")]
        public Visibility HRR_ORP { get; set; }

        [Display(Name = "Backwash")]
        public Visibility Backwash { get; set; }

        [Display(Name = "Notes")]
        public Visibility Notes { get; set; }
        public string SubDomain { get; set; }

        [Display(Name = "Unit Of Measurement")]
        public string UnitOfMeasurement { get; set; }

        [Display(Name = "Pool Daily Capacity")]
        public int PoolDailyCapacity { get; set; }
         
        public System.Collections.Generic.List<ChemicalCustomFieldVM> ChemicalCustomFields { get; set; }
    }

    //[Validator(typeof(ChemicalCustomFieldVMValidatior))]
    public class ChemicalCustomFieldVM
    {
        public long? ChemicalCustomFieldID { get; set; }
        [Display(Name = "Label")]
        public string Label { get; set; }

        [Display(Name = "Type")]
        public InputType? InputType { get; set; }
        [Display(Name = "Required")]
        public YesNo? Required { get; set; }
        [Display(Name = "Options (comma separated)")]
        public string SelectOptions { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ChemicalSettingsVMValidatior : AbstractValidator<ChemicalSettingsVM>
    {
        public ChemicalSettingsVMValidatior()
        {
            RuleFor(setting => setting.IncreaseChlorine).NotEmpty();
            RuleFor(setting => setting.IncreaseAlkalinity).NotEmpty();
            RuleFor(setting => setting.DecreaseAlkalinity).NotEmpty();
            RuleFor(setting => setting.IncreaseCalciumHardness).NotEmpty();
            RuleFor(setting => setting.IncreaseStabilizer).NotEmpty();
            RuleFor(setting => setting.NeutralizeChlorine).NotEmpty();
        }
    }

    //public class ChemicalCustomFieldVMValidatior : AbstractValidator<ChemicalCustomFieldVM>
    //{
    //    public ChemicalCustomFieldVMValidatior()
    //    {
    //        RuleFor(cf => cf.Label).NotEmpty();
    //        RuleFor(cf => cf.InputType).NotEmpty();
    //        RuleFor(cf => cf.Required).NotEmpty();
    //        RuleFor(cf => cf.Required).NotEmpty();
    //        RuleFor(cf => cf.SelectOptions).NotEmpty().When(cf => cf.InputType == InputType.Select).WithMessage("Options are required for this Input Type");
    //    }
    //}
}