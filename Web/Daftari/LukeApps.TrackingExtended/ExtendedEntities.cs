using LukeApps.TrackingExtended.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TrackerEnabledDbContext;
using TrackerEnabledDbContext.Common.Configuration;
using TrackerEnabledDbContext.Common.Extensions;

namespace LukeApps.TrackingExtended
{
    public class ExtendedEntities : TrackerContext, IExtendedContext
    {
        public ExtendedEntities(string _defaultUsername, string nameOrConnectionString)
                        : base(nameOrConnectionString)
        {
            isAuditDetailEnabled = true;
            ConfigureUsername(_defaultUsername);
        }

        private string defaultUsername;

        private bool isAuditDetailEnabled;
        public bool IsAuditDetailEnabled
        {
            get { return isAuditDetailEnabled; }
            set
            {
                //    GlobalTrackingConfig.Enabled = value;
                isAuditDetailEnabled = value;
            }
        }

        public override void ConfigureUsername(string defaultUsername)
        {
            this.defaultUsername = defaultUsername;
            base.ConfigureUsername(defaultUsername);
        }

        public string GetUsername() => defaultUsername;

        public override int SaveChanges()
        {
            if (IsAuditDetailEnabled)
                setAuditDetail(defaultUsername);

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join(" ; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            if (IsAuditDetailEnabled)
                setAuditDetail(defaultUsername);

            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                       .SelectMany(x => x.ValidationErrors)
                       .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join(" ; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        protected void IgnoreDeleteAndTrack<TEntityType>(EntityTypeConfiguration<TEntityType> entity) where TEntityType : class, IAuditDetail
        {
            entity.Map(m => m.Requires("IsDeleted").HasValue(false))
                            .Ignore(m => m.IsDeleted);
            entity.TrackAllProperties();
        }

        private void setAuditDetail(string username)
        {
            var changeSet = ChangeTracker.Entries<IAuditDetail>().ToList();

            if (changeSet != null && changeSet.Any())
            {
                if (!skipAuditDetailUpdate)
                {
                    foreach (var entry in changeSet.Where(c => c.State == EntityState.Added))
                    {
                        entry.Entity.AuditDetail.CreatedEntryUserID = username;
                    }

                    foreach (var entry in changeSet.Where(c => c.State == EntityState.Modified))
                    {
                        if (entry.Entity.IsDeleted != true)
                        {
                            entry.Entity.AuditDetail.LastModifiedDate = DateTime.Now;
                            entry.Entity.AuditDetail.LastModifiedEntryUserID = username;
                        }
                    }
                }

                foreach (var entry in changeSet.Where(c => c.State == EntityState.Deleted))
                {
                    softDelete(entry);
                }
            }
        }

        private bool skipAuditDetailUpdate;

        public bool SkipAuditDetailUpdate
        {
            set => skipAuditDetailUpdate = value;
        }

        private static Dictionary<Type, EntitySetBase> _mappingCache =
             new Dictionary<Type, EntitySetBase>();

        private string getTableName(Type type)
        {
            EntitySetBase es = getEntitySet(type);

            return string.Format("[{0}].[{1}]",
                es.MetadataProperties["Schema"].Value,
                es.MetadataProperties["Table"].Value);
        }

        private string getPrimaryKeyName(Type type)
        {
            EntitySetBase es = getEntitySet(type);

            return es.ElementType.KeyMembers[0].Name;
        }

        private EntitySetBase getEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;

                string typeName = ObjectContext.GetObjectType(type).Name;

                typeName = overrideTypeName(typeName);

                var es = octx.MetadataWorkspace
                                .GetItemCollection(DataSpace.SSpace)
                                .GetItems<EntityContainer>()
                                .SelectMany(c => c.BaseEntitySets
                                                .Where(e => e.Name == typeName))
                                .FirstOrDefault();

                if (es == null)
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }

        private void softDelete(DbEntityEntry entry)
        {
            Type entryEntityType = entry.Entity.GetType();

            string tableName = getTableName(entryEntityType);

            string primaryKeyName = getPrimaryKeyName(entryEntityType);

            string sql =
                string.Format(
                    "UPDATE {0} SET IsDeleted = 1 WHERE {1} = @id",
                        tableName, primaryKeyName);

            Database.ExecuteSqlCommand(
                sql,
                new SqlParameter("@id", entry.OriginalValues[primaryKeyName]));

            string sqlLog = "BEGIN TRANSACTION; "
                + "DECLARE @DataID int;"
                + "INSERT INTO AuditLogs (UserName,EventDateUTC,EventType,TypeFullName,RecordId) VALUES (@userName,@eventDateUTC,@eventType,@typeFullName,@recordId);"
                + "SELECT @DataID = scope_identity();"
                + "INSERT INTO AuditLogDetails (PropertyName,OriginalValue,NewValue,AuditLogId) VALUES (@propertyName,@originalValue,@newValue, @DataID);"
                + "COMMIT; ";

            Database.ExecuteSqlCommand(
                sqlLog,
                new SqlParameter("@userName", defaultUsername),
                new SqlParameter("@eventDateUTC", DateTime.UtcNow),
                new SqlParameter("@eventType", TrackerEnabledDbContext.Common.Models.EventType.SoftDeleted),
                new SqlParameter("@typeFullName", entryEntityType.BaseType.FullName),
                new SqlParameter("@recordId", entry.OriginalValues[primaryKeyName]),
                new SqlParameter("@propertyName", "IsDeleted"),
                new SqlParameter("@originalValue", "False"),
                new SqlParameter("@newValue", "True")
                );

            // prevent hard delete
            entry.State = EntityState.Detached;
        }

        private static string overrideTypeName(string typeName)
        {
            if (typeName.Contains("Transmittal"))
            {
                typeName = "Transmittal";
            }

            return typeName;
        }
    }
}