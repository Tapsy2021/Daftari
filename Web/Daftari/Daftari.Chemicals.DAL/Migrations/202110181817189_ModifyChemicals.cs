namespace Daftari.Chemicals.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyChemicals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChemicalSettings", "PoolDailyCapacity", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChemicalSettings", "PoolDailyCapacity");
        }
    }
}
