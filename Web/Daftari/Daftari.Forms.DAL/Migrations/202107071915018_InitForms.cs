namespace Daftari.Forms.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitForms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        AuditLogId = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        EventDateUTC = c.DateTime(nullable: false),
                        EventType = c.Int(nullable: false),
                        TypeFullName = c.String(nullable: false, maxLength: 512),
                        RecordId = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.AuditLogId);
            
            CreateTable(
                "dbo.AuditLogDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PropertyName = c.String(nullable: false, maxLength: 256),
                        OriginalValue = c.String(),
                        NewValue = c.String(),
                        AuditLogId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditLogs", t => t.AuditLogId, cascadeDelete: true)
                .Index(t => t.AuditLogId);
            
            CreateTable(
                "dbo.LogMetadata",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuditLogId = c.Long(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditLogs", t => t.AuditLogId, cascadeDelete: true)
                .Index(t => t.AuditLogId);
            
            CreateTable(
                "dbo.FormAttachments",
                c => new
                    {
                        FormAttachmentID = c.Guid(nullable: false),
                        FormID = c.Long(),
                        FileName = c.String(),
                        FileBytes = c.Binary(),
                        ContentType = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FormAttachmentID)
                .ForeignKey("dbo.Forms", t => t.FormID)
                .Index(t => t.FormID);
            
            CreateTable(
                "dbo.Forms",
                c => new
                    {
                        FormID = c.Long(nullable: false, identity: true),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        FormSettings_FormSettingsID = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FormID)
                .ForeignKey("dbo.FormSettings", t => t.FormSettings_FormSettingsID)
                .Index(t => t.FormSettings_FormSettingsID);
            
            CreateTable(
                "dbo.FormCustomValues",
                c => new
                    {
                        FormCustomValueID = c.Long(nullable: false, identity: true),
                        FormID = c.Guid(),
                        FormCustomFieldID = c.Long(),
                        CustomValue = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Form_FormID = c.Long(),
                    })
                .PrimaryKey(t => t.FormCustomValueID)
                .ForeignKey("dbo.FormCustomFields", t => t.FormCustomFieldID)
                .ForeignKey("dbo.Forms", t => t.Form_FormID)
                .Index(t => t.FormCustomFieldID)
                .Index(t => t.Form_FormID);
            
            CreateTable(
                "dbo.FormCustomFields",
                c => new
                    {
                        FormCustomFieldID = c.Long(nullable: false, identity: true),
                        FormSettingsID = c.Guid(),
                        Label = c.String(),
                        InputType = c.Int(nullable: false),
                        Required = c.Int(nullable: false),
                        SelectOptions = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FormCustomFieldID)
                .ForeignKey("dbo.FormSettings", t => t.FormSettingsID)
                .Index(t => t.FormSettingsID);
            
            CreateTable(
                "dbo.FormSettings",
                c => new
                    {
                        FormSettingsID = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        AccessLevel = c.Int(nullable: false),
                        Description = c.String(),
                        IsAttachmentEnabled = c.Boolean(nullable: false),
                        SendNotificationsTo = c.String(),
                        SubDomain = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FormSettingsID);
            
            CreateTable(
                "dbo.FormSignatureFields",
                c => new
                    {
                        FormSignatureFieldID = c.Long(nullable: false, identity: true),
                        FormSettingsID = c.Guid(),
                        Name = c.String(),
                        Required = c.Int(nullable: false),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FormSignatureFieldID)
                .ForeignKey("dbo.FormSettings", t => t.FormSettingsID)
                .Index(t => t.FormSettingsID);
            
            CreateTable(
                "dbo.FormSignatureValues",
                c => new
                    {
                        FormSignatureValueID = c.Long(nullable: false, identity: true),
                        FormID = c.Guid(),
                        FormSignatureFieldID = c.Long(),
                        SignatureContent = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Form_FormID = c.Long(),
                    })
                .PrimaryKey(t => t.FormSignatureValueID)
                .ForeignKey("dbo.Forms", t => t.Form_FormID)
                .ForeignKey("dbo.FormSignatureFields", t => t.FormSignatureFieldID)
                .Index(t => t.FormSignatureFieldID)
                .Index(t => t.Form_FormID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FormSignatureValues", "FormSignatureFieldID", "dbo.FormSignatureFields");
            DropForeignKey("dbo.FormSignatureValues", "Form_FormID", "dbo.Forms");
            DropForeignKey("dbo.FormSignatureFields", "FormSettingsID", "dbo.FormSettings");
            DropForeignKey("dbo.Forms", "FormSettings_FormSettingsID", "dbo.FormSettings");
            DropForeignKey("dbo.FormCustomFields", "FormSettingsID", "dbo.FormSettings");
            DropForeignKey("dbo.FormCustomValues", "Form_FormID", "dbo.Forms");
            DropForeignKey("dbo.FormCustomValues", "FormCustomFieldID", "dbo.FormCustomFields");
            DropForeignKey("dbo.FormAttachments", "FormID", "dbo.Forms");
            DropForeignKey("dbo.LogMetadata", "AuditLogId", "dbo.AuditLogs");
            DropForeignKey("dbo.AuditLogDetails", "AuditLogId", "dbo.AuditLogs");
            DropIndex("dbo.FormSignatureValues", new[] { "Form_FormID" });
            DropIndex("dbo.FormSignatureValues", new[] { "FormSignatureFieldID" });
            DropIndex("dbo.FormSignatureFields", new[] { "FormSettingsID" });
            DropIndex("dbo.FormCustomFields", new[] { "FormSettingsID" });
            DropIndex("dbo.FormCustomValues", new[] { "Form_FormID" });
            DropIndex("dbo.FormCustomValues", new[] { "FormCustomFieldID" });
            DropIndex("dbo.Forms", new[] { "FormSettings_FormSettingsID" });
            DropIndex("dbo.FormAttachments", new[] { "FormID" });
            DropIndex("dbo.LogMetadata", new[] { "AuditLogId" });
            DropIndex("dbo.AuditLogDetails", new[] { "AuditLogId" });
            DropTable("dbo.FormSignatureValues");
            DropTable("dbo.FormSignatureFields");
            DropTable("dbo.FormSettings");
            DropTable("dbo.FormCustomFields");
            DropTable("dbo.FormCustomValues");
            DropTable("dbo.Forms");
            DropTable("dbo.FormAttachments");
            DropTable("dbo.LogMetadata");
            DropTable("dbo.AuditLogDetails");
            DropTable("dbo.AuditLogs");
        }
    }
}
