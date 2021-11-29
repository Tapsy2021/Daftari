namespace Daftari.Pike13Api.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnpaidField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visits", "Unpaid", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visits", "Unpaid");
        }
    }
}
