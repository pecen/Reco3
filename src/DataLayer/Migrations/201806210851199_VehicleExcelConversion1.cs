namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VehicleExcelConversion1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VehicleExcelConversion",
                c => new
                    {
                        VehicleExcelConversionId = c.Int(nullable: false, identity: true),
                        SimulationJobId = c.Int(nullable: false),
                        OwnerSss = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.VehicleExcelConversionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VehicleExcelConversion");
        }
    }
}
