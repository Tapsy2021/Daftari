using Daftari.Forms.Enum;
using LukeApps.TrackingExtended;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Daftari.Forms.Models
{
    public class FormSettings : IAuditDetail
    {
        public FormSettings()
        {
            AuditDetail = new AuditDetail();
            FormCustomFields = new HashSet<FormCustomField>();
            FormSignatureFields = new HashSet<FormSignatureField>();
            Forms = new HashSet<Form>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Form Settings ID")]
        public Guid FormSettingsID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Access Level")]
        public string AccessLevel { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Enable Attachment(s)")]
        public bool IsAttachmentEnabled { get; set; }

        //[NotMapped]
        //public List<string> SendNotificationsTo { get; set; }
        [Display(Name = "Send Completed Form Notifications To")]
        public string SendNotificationsTo { get; set; }

        [Display(Name = "Approval Process")]
        public string ApprovalProcess { get; set; }

        [Display(Name = "Is Public Form")]
        public bool IsPublic { get; set; }

        public string SubDomain { get; set; }
        public virtual ICollection<FormCustomField> FormCustomFields { get; set; }
        public virtual ICollection<FormSignatureField> FormSignatureFields { get; set; }
        public virtual ICollection<Form> Forms { get; set; }

        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FormCustomField : IAuditDetail
    {
        public FormCustomField()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        public long FormCustomFieldID { get; set; }
        public Guid? FormSettingsID { get; set; }
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

    public class FormSignatureField : IAuditDetail
    {
        public FormSignatureField()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        public long FormSignatureFieldID { get; set; }
        public Guid? FormSettingsID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Required")]
        public YesNo Required { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FormAttachment : IAuditDetail
    {
        public FormAttachment()
        {
            AuditDetail = new AuditDetail();
        }

        [Key]
        public Guid FormAttachmentID { get; set; }
        public long? FormID { get; set; }

        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        //public string Content { get; set; }
        public Form Form { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class Form : IAuditDetail
    {
        public Form()
        {
            AuditDetail = new AuditDetail();
            FormCustomValues = new HashSet<FormCustomValue>();
            FormSignatureValues = new HashSet<FormSignatureValue>();
            FormAttachments = new HashSet<FormAttachment>();
            ApprovalProcess = new HashSet<ApprovalProcess>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FormID { get; set; }
        public ICollection<FormCustomValue> FormCustomValues { get; set; }

        public virtual ICollection<FormSignatureValue> FormSignatureValues { get; set; }
        public virtual ICollection<FormAttachment> FormAttachments { get; set; }
        public virtual ICollection<ApprovalProcess> ApprovalProcess { get; set; }
        public virtual FormSettings FormSettings { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public ApprovalStatus? CurrentStatus
        {
            get
            {
                var list = ApprovalProcess.ToList();
                if (!list.Any())
                {
                    return null;
                }
                else if (list.All(x => x.Status == ApprovalStatus.Approved))
                {
                    return ApprovalStatus.Approved;
                }
                else if (list.Any(x => x.Status == ApprovalStatus.Rejected))
                {
                    return ApprovalStatus.Rejected;
                }
                else
                {
                    return ApprovalStatus.Pending;
                }
            }
        }
    }

    public class FormCustomValue : IAuditDetail
    {
        public FormCustomValue()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        public long FormCustomValueID { get; set; }
        public Guid? FormID { get; set; }
        public long? FormCustomFieldID { get; set; }
        [Display(Name = "Custom Value")]
        public string CustomValue { get; set; }
        public FormCustomField FormCustomField { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FormSignatureValue : IAuditDetail
    {
        public FormSignatureValue()
        {
            AuditDetail = new AuditDetail();
        }
        [Key]
        public long FormSignatureValueID { get; set; }
        public Guid? FormID { get; set; }
        public long? FormSignatureFieldID { get; set; }
        public FormSignatureField FormSignatureField { get; set; }
        public Form Form { get; set; }
        public string SignatureContent { get; set; }
        public AuditDetail AuditDetail { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ApprovalProcess
    {
        [Key]
        public long? FormID { get; set; }
        public Form Form { get; set; }
        public string UserName { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTime? LastModifiedDate {get;set;}
    }
}
