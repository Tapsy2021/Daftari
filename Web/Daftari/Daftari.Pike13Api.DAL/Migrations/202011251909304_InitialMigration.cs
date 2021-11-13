namespace Daftari.Pike13Api.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountPeoples",
                c => new
                    {
                        AccountPeopleID = c.Long(nullable: false, identity: true),
                        PersonID = c.Long(nullable: false),
                        Photo = c.String(),
                        BusinessID = c.Long(nullable: false),
                        BusinessName = c.String(),
                        Subdomain = c.String(),
                        Role = c.String(),
                        IsClient = c.String(),
                        TimeZone = c.String(),
                        TokenID = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        PersonName = c.String(),
                    })
                .PrimaryKey(t => t.AccountPeopleID)
                .ForeignKey("dbo.Tokens", t => t.TokenID, cascadeDelete: true)
                .Index(t => t.TokenID);
            
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        TokenID = c.Long(nullable: false, identity: true),
                        Username = c.String(),
                        AccessToken = c.String(),
                        RefreshTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TokenID);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 30),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountPeoples", "TokenID", "dbo.Tokens");
            DropIndex("dbo.AccountPeoples", new[] { "TokenID" });
            DropTable("dbo.Settings");
            DropTable("dbo.Tokens");
            DropTable("dbo.AccountPeoples");
        }
    }
}
