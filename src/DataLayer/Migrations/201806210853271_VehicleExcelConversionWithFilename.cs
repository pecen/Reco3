namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleExcelConversionWithFilename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversion", "LocalFilename", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleExcelConversion", "LocalFilename");
        }
    }
}
