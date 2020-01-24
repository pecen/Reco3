namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_18 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reco3Improvement", "Reco3TagId", "dbo.Reco3Tag");
            DropIndex("dbo.Reco3Improvement", new[] { "Reco3TagId" });
            AddColumn("dbo.Reco3Condition", "Condition_Type", c => c.Int(nullable: false));
            AddColumn("dbo.Reco3Condition", "Reco3TagId", c => c.Int());
            AddColumn("dbo.Reco3Condition", "Reco3TagValue", c => c.String());
            CreateIndex("dbo.Reco3Condition", "Reco3TagId");
            AddForeignKey("dbo.Reco3Condition", "Reco3TagId", "dbo.Reco3Tag", "Reco3TagId");
            DropColumn("dbo.Reco3Improvement", "Reco3TagId");
            DropColumn("dbo.Reco3Improvement", "Reco3TagValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reco3Improvement", "Reco3TagValue", c => c.String());
            AddColumn("dbo.Reco3Improvement", "Reco3TagId", c => c.Int());
            DropForeignKey("dbo.Reco3Condition", "Reco3TagId", "dbo.Reco3Tag");
            DropIndex("dbo.Reco3Condition", new[] { "Reco3TagId" });
            DropColumn("dbo.Reco3Condition", "Reco3TagValue");
            DropColumn("dbo.Reco3Condition", "Reco3TagId");
            DropColumn("dbo.Reco3Condition", "Condition_Type");
            CreateIndex("dbo.Reco3Improvement", "Reco3TagId");
            AddForeignKey("dbo.Reco3Improvement", "Reco3TagId", "dbo.Reco3Tag", "Reco3TagId");
        }
    }
}
