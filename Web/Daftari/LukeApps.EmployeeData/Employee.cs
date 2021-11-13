namespace LukeApps.EmployeeData
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum UserStatus
    {
        Active,
        Disabled,

        [Display(Name = "Not Available")]
        NotAvailable,

        Error
    }


    public class Employee
    {
        [Key]
        [Display(Name = "Employee Number")]
        public string Username { get; set; }

        public string Initials { get; set; }

        [Display(Name = "Employee Name")]
        public string DisplayName { get; set; }

        [NotMapped]
        public string Summary => $"{Username} - {DisplayName} {(Initials != null ? $"({Initials})" : "")} {(UserStatus != UserStatus.Active ? $"({UserStatus})" : "")}";

        [Display(Name = "Company Email 1")]
        public string CompanyMail { get; set; }

        public string Mail => CompanyMail ?? CompanyMail2;

        [Display(Name = "Company Email 2")]
        public string CompanyMail2 { get; set; }

        [Display(Name = "Telephone Number")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Electronic Signature")]
        public string ElectronicSignature { get; set; }

        [Display(Name = "Display Picture")]
        public string DisplayPicture { get; set; }

        [Display(Name = "User Status")]
        public UserStatus UserStatus { get; set; } = UserStatus.Active;

        public string Location { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [NotMapped]
        [Display(Name = "Department Number")]
        public string DepartmentNumber => DepartmentOverride ?? OfficialDepartment;

        [Display(Name = "Official Department")]
        public string OfficialDepartment { get; set; }

        [Display(Name = "Department Override")]
        public string DepartmentOverride { get; set; }

        [Display(Name = "Display Picture Present?")]
        public bool IsPicturePresent => DisplayPicture != null;

        [Display(Name = "Signature Present?")]
        public bool IsSignaturePresent => ElectronicSignature != null;

        public override bool Equals(object userR) => userR == null ? false : (this.Username == ((Employee)userR).Username);

        public static bool operator ==(Employee userL, Employee userR)
        {
            if (userL is null)
                return userR is null;

            return userL.Equals(userR);
        }

        public static bool operator !=(Employee userL, Employee userR) => !(userL == userR);

        public override int GetHashCode() => base.GetHashCode();
    }
}