namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidationFailureUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversionFailure", "SimulationJobId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleExcelConversionFailure", "SimulationJobId");
        }
    }
}
