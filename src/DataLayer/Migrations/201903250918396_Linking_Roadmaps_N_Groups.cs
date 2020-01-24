namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Linking_Roadmaps_N_Groups : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Roadmaps", "RoadmapGroupId");
            AddForeignKey("dbo.Roadmaps", "RoadmapGroupId", "dbo.RoadmapGroups", "RoadmapGroupId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Roadmaps", "RoadmapGroupId", "dbo.RoadmapGroups");
            DropIndex("dbo.Roadmaps", new[] { "RoadmapGroupId" });
        }
    }
}
