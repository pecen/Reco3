namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleExcelConversionWithFilenameAndStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversion", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleExcelConversion", "Status");
        }
    }
}
