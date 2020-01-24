namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationFailureUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversionFailure", "Component", c => c.String());
            AddColumn("dbo.VehicleExcelConversionFailure", "ExcelCellData", c => c.String());
            DropColumn("dbo.VehicleExcelConversionFailure", "Cause");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VehicleExcelConversionFailure", "Cause", c => c.String());
            DropColumn("dbo.VehicleExcelConversionFailure", "ExcelCellData");
            DropColumn("dbo.VehicleExcelConversionFailure", "Component");
        }
    }
}
