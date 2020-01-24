namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversion", "VehicleCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleExcelConversion", "VehicleCount");
        }
    }
}
