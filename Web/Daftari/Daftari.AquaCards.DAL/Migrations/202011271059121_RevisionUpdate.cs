namespace Daftari.AquaCards.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RevisionUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudentCards", "IsNew", c => c.Boolean(nullable: false));
            AddColumn("dbo.StudentCards", "PreviousStudentCard_StudentCardID", c => c.Long());
            CreateIndex("dbo.StudentCards", "PreviousStudentCard_StudentCardID");
            AddForeignKey("dbo.StudentCards", "PreviousStudentCard_StudentCardID", "dbo.StudentCards", "StudentCardID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentCards", "PreviousStudentCard_StudentCardID", "dbo.StudentCards");
            DropIndex("dbo.StudentCards", new[] { "PreviousStudentCard_StudentCardID" });
            DropColumn("dbo.StudentCards", "PreviousStudentCard_StudentCardID");
            DropColumn("dbo.StudentCards", "IsNew");
        }
    }
}
