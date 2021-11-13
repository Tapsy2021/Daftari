using Daftari.AquaCards.Models;
using LukeApps.TrackingExtended;
using System.Data.Entity;
using TrackerEnabledDbContext.Common.Extensions;

namespace Daftari.AquaCards.DAL
{
    public class AquaCardsEntities : ExtendedEntities
    {
        public AquaCardsEntities(string username)
                    : base(username, "AquaTotsEntities")
        {
        }

        public AquaCardsEntities()
            : base("", "AquaTotsEntities")
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<StudentCard> StudentCards { get; set; }
        public virtual DbSet<StudentCardDetail> StudentCardDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var CustomersEntity = modelBuilder.Entity<Customer>();
            CustomersEntity.TrackAllProperties()
                           .Except(c => c.FullName)
                           .And(c => c.IsMember)
                           .And(c => c.MotherName)
                           .And(c => c.FatherName)
                           .And(c => c.CustomerStatus)
                           .And(c => c.Address)
                           .And(c => c.City)
                           .And(c => c.Region)
                           .And(c => c.IsMember)
                           .And(c => c.IsMember)
                           .And(c => c.MotherName)
                           .And(c => c.CellPhone)
                           .And(c => c.AlternatePhone)
                           .And(c => c.Referral)
                           .And(c => c.Reference)
                           .And(c => c.GuardianName)
                           .And(c => c.PhotoMD)
                           .And(c => c.PhotoLG)
                           .And(c => c.Dependants)
                           .And(c => c.Providers)
                           .And(c => c.AuditDetail);

            CustomersEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var SkillEntity = modelBuilder.Entity<Skill>();
            SkillEntity.TrackAllProperties();
            SkillEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var StudentCardEntity = modelBuilder.Entity<StudentCard>();
            StudentCardEntity.TrackAllProperties();
            StudentCardEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var StudentCardDetailEntity = modelBuilder.Entity<StudentCardDetail>();
            StudentCardDetailEntity.TrackAllProperties();
            StudentCardDetailEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
        }
    }
}