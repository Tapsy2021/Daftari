using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LukeApps.AspIdentity
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
            : base()
        {
            Groups = new HashSet<ApplicationUserGroup>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public string ProfilePhoto { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Initials { get; set; }

        public string ESignature { get; set; }
        public string LDAPReference { get; set; }
        public string JobTitle { get; set; }
        public string TelephoneNumber { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
    }
}