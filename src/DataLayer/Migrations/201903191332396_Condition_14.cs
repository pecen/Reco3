namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_14 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Reco3Condition", "ComponentId");
            AddForeignKey("dbo.Reco3Condition", "ComponentId", "dbo.Reco3Component", "ComponentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Condition", "ComponentId", "dbo.Reco3Component");
            DropIndex("dbo.Reco3Condition", new[] { "ComponentId" });
        }
    }
}
