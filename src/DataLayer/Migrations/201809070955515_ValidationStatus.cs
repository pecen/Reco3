namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "Status");
        }
    }
}
