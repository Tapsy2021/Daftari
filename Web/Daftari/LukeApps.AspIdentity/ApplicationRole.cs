using Microsoft.AspNet.Identity.EntityFramework;

namespace LukeApps.AspIdentity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string name) : base(name)
        {
        }

        public string Description { get; set; }

        public string LDAPReference { get; set; }
    }
}
