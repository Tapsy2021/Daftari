using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LukeApps.AspIdentity
{
    public class ApplicationGroup
    {
        public ApplicationGroup()
        {
            //UserGroups = new HashSet<ApplicationUserGroup>();
        }

        //public ApplicationGroup(string name) : this()
        //{
        //    Name = name;
        //}

        [Key]
        [Required]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
        public virtual ICollection<ApplicationUserGroup> UserGroups { get; set; }
    }
}
