namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_15 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reco3Tag",
                c => new
                    {
                        Reco3TagId = c.Int(nullable: false, identity: true),
                        Reco3TagName = c.String(),
                    })
                .PrimaryKey(t => t.Reco3TagId);
            
            AddColumn("dbo.Reco3Improvement", "Reco3TagId", c => c.Int());
            CreateIndex("dbo.Reco3Improvement", "Reco3TagId");
            AddForeignKey("dbo.Reco3Improvement", "Reco3TagId", "dbo.Reco3Tag", "Reco3TagId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Improvement", "Reco3TagId", "dbo.Reco3Tag");
            DropIndex("dbo.Reco3Improvement", new[] { "Reco3TagId" });
            DropColumn("dbo.Reco3Improvement", "Reco3TagId");
            DropTable("dbo.Reco3Tag");
        }
    }
}
