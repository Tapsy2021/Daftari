using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.Pike13Api.Models
{
    public class AccountPeople
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountPeopleID { get; set; }

        public long PersonID { get; set; }
        public string Photo { get; set; }
        public long BusinessID { get; set; }

        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }

        public string Subdomain { get; set; }
        public string Role { get; set; }
        public string IsClient { get; set; }
        public string TimeZone { get; set; }

        public long TokenID { get; set; }

        public virtual Token Token { get; set; }

        public string Username => Token.Username;

        public string AccessToken => Token.AccessToken;

        public bool IsActive { get; set; }
        public string PersonName { get; set; }
    }
}