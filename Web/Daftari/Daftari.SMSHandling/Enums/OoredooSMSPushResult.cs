using System.ComponentModel.DataAnnotations;

namespace Daftari.SMSHandling.Enums
{
    public enum OoredooSMSPushResult
    {
        Success,

        [Display(Name = "Invalid Username / Password")]
        InvalidUser,

        [Display(Name = "Account Deactivated")]
        AccountDeactivated,

        [Display(Name = "Account Locked")]
        AccountLocked,

        [Display(Name = "Priority Support 1 to 10 only")]
        InvalidPrioritySupport,

        [Display(Name = @"Invalid Schedule Time(Date should MM\DD\YYYY)")]
        InvalidScheduleTime,

        [Display(Name = "Invalid Source Key")]
        InvalidSourceKey,

        [Display(Name = "Invalid Sender")]
        InvalidSender,

        [Display(Name = "Insufficient Credit")]
        InsufficientCredit,

        [Display(Name = "Account Expired")]
        AccountExpired,

        [Display(Name = "Invalid Message")]
        InvalidMessage,

        [Display(Name = "Phone no is exceeding 50 number")]
        TooManyPhoneNo,

        [Display(Name = "Internal Error")]
        InternalError77 = 77,

        [Display(Name = "Internal Error")]
        InternalError88 = 88,

        [Display(Name = "Internal Error")]
        InternalError99 = 99,

        [Display(Name = "Error Code not registered")]
        NewErrorCode = 1000
    }
}