namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reco3Condition", "Reco3Improvement_ImprovementId", c => c.Int());
            CreateIndex("dbo.Reco3Condition", "Reco3Improvement_ImprovementId");
            AddForeignKey("dbo.Reco3Condition", "Reco3Improvement_ImprovementId", "dbo.Reco3Improvement", "ImprovementId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Condition", "Reco3Improvement_ImprovementId", "dbo.Reco3Improvement");
            DropIndex("dbo.Reco3Condition", new[] { "Reco3Improvement_ImprovementId" });
            DropColumn("dbo.Reco3Condition", "Reco3Improvement_ImprovementId");
        }
    }
}
