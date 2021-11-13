using Daftari.AquaCards.Enum;
using Daftari.AquaCards.Interfaces;
using Daftari.AquaCards.Validatons;
using FluentValidation.Attributes;
using LukeApps.TrackingExtended;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.AquaCards.Models
{
    [Validator(typeof(CustomerValidator))]
    public class Customer : IAuditDetail, ICustomer
    {
        public Customer()
        {
            this.AuditDetail = new AuditDetail();
            this.Reference = new Reference();
        }

        public Customer(ICustomer customer)
        {
            CustomerID = customer.CustomerID;
            FullName = customer.FullName;
            IsMember = customer.IsMember;
            IsSubToCommunications = customer.IsSubToCommunications;
            Birthday = customer.Birthday;
            MotherName = customer.MotherName;
            FatherName = customer.FatherName;
            Address = customer.Address;
            City = customer.City;
            Region = customer.Region;
            PrimaryPhone = customer.PrimaryPhone;
            CellPhone = customer.CellPhone;
            AlternatePhone = customer.AlternatePhone;
            EmailAddress = customer.EmailAddress;
            Referral = customer.Referral;
            Reference = new Reference();
            AuditDetail = new AuditDetail();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Customer ID")]
        public Guid CustomerID { get; set; }

        public string Address { get; set; }

        [Display(Name = "Alternate Phone")]
        public string AlternatePhone { get; set; }

        public AuditDetail AuditDetail { get; set; }

        [Display(Name = "Cell Phone")]
        public string CellPhone { get; set; }

        //public virtual ICollection<Child> Children { get; set; }

        public string City { get; set; }

        [Display(Name = "Customer Status")]
        public CustomerStatus CustomerStatus { get; set; } = CustomerStatus.New;

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Father/Guardian Name")]
        public string FatherName { get; set; }

        public bool IsDeleted { get; set; }

        [Display(Name = "Subscribed To Communications?")]
        public bool IsSubToCommunications { get; set; }

        [Display(Name = "Mother/Guardian Name")]
        public string MotherName { get; set; }

        [Display(Name = "Primary Phone")]
        public string PrimaryPhone { get; set; }

        public Reference Reference { get; set; }
        public string Referral { get; set; }

        [Display(Name = "Region")]
        public string Region { get; set; }

        public bool IsMember { get; set; }

        public DateTime? Birthday { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string GuardianName { get; set; }
        public long ExternalReference { get; set; }

        public string SubDomain { get; set; }
        public string PhotoMD { get; set; }
        public string PhotoLG { get; set; }
        public string Dependants { get; set; }
        public string Providers { get; set; }
    }

    public class Reference
    {
        public const string Prefix = "AQT-MCT";
        public long Sequence { get; set; }

        public override string ToString()
        {
            return Prefix + "-" + Sequence.ToString("0000000");
        }
    }
}