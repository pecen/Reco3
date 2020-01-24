namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedSimulationJob21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversion", "ConversionPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleExcelConversion", "ConversionPath");
        }
    }
}
