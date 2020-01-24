namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime_and_vehicleid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VSumRecord", "SimulationTimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.VSumRecord", "VehicleId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VSumRecord", "VehicleId");
            DropColumn("dbo.VSumRecord", "SimulationTimeStamp");
        }
    }
}
