namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Index_for_VehicleId_OnVehicle : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Vehicle", "VehicleId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Vehicle", new[] { "VehicleId" });
        }
    }
}
