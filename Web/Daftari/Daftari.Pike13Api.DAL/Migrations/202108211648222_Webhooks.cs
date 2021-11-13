namespace Daftari.Pike13Api.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Webhooks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TopicSubsriptions",
                c => new
                    {
                        TopicSubsriptionID = c.Long(nullable: false, identity: true),
                        Topic = c.String(),
                        TopicID = c.Long(),
                        Subdomain = c.String(),
                        BusinessID = c.Long(),
                        Link = c.String(),
                        AuditDetail_CreatedDate = c.DateTime(nullable: false),
                        AuditDetail_CreatedEntryUserID = c.String(),
                        AuditDetail_LastModifiedDate = c.DateTime(),
                        AuditDetail_LastModifiedEntryUserID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TopicSubsriptionID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TopicSubsriptions");
        }
    }
}
