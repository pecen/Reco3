namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conditions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reco3Condition",
                c => new
                    {
                        ConditionId = c.Int(nullable: false),
                        ValidFrom = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ConditionId)
                .ForeignKey("dbo.Reco3Improvement", t => t.ConditionId)
                .Index(t => t.ConditionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Condition", "ConditionId", "dbo.Reco3Improvement");
            DropIndex("dbo.Reco3Condition", new[] { "ConditionId" });
            DropTable("dbo.Reco3Condition");
        }
    }
}
