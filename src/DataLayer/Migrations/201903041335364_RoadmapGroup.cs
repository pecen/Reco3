namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoadmapGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roadmaps", "RoadmapGroupId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roadmaps", "RoadmapGroupId");
        }
    }
}
