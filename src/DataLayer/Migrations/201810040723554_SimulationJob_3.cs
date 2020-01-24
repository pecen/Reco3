namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimulationJob_3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "ConvertToVehicleInput_Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "ConvertToVehicleInput_Status");
        }
    }
}
