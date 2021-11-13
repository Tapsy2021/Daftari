using Daftari.Pike13Api.Models;
using System.Data.Entity;
using TrackerEnabledDbContext.Common.Extensions;

namespace Daftari.Pike13Api.DAL
{
    public partial class Pike13ApiContext : DbContext
    {
        public Pike13ApiContext()
            : base("name=Pike13ApiContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<Pike13ApiContext>());
        }

        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<AccountPeople> AccountPeoples { get; set; }

        public virtual DbSet<EventOccurrance> EventOccurrances { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }
        public virtual DbSet<TopicSubsription> TopicSubsriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventOccurrance>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted).TrackAllProperties();
            modelBuilder.Entity<Visit>()
                .Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted).TrackAllProperties();
        }
    }
}