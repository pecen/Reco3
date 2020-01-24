namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Index_for_VehicleId_OnVSum : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.VSumRecord", "VehicleId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VSumRecord", new[] { "VehicleId" });
        }
    }
}
