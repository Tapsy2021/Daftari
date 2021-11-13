namespace Daftari.Chemicals.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitChemicals : DbMigration
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
                "dbo.ChemicalCustomFields",
                c => new
                    {
                        ChemicalCustomFieldID = c.Long(nullable: false, identity: true),
                        ChemicalRecordSettingsID = c.Guid(),
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
                .PrimaryKey(t => t.ChemicalCustomFieldID)
                .ForeignKey("dbo.ChemicalSettings", t => t.ChemicalRecordSettingsID)
                .Index(t => t.ChemicalRecordSettingsID);
            
            CreateTable(
                "dbo.ChemicalCustomValues",
                c => new
                    {
                        ChemicalCustomValueID = c.Long(nullable: false, identity: true),
                        ChemicalRecordID = c.Long(),
                        ChemicalCustomFieldID = c.Long(),
                        CustomValue = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ChemicalCustomValueID)
                .ForeignKey("dbo.ChemicalCustomFields", t => t.ChemicalCustomFieldID)
                .ForeignKey("dbo.ChemicalRecords", t => t.ChemicalRecordID)
                .Index(t => t.ChemicalRecordID)
                .Index(t => t.ChemicalCustomFieldID);
            
            CreateTable(
                "dbo.ChemicalRecordComments",
                c => new
                    {
                        ChemicalRecordCommentID = c.Long(nullable: false, identity: true),
                        ChemicalRecordID = c.Long(),
                        Comment = c.String(),
                        Date = c.DateTime(nullable: false),
                        SubmittedBy = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                    })
                .PrimaryKey(t => t.ChemicalRecordCommentID)
                .ForeignKey("dbo.ChemicalRecords", t => t.ChemicalRecordID)
                .Index(t => t.ChemicalRecordID);
            
            CreateTable(
                "dbo.ChemicalRecords",
                c => new
                    {
                        ChemicalRecordID = c.Long(nullable: false, identity: true),
                        FreeChlorine = c.Double(nullable: false),
                        TotalChlorine = c.Double(nullable: false),
                        TotalBromine = c.Double(nullable: false),
                        pH = c.Double(nullable: false),
                        PoolTemp = c.Double(nullable: false),
                        AirTemp = c.Double(nullable: false),
                        WaterClarity = c.Int(nullable: false),
                        Alkalinity = c.Double(nullable: false),
                        CalciumHardness = c.Double(nullable: false),
                        Backwash = c.Int(nullable: false),
                        HRR_ORP = c.Double(nullable: false),
                        Notes = c.String(),
                        Employee = c.String(),
                        Date = c.DateTime(nullable: false),
                        SubmittedBy = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        ChemicalSettings_ChemicalRecordSettingsID = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ChemicalRecordID)
                .ForeignKey("dbo.ChemicalSettings", t => t.ChemicalSettings_ChemicalRecordSettingsID)
                .Index(t => t.ChemicalSettings_ChemicalRecordSettingsID);
            
            CreateTable(
                "dbo.ChemicalSettings",
                c => new
                    {
                        ChemicalRecordSettingsID = c.Guid(nullable: false, identity: true),
                        Volume = c.Double(nullable: false),
                        IncreaseChlorine = c.Int(nullable: false),
                        IncreaseAlkalinity = c.Int(nullable: false),
                        DecreaseAlkalinity = c.Int(nullable: false),
                        IncreaseCalciumHardness = c.Int(nullable: false),
                        IncreaseStabilizer = c.Int(nullable: false),
                        NeutralizeChlorine = c.Int(nullable: false),
                        FreeChlorine = c.Int(nullable: false),
                        FreeChlorineLowAlert = c.Double(nullable: false),
                        FreeChlorineHighAlert = c.Double(nullable: false),
                        TotalChlorine = c.Int(nullable: false),
                        TotalChlorineLowAlert = c.Double(nullable: false),
                        TotalChlorineHighAlert = c.Double(nullable: false),
                        TotalBromine = c.Int(nullable: false),
                        TotalBromineLowAlert = c.Double(nullable: false),
                        TotalBromineHighAlert = c.Double(nullable: false),
                        pH = c.Int(nullable: false),
                        pHLowAlert = c.Double(nullable: false),
                        pHHighAlert = c.Double(nullable: false),
                        PoolTemp = c.Int(nullable: false),
                        IdealPoolTemp = c.Double(nullable: false),
                        PoolTempLowAlert = c.Double(nullable: false),
                        PoolTempHighAlert = c.Double(nullable: false),
                        AirTemp = c.Int(nullable: false),
                        WaterClarity = c.Int(nullable: false),
                        Alkalinity = c.Int(nullable: false),
                        AlkalinityLowAlert = c.Double(nullable: false),
                        AlkalinityHighAlert = c.Double(nullable: false),
                        CalciumHardness = c.Int(nullable: false),
                        CalciumHardnessLowAlert = c.Double(nullable: false),
                        CalciumHardnessHighAlert = c.Double(nullable: false),
                        HRR_ORP = c.Int(nullable: false),
                        Backwash = c.Int(nullable: false),
                        Notes = c.Int(nullable: false),
                        SubDomain = c.String(),
                        UnitOfMeasurement = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ChemicalRecordSettingsID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChemicalRecords", "ChemicalSettings_ChemicalRecordSettingsID", "dbo.ChemicalSettings");
            DropForeignKey("dbo.ChemicalCustomFields", "ChemicalRecordSettingsID", "dbo.ChemicalSettings");
            DropForeignKey("dbo.ChemicalRecordComments", "ChemicalRecordID", "dbo.ChemicalRecords");
            DropForeignKey("dbo.ChemicalCustomValues", "ChemicalRecordID", "dbo.ChemicalRecords");
            DropForeignKey("dbo.ChemicalCustomValues", "ChemicalCustomFieldID", "dbo.ChemicalCustomFields");
            DropForeignKey("dbo.LogMetadata", "AuditLogId", "dbo.AuditLogs");
            DropForeignKey("dbo.AuditLogDetails", "AuditLogId", "dbo.AuditLogs");
            DropIndex("dbo.ChemicalRecords", new[] { "ChemicalSettings_ChemicalRecordSettingsID" });
            DropIndex("dbo.ChemicalRecordComments", new[] { "ChemicalRecordID" });
            DropIndex("dbo.ChemicalCustomValues", new[] { "ChemicalCustomFieldID" });
            DropIndex("dbo.ChemicalCustomValues", new[] { "ChemicalRecordID" });
            DropIndex("dbo.ChemicalCustomFields", new[] { "ChemicalRecordSettingsID" });
            DropIndex("dbo.LogMetadata", new[] { "AuditLogId" });
            DropIndex("dbo.AuditLogDetails", new[] { "AuditLogId" });
            DropTable("dbo.ChemicalSettings");
            DropTable("dbo.ChemicalRecords");
            DropTable("dbo.ChemicalRecordComments");
            DropTable("dbo.ChemicalCustomValues");
            DropTable("dbo.ChemicalCustomFields");
            DropTable("dbo.LogMetadata");
            DropTable("dbo.AuditLogDetails");
            DropTable("dbo.AuditLogs");
        }
    }
}
