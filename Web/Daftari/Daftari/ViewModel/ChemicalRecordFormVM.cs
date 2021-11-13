using Daftari.AquaCards.Enum;
using Daftari.Chemicals.Enum;
using Daftari.Chemicals.Models;
using FluentValidation;
using LukeApps.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Daftari.ViewModel
{
    //[Validator(typeof(ChemicalRecordFormVMValidatior))]
    public class ChemicalRecordFormVM
    {
        public ChemicalRecordFormVM()
        {
            ChemicalCustomValues = new List<ChemicalCustomValueVM>();
            ChemicalRecordComments = new List<ChemicalRecordCommentVM>();
        }

        public long? ChemicalRecordID { get; set; }
        //public Guid? ChemicalRecordSettingsID { get; set; }
        [Display(Name = "Free Chlorine (ppm)")]
        public double? FreeChlorine { get; set; }

        [Display(Name = "Total Chlorine (ppm)")]
        public double? TotalChlorine { get; set; }

        [Display(Name = "Total Bromine (ppm)")]
        public double? TotalBromine { get; set; }

        [Display(Name = "pH")]
        public double? pH { get; set; }
        //(C)
        [Display(Name = "Pool Temp")]
        public double? PoolTemp { get; set; }
        //(C)
        [Display(Name = "Air Temp")]
        public double? AirTemp { get; set; }
        [Display(Name = "Water Clarity")]
        public WaterClarity? WaterClarity { get; set; }
        [Display(Name = "Alkalinity")]
        public double? Alkalinity { get; set; }
        [Display(Name = "Calcium Hardness")]
        public double? CalciumHardness { get; set; }
        [Display(Name = "Backwash")]
        public YesNo? Backwash { get; set; }
        [Display(Name = "HRR/ORP")]
        public double? HRR_ORP { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Employee *")]
        public string Employee { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date *")]
        public DateTime? Date { get; set; }
        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }

        public List<ChemicalCustomValueVM> ChemicalCustomValues { get; set; }
        public ChemicalSettings ChemicalSettings { get; set; }
        public List<ChemicalRecordCommentVM> ChemicalRecordComments { get; set; }

        public string TempUnits { get; set; }
        public string VolumeUnits { get; set; }
        public double PoolTempMax { get; set; }
        public double AirTempMax { get; set; }
        /// <summary>
        /// Gets an object containing a htmlAttributes collection for any Razor HTML helper component,
        /// supporting a static set (anonymous object) and/or a dynamic set (Dictionary)
        /// </summary>
        /// <param name="fixedHtmlAttributes">A fixed set of htmlAttributes (anonymous object)</param>
        /// <param name="dynamicHtmlAttributes">A dynamic set of htmlAttributes (Dictionary)</param>
        /// <returns>A collection of htmlAttributes including a merge of the given set(s)</returns>
        public object GetHtmlAttributes(
            object fixedHtmlAttributes = null,
            IDictionary<string, object> dynamicHtmlAttributes = null
            )
        {
            var rvd = (fixedHtmlAttributes == null)
                ? new System.Web.Routing.RouteValueDictionary()
                : System.Web.Mvc.HtmlHelper.AnonymousObjectToHtmlAttributes(fixedHtmlAttributes);
            if (dynamicHtmlAttributes != null)
            {
                foreach (KeyValuePair<string, object> kvp in dynamicHtmlAttributes)
                    rvd[kvp.Key] = kvp.Value;
            }
            return Chemicals.Helpers.AnonymousType.FromDictonaryToAnonymousObject(rvd);
        }
    
        public string CalculateBalance(double volume, double constVal, double idealVal, double chemPPM)
        {
            if (ChemicalSettings.UnitOfMeasurement == UnitOfMeasurement.Metric.ToString())
            {
                volume *= 0.264172;
            }
            var galConst = 10000;
            var val = volume * constVal * idealVal;
            val /= (chemPPM * galConst);
            return Math.Abs(val).ToString("N2");
        }
        //Error messages
        public string FreeChlorineError
        {
            get
            {
                var constant = 0d;
                var units = "";
                var chemPPH = 1;
                var ideal = ((ChemicalSettings.FreeChlorineHighAlert + ChemicalSettings.FreeChlorineLowAlert) / 2) - FreeChlorine;
                var correction = FreeChlorine - ideal;
                if (FreeChlorine < ChemicalSettings.FreeChlorineLowAlert)
                {
                    units = "oz.";
                    //Increase Free Chlorine constant
                    switch (ChemicalSettings.IncreaseChlorine)
                    {
                        case IncreaseChlorine.Chlorine_Gas:
                            constant = 1.3;
                            break;
                        case IncreaseChlorine.Calcium_Hypochlorite_67:
                            constant = 2.0;
                            break;
                        case IncreaseChlorine.Sodium_Hypochlorite_12:
                            units = "fl. " + units;
                            constant = 10.7;
                            break;
                        case IncreaseChlorine.Lihium_Hypochlorite:
                            constant = 3.8;
                            break;
                        case IncreaseChlorine.Trichlor:
                            constant = 1.5;
                            break;
                        case IncreaseChlorine.Dichlor_56:
                            constant = 2.4;
                            break;
                        case IncreaseChlorine.Dichlor_62:
                            constant = 2.1;
                            break;
                    }

                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To increase free chlorine by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.IncreaseChlorine.GetDisplay()}.";
                }
                else if (FreeChlorine > ChemicalSettings.FreeChlorineHighAlert)
                {
                    units = "oz.";
                    switch (ChemicalSettings.NeutralizeChlorine)
                    {
                        case NeutralizeChlorine.Sodium_Thiosulfate_Acid:
                            constant = 2.6;
                            break;
                        case NeutralizeChlorine.Sodium_Sulfite:
                            constant = 2.4;
                            break;
                    }
                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To decrease free chlorine by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.NeutralizeChlorine.GetDisplay()}.";
                }

                return null;
            }
        }

        public string AlkalinityError
        {
            get
            {
                var constant = 0d;
                var units = "";
                var chemPPH = 10;
                var ideal = ((ChemicalSettings.AlkalinityHighAlert + ChemicalSettings.AlkalinityLowAlert) / 2) - Alkalinity;
                var correction = Alkalinity - ideal;
                if (Alkalinity < ChemicalSettings.AlkalinityLowAlert)
                {
                    units = "lbs.";
                    switch (ChemicalSettings.IncreaseAlkalinity)
                    {
                        case IncreaseAlkalinity.Sodium_Bicarbonate:
                            constant = 1.4;
                            break;
                        case IncreaseAlkalinity.Sodium_Carbonate:
                            units = "oz.";
                            constant = 14.0;
                            break;
                        case IncreaseAlkalinity.Sodium_Sesquicarbonate:
                            constant = 1.25;
                            break;
                    }

                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To increase total alkalinity by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.IncreaseAlkalinity.GetDisplay()}.";
                }
                else if (Alkalinity > ChemicalSettings.AlkalinityHighAlert)
                {                    
                    switch (ChemicalSettings.DecreaseAlkalinity)
                    {
                        case DecreaseAlkalinity.Muriatic_Acid:
                            units = "fl. oz.";
                            constant = 26.0;
                            break;
                        case DecreaseAlkalinity.Sodium_Bisulfate:
                            units = "lbs.";
                            constant = 2.1;
                            break;
                    }
                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To decrease total alkalinity by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.DecreaseAlkalinity.GetDisplay()}.";
                }

                return null;
            }
        }

        public string pHError
        {
            get
            {
                var constant = 0d;
                var units = "";
                var chemPPH = 0.2;
                var ideal = ((ChemicalSettings.pHHighAlert + ChemicalSettings.pHLowAlert) / 2) - pH;
                var correction = pH - ideal;
                if (pH < ChemicalSettings.pHLowAlert)
                {
                    units = "oz.";
                    //Increase pH constant
                    switch (ChemicalSettings.IncreasepH)
                    {
                        case IncreasepH.Sodium_Carbonate:
                            constant = 6.0;
                            break;
                        case IncreasepH.Sodium_Hydroxide_50:
                            units = "fl. " + units;
                            constant = 5.5;
                            break;
                    }

                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To increase pH by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.IncreasepH.GetDisplay()}.";
                }
                else if (pH > ChemicalSettings.pHHighAlert)
                {
                    units = "oz.";
                    switch (ChemicalSettings.DecreasepH)
                    {
                        case DecreasepH.Muriatic_Acid:
                            units = "fl. " + units;
                            constant = 12.0;
                            break;
                        case DecreasepH.Sodium_Bisulfate:
                            units = "lbs.";
                            constant = 1.0;
                            break;
                        case DecreasepH.Carbon_Dioxide:
                            constant = 4.0;
                            break;
                    }
                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To decrease pH by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.DecreasepH.GetDisplay()}.";
                }

                return null;
            }
        }

        public string CalciumHardnessError
        {
            get
            {
                var constant = 0d;
                var units = "";
                var chemPPH = 10;
                var ideal = ((ChemicalSettings.CalciumHardnessHighAlert + ChemicalSettings.CalciumHardnessLowAlert) / 2) - CalciumHardness;
                var correction = CalciumHardness - ideal;
                if (CalciumHardness < ChemicalSettings.CalciumHardnessLowAlert)
                {
                    units = "lbs.";
                    //Increase Calcium Hardness constant
                    switch (ChemicalSettings.IncreaseCalciumHardness)
                    {
                        case IncreaseCalciumHardness.Calcium_Chloride_100:
                            constant = 0.9;
                            break;
                        case IncreaseCalciumHardness.Calcium_Chloride_77:
                            constant = 1.2;
                            break;
                    }

                    var error = CalculateBalance(ChemicalSettings.Volume, constant, ideal.Value, chemPPH);
                    return $"To increase calcium hardness by {Math.Abs(ideal.Value)} ppm add {error} {units} of { ChemicalSettings.IncreaseCalciumHardness.GetDisplay()}.";
                }
                else if (CalciumHardness > ChemicalSettings.CalciumHardnessHighAlert)
                {
                    return "Reducing hardness is done through dilution by draining your water.";
                }

                return null;
            }
        }
    }

    public class ChemicalCustomValueVM
    {
        public long? ChemicalCustomValueID { get; set; }
        public string CustomValue { get; set; }
        public ChemicalCustomFieldVM ChemicalCustomField { get; set; }
    }

    public class ChemicalRecordCommentVM
    {
        public long ChemicalRecordCommentID { get; set; }
        public long? ChemicalRecordID { get; set; }
        public string Comment { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string SubmittedBy { get; set; }
    }

    public class ChemicalRecordFormVMValidatior : AbstractValidator<ChemicalRecordFormVM>
    {
        public ChemicalRecordFormVMValidatior()
        {
            //RuleFor(cr => cr.FreeChlorine).NotEmpty();
            //RuleFor(cr => cr.TotalChlorine).NotEmpty();
            //RuleFor(cr => cr.pH).NotEmpty();
            //RuleFor(cr => cr.PoolTemp).NotEmpty();
            //RuleFor(cr => cr.AirTemp).NotEmpty();
            //RuleFor(cr => cr.WaterClarity).NotEmpty();
            //RuleFor(cr => cr.Alkalinity).NotEmpty();
            //RuleFor(cr => cr.CalciumHardness).NotEmpty();
            //RuleFor(cr => cr.Backwash).NotEmpty();
            RuleFor(cr => cr.Employee).Empty();
            RuleFor(cr => cr.Date).Empty();
        }
    }

}

