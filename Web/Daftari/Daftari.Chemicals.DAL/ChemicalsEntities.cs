using Daftari.Chemicals.Models;
using LukeApps.TrackingExtended;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TrackerEnabledDbContext;
using TrackerEnabledDbContext.Common.Extensions;

namespace Daftari.Chemicals.DAL
{
    public class ChemicalsEntities : ExtendedEntities
    {
        public ChemicalsEntities(string username)
                    : base(username, "ChemicalsEntities")
        {
        }

        public ChemicalsEntities()
            : base("", "ChemicalsEntities")
        {
        }

        public virtual DbSet<ChemicalSettings> ChemicalSettings { get; set; }
        public virtual DbSet<ChemicalCustomField> ChemicalCustomFields { get; set; }
        public virtual DbSet<ChemicalRecord> ChemicalRecords { get; set; }
        public virtual DbSet<ChemicalCustomValue> ChemicalCustomValues { get; set; }

        public virtual DbSet<ChemicalRecordComment> ChemicalRecordComments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var ChemicalSettingsEntity = modelBuilder.Entity<ChemicalSettings>();
            ChemicalSettingsEntity.TrackAllProperties();
            ChemicalSettingsEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var ChemicalRecordEntity = modelBuilder.Entity<ChemicalRecord>();
            ChemicalRecordEntity.TrackAllProperties();
            ChemicalRecordEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var ChemicalCustomFieldEntity = modelBuilder.Entity<ChemicalCustomField>();
            ChemicalCustomFieldEntity.TrackAllProperties();
            ChemicalCustomFieldEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);
        }
    }
}
