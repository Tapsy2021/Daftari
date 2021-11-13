using Daftari.Forms.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Daftari.ViewModel
{
    public class FormSettingsVM
    {
        public FormSettingsVM()
        {
            FormCustomFields = new List<FormCustomFieldVM>();
            FormSignatureFields = new List<FormSignatureFieldVM>();
        }

        [Display(Name = "Form Settings ID")]
        public Guid? FormSettingsID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Access Level")]
        public string AccessLevel { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Enable Attachment(s)")]
        public bool IsAttachmentEnabled { get; set; }

        [Display(Name = "Send Completed Form Notifications To")]
        public List<string> SendNotificationsTo { get; set; }

        [Display(Name = "Approval Process")]
        public List<string> AprrovalProcess { get; set; }

        [Display(Name = "Is Public Form")]
        public bool IsPublic { get; set; }

        public string SubDomain { get; set; }
        public virtual List<FormCustomFieldVM> FormCustomFields { get; set; }
        public virtual List<FormSignatureFieldVM> FormSignatureFields { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FormCustomFieldVM
    {
        public long? FormCustomFieldID { get; set; }
        public Guid? FormSettingsID { get; set; }
        [Display(Name = "Label")]
        public string Label { get; set; }

        [Display(Name = "Type")]
        public InputType? InputType { get; set; }
        [Display(Name = "Required")]
        public YesNo? Required { get; set; }
        public string SelectOptions { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FormSignatureFieldVM
    {
        public long? FormSignatureFieldID { get; set; }
        public Guid? FormSettingsID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Required")]
        public YesNo? Required { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class CompletedFormVM
    {
        public CompletedFormVM()
        {
            FormCustomValues = new List<FormCustomValueVM>();
            FormSignatureValues = new List<FormSignatureValueVM>();
            FormAttachments = new List<FormAttachmentVM>();
            ApprovalProcess = new List<ApprovalProcessVM>();
            //Files = new List<HttpPostedFileBase>();
        }
        [Display(Name = "Form ID Number")]
        public long FormID { get; set; }
        public List<FormCustomValueVM> FormCustomValues { get; set; }

        public List<FormSignatureValueVM> FormSignatureValues { get; set; }
        [Display(Name = "Attachment(s)")]
        public List<FormAttachmentVM> FormAttachments { get; set; }
        public List<ApprovalProcessVM> ApprovalProcess { get; set; }
        public Forms.Models.FormSettings FormSettings { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string Submitted_By { get; set; }
        public DateTime? Submitted_At { get; set; }
        public ApprovalStatus? ApprovalStatus { get; set; }
        public Guid FormGuidID { get; set; }
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
    }

    public class FormAttachmentVM
    {
        public Guid FormAttachmentID { get; set; }
        public long? FormID { get; set; }

        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileSize { get; set; }
        public string Base64 { get; set; }
        public CompletedFormVM Form { get; set; }
    }

    public class FormCustomValueVM
    {
        public long FormCustomValueID { get; set; }
        public long? FormID { get; set; }
        public long? FormCustomFieldID { get; set; }
        [Display(Name = "Custom Value")]
        public string CustomValue { get; set; }
        public FormCustomFieldVM FormCustomField { get; set; }
    }

    public class FormSignatureValueVM
    {
        public long FormSignatureValueID { get; set; }
        public long? FormID { get; set; }
        public long? FormSignatureFieldID { get; set; }
        public FormSignatureFieldVM FormSignatureField { get; set; }
        public CompletedFormVM Form { get; set; }
        public string SignatureContent { get; set; }
    }

    public class ApprovalProcessVM
    {
        //public long? FormID { get; set; }
        //public Form Form { get; set; }
        public string UserName { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }




























    public class FormSettingsVMValidatior : AbstractValidator<FormSettingsVM>
    {
        public FormSettingsVMValidatior()
        {
            RuleFor(setting => setting.AccessLevel).NotEmpty();
        }
    }
}