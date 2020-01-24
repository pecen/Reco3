namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimulationMode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "Simulation_Mode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "Simulation_Mode");
        }
    }
}
