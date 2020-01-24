namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reco3Improvement_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reco3Improvement", "ComponentId", "dbo.Reco3Component");
            DropIndex("dbo.Reco3Improvement", new[] { "ComponentId" });
            AlterColumn("dbo.Reco3Improvement", "ComponentId", c => c.Int());
            CreateIndex("dbo.Reco3Improvement", "ComponentId");
            AddForeignKey("dbo.Reco3Improvement", "ComponentId", "dbo.Reco3Component", "ComponentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Improvement", "ComponentId", "dbo.Reco3Component");
            DropIndex("dbo.Reco3Improvement", new[] { "ComponentId" });
            AlterColumn("dbo.Reco3Improvement", "ComponentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reco3Improvement", "ComponentId");
            AddForeignKey("dbo.Reco3Improvement", "ComponentId", "dbo.Reco3Component", "ComponentId", cascadeDelete: true);
        }
    }
}
