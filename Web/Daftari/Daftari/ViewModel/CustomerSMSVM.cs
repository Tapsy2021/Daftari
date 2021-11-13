using Daftari.SMSHandling.Enums;
using Daftari.ViewModel.Validations;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Daftari.ViewModel
{
    [Validator(typeof(CustomerSMSVMValidator))]
    public class CustomerSMSVM
    {
        public CustomerSMSVM()
        {
            this.Customers = new List<Guid>();
        }
        public SMSAPI SMSAPI { get; set; }
        public List<Guid> Customers { get; set; }

        public Language Language { get; set; }

        [StringLength(480)]
        [Display(Name = "SMS Message")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
    }
}