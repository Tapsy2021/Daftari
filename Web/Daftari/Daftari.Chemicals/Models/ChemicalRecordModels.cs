using Daftari.Chemicals.Enum;
using LukeApps.TrackingExtended;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.Chemicals.Models
{
    public class ChemicalRecord : IAuditDetail 
    {
        public ChemicalRecord()
        {
            AuditDetail = new AuditDetail();
            ChemicalCustomValues = new HashSet<ChemicalCustomValue>();
            ChemicalRecordComments = new HashSet<ChemicalRecordComment>();
        }
        [Key]
        public long ChemicalRecordID { get; set; }
        //public Guid ChemicalRecordSettingsID { get; set; }

        [Display(Name = "Free Chlorine (ppm)")]
        public double? FreeChlorine { get; set; }

        [Display(Name = "Total Chlorine (ppm)")]
        public double? TotalChlorine { get; set; }
        [Display(Name = "Total Bromine (ppm)")]
        public double? TotalBromine { get; set; }

        [Display(Name = "pH")]
        public double? pH { get; set; }

        [Display(Name = "Pool Temp (C)")]
        public double? PoolTemp { get; set; }

        [Display(Name = "Air Temp (C)")]
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
        [Display(Name = "Employee")]
        public string Employee { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Submitted By")] 
        public string SubmittedBy { get; set; }

        [Display(Name = "Unit Of Measurement")]
        public string UnitOfMeasurement { get; set; }

        public ICollection<ChemicalCustomValue> ChemicalCustomValues { get; set; }
        public virtual ChemicalSettings ChemicalSettings { get; set; }
        public ICollection<ChemicalRecordComment> ChemicalRecordComments { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
        public double? TempConverter(double? val)
        {
            if (!val.HasValue)
                return null;

            if (UnitOfMeasurement == ChemicalSettings.UnitOfMeasurement)
                return val;
            
            if (UnitOfMeasurement == Enum.UnitOfMeasurement.Metric.ToString())
                return (val.Value * 9f / 5f) + 32;
            else
                return (val.Value - 32) * 5f / 9f;
        }
    }

    public class ChemicalRecordComment
    {        
        public ChemicalRecordComment()
        {
            AuditDetail = new AuditDetail();
        }
        public long ChemicalRecordCommentID { get; set; }
        public long? ChemicalRecordID { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string SubmittedBy { get; set; }
        public AuditDetail AuditDetail { get; set; }
    }

    public class ChemicalCustomValue : IAuditDetail
    {
        public ChemicalCustomValue()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        public long ChemicalCustomValueID { get; set; }
        public long? ChemicalRecordID { get; set; }
        public long? ChemicalCustomFieldID { get; set; }
        [Display(Name = "Custom Value")]        
        public string CustomValue { get; set; }
        public ChemicalCustomField ChemicalCustomField { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ChemicalSettings : IAuditDetail
    {
        public ChemicalSettings()
        {
            AuditDetail = new AuditDetail();
            ChemicalCustomFields = new HashSet<ChemicalCustomField>();
            ChemicalRecords = new HashSet<ChemicalRecord>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Chemical Settings ID")]
        public Guid ChemicalRecordSettingsID { get; set; }

        [Display(Name = "Volume")]
        public double Volume { get; set; }

        [Display(Name = "Increase Chlorine Chemical")]
        public IncreaseChlorine IncreaseChlorine { get; set; }

        [Display(Name = "Neutralize Chlorine Chemical")]
        public NeutralizeChlorine NeutralizeChlorine { get; set; }

        [Display(Name = "Increase Total Alkalinity Chemical")]
        public IncreaseAlkalinity IncreaseAlkalinity { get; set; }

        [Display(Name = "Decrease Total Alkalinity Chemical")]
        public DecreaseAlkalinity DecreaseAlkalinity { get; set; }

        [Display(Name = "Increase Calcium Hardness Chemical")]
        public IncreaseCalciumHardness IncreaseCalciumHardness { get; set; }

        [Display(Name = "Increase Stabilizer Chemical")]
        public IncreaseStabilizer IncreaseStabilizer { get; set; }

        [Display(Name = "Increase pH Chemical")]
        public IncreasepH IncreasepH { get; set; }

        [Display(Name = "Decrease pH Chemical")]
        public DecreasepH DecreasepH { get; set; }

        //Default Fields
        [Display(Name = "Free Chlorine (ppm)", ShortName = "Free Cl")]
        public Visibility FreeChlorine { get; set; }

        [Display(Name = "Free Chlorine Low Alert Setpoint (ppm)")]
        public double FreeChlorineLowAlert { get; set; }

        [Display(Name = "Free Chlorine High Alert Setpoint (ppm)")]
        public double FreeChlorineHighAlert { get; set; }

        [Display(Name = "Total Chlorine (ppm)", ShortName = "Total Cl")]
        public Visibility TotalChlorine { get; set; }

        [Display(Name = "Total Chlorine Low Alert Setpoint (ppm)")]
        public double TotalChlorineLowAlert { get; set; }

        [Display(Name = "Total Chlorine High Alert Setpoint (ppm)")]
        public double TotalChlorineHighAlert { get; set; }


        [Display(Name = "Total Bromine (ppm)", ShortName = "Total Br")]
        public Visibility TotalBromine { get; set; }

        [Display(Name = "Total Bromine Low Alert Setpoint (ppm)")]
        public double TotalBromineLowAlert { get; set; }

        [Display(Name = "Total Bromine High Alert Setpoint (ppm)")]
        public double TotalBromineHighAlert { get; set; }


        [Display(Name = "pH", ShortName = "pH")]
        public Visibility pH { get; set; }

        [Display(Name = "pH Low Alert Setpoint")]
        public double pHLowAlert { get; set; }

        [Display(Name = "pH High Alert")]
        public double pHHighAlert { get; set; }

        //(F)
        [Display(Name = "Pool Temp", ShortName = "Pool")]
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
        [Display(Name = "Air Temp", ShortName = "Air")]
        public Visibility AirTemp { get; set; }

        [Display(Name = "Water Clarity", ShortName = "Water Clarity")]
        public Visibility WaterClarity { get; set; }


        [Display(Name = "Alkalinity", ShortName = "Alk")]
        public Visibility Alkalinity { get; set; }

        [Display(Name = "Alkalinity Low Alert Setpoint (ppm)")]
        public double AlkalinityLowAlert { get; set; }

        [Display(Name = "Alkalinity High Alert Setpoint (ppm)")]
        public double AlkalinityHighAlert { get; set; }


        [Display(Name = "Calcium Hardness", ShortName = "CH")]
        public Visibility CalciumHardness { get; set; }

        [Display(Name = "Calcium Hardness Low Alert Setpoint (ppm)")]
        public double CalciumHardnessLowAlert { get; set; }

        [Display(Name = "Calcium Hardness High Alert Setpoint (ppm)")]
        public double CalciumHardnessHighAlert { get; set; }


        [Display(Name = "HRR/ORP", ShortName = "HRR/ORP")]
        public Visibility HRR_ORP { get; set; }

        [Display(Name = "Backwash", ShortName = "Backwash")]
        public Visibility Backwash { get; set; }

        [Display(Name = "Notes", ShortName = "Notes")]
        public Visibility Notes { get; set; }
        public string SubDomain { get; set; }

        [Display(Name = "Unit Of Measurement")]
        public string UnitOfMeasurement { get; set; }

        [Display(Name = "Pool Daily Capacity")]
        public int PoolDailyCapacity { get; set; }

        [NotMapped]
        public string TempUnits { get { return Enum.UnitOfMeasurement.Imperial.ToString().Equals(UnitOfMeasurement) ? "(F)" : "(C)"; } }

        [NotMapped]
        public string VolumeUnits { get { return Enum.UnitOfMeasurement.Imperial.ToString().Equals(UnitOfMeasurement) ? "(gal)" : "(L)"; } }

        [NotMapped]
        public double AirTempMax => UnitOfMeasurement == Enum.UnitOfMeasurement.Metric.ToString() ? 40 : 104;

        [NotMapped]
        public double PoolTempMax => UnitOfMeasurement == Enum.UnitOfMeasurement.Metric.ToString() ? 50 : 122;

        public virtual ICollection<ChemicalCustomField> ChemicalCustomFields { get; set; }
        public virtual ICollection<ChemicalRecord> ChemicalRecords { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ChemicalCustomField : IAuditDetail
    {
        public ChemicalCustomField()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        public long ChemicalCustomFieldID { get; set; }
        public Guid? ChemicalRecordSettingsID { get; set; }
        [Display(Name = "Label")]
        public string Label { get; set; }

        [Display(Name = "Type")]
        public InputType InputType { get; set; }
        [Display(Name = "Required")]
        public YesNo Required { get; set; }
        public string SelectOptions { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }        
    }
}
