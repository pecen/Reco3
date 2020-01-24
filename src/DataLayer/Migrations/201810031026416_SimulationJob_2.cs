namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimulationJob_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "SimulationCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "SimulationCount");
        }
    }
}
