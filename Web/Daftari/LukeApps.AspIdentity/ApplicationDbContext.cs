using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace LukeApps.AspIdentity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("UserEntities", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //
        // Summary:
        //     IDbSet of Groups
        public virtual IDbSet<ApplicationGroup> Groups { get; set; }
        public virtual IDbSet<ApplicationUserGroup> UserGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //// Add the group stuff here:
            modelBuilder.Entity<ApplicationUser>().HasMany((ApplicationUser u) => u.Groups);
            modelBuilder.Entity<ApplicationUserGroup>().HasKey((ApplicationUserGroup r) => new { r.UserId, r.GroupId }).ToTable("AspNetUserGroups");

            //// And Here:
            EntityTypeConfiguration<ApplicationGroup> groupsConfig = modelBuilder.Entity<ApplicationGroup>().ToTable("AspNetGroups");
            groupsConfig.Property((ApplicationGroup r) => r.Name).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
