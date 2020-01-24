namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conditions_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reco3Condition", "ConditionId", "dbo.Reco3Improvement");
            DropIndex("dbo.Reco3Condition", new[] { "ConditionId" });
            DropPrimaryKey("dbo.Reco3Condition");
            AddColumn("dbo.Reco3Condition", "Reco3Improvement_ImprovementId", c => c.Int(nullable: false));
            AlterColumn("dbo.Reco3Condition", "ConditionId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Reco3Condition", "ConditionId");
            CreateIndex("dbo.Reco3Condition", "Reco3Improvement_ImprovementId");
            AddForeignKey("dbo.Reco3Condition", "Reco3Improvement_ImprovementId", "dbo.Reco3Improvement", "ImprovementId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Condition", "Reco3Improvement_ImprovementId", "dbo.Reco3Improvement");
            DropIndex("dbo.Reco3Condition", new[] { "Reco3Improvement_ImprovementId" });
            DropPrimaryKey("dbo.Reco3Condition");
            AlterColumn("dbo.Reco3Condition", "ConditionId", c => c.Int(nullable: false));
            DropColumn("dbo.Reco3Condition", "Reco3Improvement_ImprovementId");
            AddPrimaryKey("dbo.Reco3Condition", "ConditionId");
            CreateIndex("dbo.Reco3Condition", "ConditionId");
            AddForeignKey("dbo.Reco3Condition", "ConditionId", "dbo.Reco3Improvement", "ImprovementId");
        }
    }
}
