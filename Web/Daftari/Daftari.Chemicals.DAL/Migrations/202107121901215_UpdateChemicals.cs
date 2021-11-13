namespace Daftari.Chemicals.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChemicals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChemicalRecords", "UnitOfMeasurement", c => c.String());
            AddColumn("dbo.ChemicalSettings", "IncreasepH", c => c.Int(nullable: false));
            AddColumn("dbo.ChemicalSettings", "DecreasepH", c => c.Int(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "FreeChlorine", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "TotalChlorine", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "TotalBromine", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "pH", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "PoolTemp", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "AirTemp", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "WaterClarity", c => c.Int());
            AlterColumn("dbo.ChemicalRecords", "Alkalinity", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "CalciumHardness", c => c.Double());
            AlterColumn("dbo.ChemicalRecords", "Backwash", c => c.Int());
            AlterColumn("dbo.ChemicalRecords", "HRR_ORP", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChemicalRecords", "HRR_ORP", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "Backwash", c => c.Int(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "CalciumHardness", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "Alkalinity", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "WaterClarity", c => c.Int(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "AirTemp", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "PoolTemp", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "pH", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "TotalBromine", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "TotalChlorine", c => c.Double(nullable: false));
            AlterColumn("dbo.ChemicalRecords", "FreeChlorine", c => c.Double(nullable: false));
            DropColumn("dbo.ChemicalSettings", "DecreasepH");
            DropColumn("dbo.ChemicalSettings", "IncreasepH");
            DropColumn("dbo.ChemicalRecords", "UnitOfMeasurement");
        }
    }
}
