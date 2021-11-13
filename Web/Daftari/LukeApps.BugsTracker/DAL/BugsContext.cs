namespace LukeApps.DAL
{
    using BugsTracker.Models;
    using System.Data.Entity;

    public partial class BugsContext : DbContext
    {
        public BugsContext()
            : base("name=BugsContext")
        {
        }

        public virtual DbSet<Bug> Bugs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}