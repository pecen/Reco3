namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleConversionFailure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VehicleExcelConversionFailure",
                c => new
                    {
                        VehicleExcelConversionFailureId = c.Int(nullable: false, identity: true),
                        VehicleExcelConversionId = c.Int(nullable: false),
                        VIN = c.String(),
                        Cause = c.String(),
                    })
                .PrimaryKey(t => t.VehicleExcelConversionFailureId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VehicleExcelConversionFailure");
        }
    }
}
