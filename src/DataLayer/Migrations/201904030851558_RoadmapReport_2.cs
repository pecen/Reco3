namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoadmapReport_2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoadmapReports",
                c => new
                    {
                        RoadmapReportId = c.Int(nullable: false, identity: true),
                        RoadmapId = c.Int(nullable: false),
                        RoadmapGroupId = c.Int(nullable: false),
                        VehicleId = c.Int(nullable: false),
                        VIN = c.String(),
                        VehicleDate = c.DateTime(nullable: false),
                        EngineModel = c.String(),
                        GearboxModel = c.String(),
                        AxleModel = c.String(),
                        AirDragModel = c.String(),
                    })
                .PrimaryKey(t => t.RoadmapReportId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RoadmapReports");
        }
    }
}
