using Daftari.Forms.Models;
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

namespace Daftari.Forms.DAL
{
    public class FormsEntities : ExtendedEntities
    {
        public FormsEntities(string username)
                    : base(username, "FormsEntities")
        {
        }

        public FormsEntities()
            : base("", "FormsEntities")
        {
        }

        public virtual DbSet<FormSettings> FormSettings { get; set; }
        public virtual DbSet<FormCustomField> FormCustomFields { get; set; }
        public virtual DbSet<FormSignatureField> FormSignatureFields { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormCustomValue> FormCustomValues { get; set; }
        public virtual DbSet<FormSignatureValue> FormSignatureValues { get; set; }
        public virtual DbSet<FormAttachment> FormAttachments { get; set; }
        public virtual DbSet<ApprovalProcess> ApprovalProcess { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var FormSettingsEntity = modelBuilder.Entity<FormSettings>();
            FormSettingsEntity.TrackAllProperties();
            FormSettingsEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var FormEntity = modelBuilder.Entity<Form>();
            FormEntity.TrackAllProperties();
            FormEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var FormCustomFieldEntity = modelBuilder.Entity<FormCustomField>();
            FormCustomFieldEntity.TrackAllProperties();
            FormCustomFieldEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var FormSignatureFieldEntity = modelBuilder.Entity<FormSignatureField>();
            FormSignatureFieldEntity.TrackAllProperties();
            FormSignatureFieldEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            var FormAttachmentEntity = modelBuilder.Entity<FormAttachment>();
            FormAttachmentEntity.TrackAllProperties();
            FormAttachmentEntity.Map(m => m.Requires("IsDeleted").HasValue(false))
                .Ignore(m => m.IsDeleted);

            modelBuilder.Entity<ApprovalProcess>().HasKey((ApprovalProcess ug) => new { ug.FormID, ug.UserName });
        }
    }
}
