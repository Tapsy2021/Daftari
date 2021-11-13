using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Daftari.Pike13Api.Models
{
    public class Pike13Person
    {
        [Display(Name = "ID")]
        public long id { get; set; }

        [Display(Name = "First Name")]
        public string first_name { get; set; }

        [Display(Name = "Middle Name")]
        public string middle_name { get; set; }

        [Display(Name = "Last Name")]
        public string last_name { get; set; }

        [Display(Name = "Guardian Name")]
        public string guardian_name { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Membership")]
        public string membership { get; set; }

        [Display(Name = "Is Member")]
        public string is_member { get; set; }

        [Display(Name = "Joined At")]
        public string joined_at { get; set; }

        [Display(Name = "Phone")]
        public string phone { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Secondary Info Field")]
        public string secondary_info_field { get; set; }

        [Display(Name = "Guardian Email")]
        public string guardian_email { get; set; }

        [Display(Name = "Timezone")]
        public string timezone { get; set; }

        [Display(Name = "Apdated At")]
        public string updated_at { get; set; }

        [Display(Name = "Deleted At")]
        public string deleted_at { get; set; }

        [Display(Name = "Hidden At")]
        public string hidden_at { get; set; }

        [Display(Name = "BirthDate")]
        public string birthdate { get; set; }

        [Display(Name = "Location ID")]
        public string location_id { get; set; }

        [Display(Name = "Custom Fields")]
        public List<custom_field> custom_fields { get; set; }

        [Display(Name = "Profile Photo")]
        public Pike13ProfilePhoto profile_photo { get; set; }

        [Display(Name = "Dependents")]
        public List<Pike13Person> dependents { get; set; }

        [Display(Name = "Dependents")]
        public List<Pike13Person> providers { get; set; }

        [Display(Name = "Business ID")]
        public string business_id { get; set; }

        public string business_name { get; set; }
        public string subdomain { get; set; }
        public string role { get; set; }
        public string is_client { get; set; }
    }
}