namespace Daftari.Forms.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix_This : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FormCustomFieldDefinitions", "ID", "dbo.FormCustomFields");
            DropIndex("dbo.FormCustomFieldDefinitions", new[] { "ID" });
            DropColumn("dbo.FormSettings", "FormType");
            DropTable("dbo.FormCustomFieldDefinitions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FormCustomFieldDefinitions",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        FormCustomFieldID = c.Long(),
                        Alignment = c.Int(),
                        FontSize = c.Int(),
                        XAxis = c.Single(),
                        YAxis = c.Single(),
                        Rotation = c.Single(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.FormSettings", "FormType", c => c.Int(nullable: false));
            CreateIndex("dbo.FormCustomFieldDefinitions", "ID");
            AddForeignKey("dbo.FormCustomFieldDefinitions", "ID", "dbo.FormCustomFields", "FormCustomFieldID");
        }
    }
}
