using Daftari.SMSHandling.Enums;
using Daftari.ViewModel.Validations;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Daftari.ViewModel
{
    [Validator(typeof(CustomerMailsVMValidator))]
    public class CustomerMailsVM
    {
        public CustomerMailsVM()
        {
            this.Customers = new List<Guid>();
        }

        public List<Guid> Customers { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string EmailMessage { get; set; }

        public string MessageDelta { get; set; } = "[]";
    }
}