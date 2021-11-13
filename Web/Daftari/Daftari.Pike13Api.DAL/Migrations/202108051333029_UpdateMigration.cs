namespace Daftari.Pike13Api.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventOccurrances",
                c => new
                    {
                        EventOccurrenceID = c.Long(nullable: false),
                        EventID = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ServiceID = c.Long(),
                        LocationID = c.Long(),
                        StartAt = c.DateTime(),
                        EndAt = c.DateTime(),
                        Timezone = c.String(),
                        AttendanceComplete = c.Boolean(),
                        State = c.String(),
                        Full = c.Boolean(),
                        CapacityRemaining = c.Int(),
                        StaffMembers = c.String(),
                        people = c.String(),
                        SubDomain = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        VisitsCount = c.Int(),
                    })
                .PrimaryKey(t => t.EventOccurrenceID);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        VisitID = c.Long(nullable: false),
                        PersonID = c.Long(),
                        EventOccurrenceID = c.Long(),
                        State = c.String(),
                        Status = c.String(),
                        RegisteredAt = c.DateTime(),
                        CompletedAt = c.DateTime(),
                        NoshowAt = c.DateTime(),
                        CancelledAt = c.DateTime(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        Paid = c.Boolean(),
                        PaidForBy = c.String(),
                        PunchID = c.Long(),
                        OnlyStaffCanCancel = c.Boolean(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VisitID)
                .ForeignKey("dbo.EventOccurrances", t => t.EventOccurrenceID)
                .Index(t => t.EventOccurrenceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visits", "EventOccurrenceID", "dbo.EventOccurrances");
            DropIndex("dbo.Visits", new[] { "EventOccurrenceID" });
            DropTable("dbo.Visits");
            DropTable("dbo.EventOccurrances");
        }
    }
}
