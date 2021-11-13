using LukeApps.EmployeeData;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LukeApps.TrackingExtended
{
    public class AuditDetail
    {
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created Entry User")]
        public string CreatedEntryUserID { get; set; }

        [Display(Name = "Created Entry User")]
        public Employee CreatedEntryUser => EmployeeProvider.GetEmployeeProvider().GetUserData(CreatedEntryUserID) ?? new Employee();

        public string CreatedEntryUserDisplayName => EmployeeProvider.GetEmployeeProvider().GetDisplayName(CreatedEntryUserID);

        [Display(Name = "Last Modified Date")]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Name = "Last Modified Entry User")]
        public string LastModifiedEntryUserID { get; set; }

        [Display(Name = "Last Modified Entry User")]
        public Employee LastModifiedEntryUser => EmployeeProvider.GetEmployeeProvider().GetUserData(LastModifiedEntryUserID);

        public string LastModifiedEntryUserDisplayName => LastModifiedEntryUserID == null ? "-" : EmployeeProvider.GetEmployeeProvider().GetDisplayName(LastModifiedEntryUserID);

        /// <summary>
        /// Simple Summary of the AuditDetail
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("CreatedDate: ");
            sb.Append(CreatedDate.ToString());
            sb.Append(", CreatedEntryUser: ");
            sb.Append(CreatedEntryUserDisplayName);
            sb.Append(", LastModifiedDate: ");
            sb.Append(LastModifiedDate?.ToString());
            sb.Append(", LastModifiedEntryUser: ");
            sb.Append(LastModifiedEntryUserDisplayName);

            return sb.ToString();
        }
    }
}