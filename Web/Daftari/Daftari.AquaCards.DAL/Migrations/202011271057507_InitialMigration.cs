namespace Daftari.AquaCards.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
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
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Guid(nullable: false, identity: true),
                        Address = c.String(),
                        AlternatePhone = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        CellPhone = c.String(),
                        City = c.String(),
                        CustomerStatus = c.Int(nullable: false),
                        EmailAddress = c.String(),
                        FullName = c.String(),
                        FatherName = c.String(),
                        IsSubToCommunications = c.Boolean(nullable: false),
                        MotherName = c.String(),
                        PrimaryPhone = c.String(),
                        Reference_Sequence = c.Long(nullable: false),
                        Referral = c.String(),
                        Region = c.String(),
                        IsMember = c.Boolean(nullable: false),
                        Birthday = c.DateTime(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        GuardianName = c.String(),
                        ExternalReference = c.Long(nullable: false),
                        SubDomain = c.String(),
                        PhotoMD = c.String(),
                        PhotoLG = c.String(),
                        Dependants = c.String(),
                        Providers = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        SkillID = c.Long(nullable: false, identity: true),
                        SetName = c.String(),
                        Name = c.String(),
                        SkillLevel = c.Int(nullable: false),
                        SkillDifficulty = c.Int(nullable: false),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SkillID);
            
            CreateTable(
                "dbo.StudentCardDetails",
                c => new
                    {
                        StudentCardDetailID = c.Long(nullable: false, identity: true),
                        SkillID = c.Long(nullable: false),
                        IsComplete = c.Boolean(nullable: false),
                        CompleteDate = c.DateTime(),
                        CompletedBy = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        StudentCard_StudentCardID = c.Long(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StudentCardDetailID)
                .ForeignKey("dbo.Skills", t => t.SkillID, cascadeDelete: true)
                .ForeignKey("dbo.StudentCards", t => t.StudentCard_StudentCardID)
                .Index(t => t.SkillID)
                .Index(t => t.StudentCard_StudentCardID);
            
            CreateTable(
                "dbo.StudentCards",
                c => new
                    {
                        StudentCardID = c.Long(nullable: false, identity: true),
                        Initial = c.String(),
                        LastName = c.String(),
                        StudentName = c.String(),
                        BirthDate = c.DateTime(),
                        Level = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Plan = c.String(),
                        Instructors = c.String(),
                        IsManual = c.Boolean(nullable: false),
                        IsGraduated = c.Boolean(nullable: false),
                        GraduatedBy = c.String(),
                        GraduationDate = c.DateTime(),
                        CustomerID = c.Guid(nullable: false),
                        ExternalReferenceID = c.Long(nullable: false),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StudentCardID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentCardDetails", "StudentCard_StudentCardID", "dbo.StudentCards");
            DropForeignKey("dbo.StudentCards", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.StudentCardDetails", "SkillID", "dbo.Skills");
            DropForeignKey("dbo.LogMetadata", "AuditLogId", "dbo.AuditLogs");
            DropForeignKey("dbo.AuditLogDetails", "AuditLogId", "dbo.AuditLogs");
            DropIndex("dbo.StudentCards", new[] { "CustomerID" });
            DropIndex("dbo.StudentCardDetails", new[] { "StudentCard_StudentCardID" });
            DropIndex("dbo.StudentCardDetails", new[] { "SkillID" });
            DropIndex("dbo.LogMetadata", new[] { "AuditLogId" });
            DropIndex("dbo.AuditLogDetails", new[] { "AuditLogId" });
            DropTable("dbo.StudentCards");
            DropTable("dbo.StudentCardDetails");
            DropTable("dbo.Skills");
            DropTable("dbo.Customers");
            DropTable("dbo.LogMetadata");
            DropTable("dbo.AuditLogDetails");
            DropTable("dbo.AuditLogs");
        }
    }
}
