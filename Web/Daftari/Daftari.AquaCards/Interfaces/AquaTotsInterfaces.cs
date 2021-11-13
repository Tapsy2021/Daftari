using Daftari.AquaCards.Enum;
using System;

namespace Daftari.AquaCards.Interfaces
{
    public interface ICustomer
    {
        string Address { get; set; }
        string AlternatePhone { get; set; }
        DateTime? Birthday { get; set; }
        string CellPhone { get; set; }
        string City { get; set; }
        Guid CustomerID { get; set; }

        string EmailAddress { get; set; }
        string FatherName { get; set; }
        string FullName { get; set; }
        bool IsMember { get; set; }
        bool IsSubToCommunications { get; set; }
        string MotherName { get; set; }
        string PrimaryPhone { get; set; }
        string Referral { get; set; }
        string Region { get; set; }
    }
}