using System.ComponentModel.DataAnnotations;

namespace Daftari.Pike13Api.Enum
{
    public enum EventOccurrenceState
    {

    }
    public enum VisitTopic
    {
        /// <summary>
        ///Triggers when a new visit is created in a state other than "reserved." Also triggers when a visit transitions from "reserved" to "registered", "completed", "noshowed", or "late_cancel."
        /// </summary>
        [Display(Name = "visit.new")]
        New,
        /// <summary>
        /// Triggers whenever a new visit is created, including those in a pre-booked "reserved" state.
        /// </summary>
        //[Display(Name = "visit.created")]
        //Created,
        /// <summary>
        /// Triggers when a visit is deleted.
        /// </summary>
        [Display(Name = "visit.deleted")]
        Deleted,
        /// <summary>
        /// Triggers when specific fields of a visit are updated.
        /// </summary>
        [Display(Name = "visit.updated")]
        Updated
    }

    public enum PersonState
    {
        /// <summary>
        /// Triggers when a new person is added to a business.
        /// </summary>
        [Display(Name = "person.created")]
        Created,
        /// <summary>
        /// Triggers when a person is deleted.
        /// </summary>
        [Display(Name = "person.deleted")]
        Deleted,
        /// <summary>
        /// Triggers when a specific fields of a person are updated.
        /// </summary>
        [Display(Name = "person.updated")]
        Updated
    }

    public enum VisitStatus
    {
        //[Display(Name = "register")]
        //Register,
        [Display(Name = "complete")]
        Complete,

        [Display(Name = "late_cancel")]
        Late_Cancel,

        [Display(Name = "noshow")]
        NoShow,
        //[Display(Name = "reset")]
        //Reset
        [Display(Name = "unpaid")]
        Unpaid,

        [Display(Name = "enrolled")]
        Enrolled,

        [Display(Name = "incomplete")]
        Incomplete,

        [Display(Name = "first_visit")]
        First_Visit
    }

    public enum VisitState
    {
        [Display(Name = "completed")]
        Completed,
        [Display(Name = "late_canceled")]
        Late_Canceled,
        [Display(Name = "noshowed")]
        NoShowed,
        [Display(Name = "registered")]
        Registered
    }
}
