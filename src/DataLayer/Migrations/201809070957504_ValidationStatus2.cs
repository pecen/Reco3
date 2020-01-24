namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationStatus2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "Validation_Status", c => c.Int(nullable: false));
            DropColumn("dbo.SimulationJob", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SimulationJob", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.SimulationJob", "Validation_Status");
        }
    }
}
