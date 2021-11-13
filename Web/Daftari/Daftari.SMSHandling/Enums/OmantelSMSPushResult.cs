using System.ComponentModel.DataAnnotations;

namespace Daftari.SMSHandling.Enums
{
    public enum OmantelSMSPushResult
    {
        [Display(Name = "Message Pushed")]
        MessagePushed = 1,

        [Display(Name = "Company Not Exists. Please check the company")]
        CompanyNotExits = 2,

        [Display(Name = "User or Password is wrong")]
        BadCredentials = 3,

        [Display(Name = "Credit is Low")]
        LowCredit = 4,

        [Display(Name = "Message is blank")]
        BlankMessage = 5,

        [Display(Name = "Message Length Exceeded")]
        MessageLengthExceeded = 6,

        [Display(Name = "Account is Inactive")]
        InactiveAccount = 7,

        [Display(Name = "No Recipient found, array length is zero")]
        NoRecipientFound = 8,

        [Display(Name = "One or more mobile numbers are of invalid length")]
        Mobileinvalidlength = 9,

        [Display(Name = "Invalid Language")]
        InvalidLanguage = 10,

        [Display(Name = "Un Known Error")]
        UnKnownError = 11,

        [Display(Name = "Account is Blocked by administrator, concurrent failure of login")]
        AccountBlocked = 12,

        [Display(Name = "Account Expired")]
        AccountExpired = 13,

        [Display(Name = "Credit Expired")]
        CreditExpired = 14,

        [Display(Name = "Web Service User Id not configured with Infocomm")]
        UserIdnotconfigured = 18,

        [Display(Name = "Client IP Has been Blocked, Please contact to Administrator")]
        ClientIPBlocked = 20,

        [Display(Name = "Client IP is outside Oman, Outside Oman IP is not allowed to access web service")]
        OutsideOman = 21,

        [Display(Name = "Mobile Number Optout by the customer. SMS Not Sent.")]
        ClientOptOut = 23,

        [Display(Name = "Error Code not registered")]
        NewErrorCode = 1000
    }
}