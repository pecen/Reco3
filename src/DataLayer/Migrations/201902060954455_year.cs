namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class year : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roadmaps",
                c => new
                    {
                        RoadmapId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.RoadmapId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Roadmaps");
        }
    }
}
