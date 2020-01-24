namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoadmapGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoadmapGroups",
                c => new
                    {
                        RoadmapGroupId = c.Int(nullable: false, identity: true),
                        OwnerSss = c.String(),
                        RoadmapName = c.String(),
                        Protected = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        StartYear = c.Int(nullable: false),
                        EndYear = c.Int(nullable: false),
                        XML = c.String(),
                        Validation_Status = c.Int(nullable: false),
                        ConvertToVehicleInput_Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RoadmapGroupId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RoadmapGroups");
        }
    }
}
