using Daftari.AquaCards.Enum;
using Daftari.AquaCards.Interfaces;
using Daftari.AquaCards.Models;
using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Daftari.ViewModel
{
    [Validator(typeof(RegistrationFormVMValidatior))]
    public class RegistrationFormVM : ICustomer
    {
        public RegistrationFormVM()
        {
        }

        public string Address { get; set; }

        [Display(Name = "Alternate Phone")]
        public string AlternatePhone { get; set; }

        public DateTime? Birthday { get; set; }

        [Display(Name = "Cell Phone")]
        public string CellPhone { get; set; }

        public string City { get; set; }
        public Guid CustomerID { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Father/Guardian Name")]
        public string FatherName { get; set; }

        [Display(Name = "Guardian Name")]
        public string FullName { get; set; }

        [Display(Name = "Member?")]
        public bool IsMember { get; set; }

        [Display(Name = "Subscribed To Communications?")]
        public bool IsSubToCommunications { get; set; }

        [Display(Name = "Mother/Guardian Name")]
        public string MotherName { get; set; }

        [Display(Name = "Primary Phone")]
        public string PrimaryPhone { get; set; }

        public string Referral { get; set; }
        public string Region { get; set; }
    }

    public class RegistrationFormSummaryVM : RegistrationFormVM
    {
        public RegistrationFormSummaryVM(Customer customer)
            : base()
        {
            CustomerID = customer.CustomerID;
            Reference = customer.Reference.ToString();
            Date = customer.AuditDetail.CreatedDate.ToString("dd/MM/yyyy");
            MotherName = customer.MotherName ?? "-";
            FatherName = customer.FatherName ?? "-";
            Address = customer.Address ?? "-";
            City = customer.City ?? "-";
            Region = customer.Region ?? "-";
            PrimaryPhone = customer.PrimaryPhone;
            CellPhone = customer.CellPhone ?? "-";
            AlternatePhone = customer.AlternatePhone ?? "-";
            EmailAddress = customer.EmailAddress;
            Referral = customer.Referral;

            IsNewCustomer = (customer.CustomerStatus == CustomerStatus.New);
            IsReturningCustomer = (customer.CustomerStatus == CustomerStatus.Returning);
        }

        public string Date { get; set; }

        public bool IsNewCustomer { get; set; }
        public bool IsReturningCustomer { get; set; }

        public string Reference { get; set; }
    }

    public class RegistrationFormVMValidatior : AbstractValidator<RegistrationFormVM>
    {
        public RegistrationFormVMValidatior()
        {
            RuleFor(customer => customer.FullName).NotEmpty().When(customer => customer.MotherName == string.Empty || customer.MotherName == null || customer.FatherName == string.Empty || customer.FatherName == null).WithMessage("Please enter atleast one Guardian's Name");
            RuleFor(customer => customer.PrimaryPhone).NotEmpty();
            RuleFor(customer => customer.PrimaryPhone).Matches("([2,7,9])[0-9]{7}").WithMessage("Invalid Phone Number, Phone Number Format: (2, 7 or 9)xxxxxxx");
            RuleFor(customer => customer.PrimaryPhone).Length(8);
            RuleFor(customer => customer.CellPhone).Matches("([2,7,9])[0-9]{7}").WithMessage("Invalid Phone Number, Phone Number Format: (2, 7 or 9)xxxxxxx");
            RuleFor(customer => customer.CellPhone).Length(8);
            RuleFor(customer => customer.AlternatePhone).Matches("([2,7,9])[0-9]{7}").WithMessage("Invalid Phone Number, Phone Number Format: (2, 7 or 9)xxxxxxx");
            RuleFor(customer => customer.AlternatePhone).Length(8);
            RuleFor(customer => customer.EmailAddress).EmailAddress().WithMessage("A valid email address is required");
        }
    }
}