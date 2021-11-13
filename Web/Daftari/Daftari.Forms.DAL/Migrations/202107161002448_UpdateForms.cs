namespace Daftari.Forms.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateForms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovalProcesses",
                c => new
                    {
                        FormID = c.Long(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Status = c.Int(nullable: false),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.FormID, t.UserName })
                .ForeignKey("dbo.Forms", t => t.FormID, cascadeDelete: true)
                .Index(t => t.FormID);
            
            AddColumn("dbo.FormSettings", "ApprovalProcess", c => c.String());
            AddColumn("dbo.FormSettings", "IsPublic", c => c.Boolean(nullable: false));
            AlterColumn("dbo.FormSettings", "AccessLevel", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApprovalProcesses", "FormID", "dbo.Forms");
            DropIndex("dbo.ApprovalProcesses", new[] { "FormID" });
            AlterColumn("dbo.FormSettings", "AccessLevel", c => c.Int(nullable: false));
            DropColumn("dbo.FormSettings", "IsPublic");
            DropColumn("dbo.FormSettings", "ApprovalProcess");
            DropTable("dbo.ApprovalProcesses");
        }
    }
}
