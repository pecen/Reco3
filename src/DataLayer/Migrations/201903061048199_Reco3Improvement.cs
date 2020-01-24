namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reco3Improvement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reco3Improvement",
                c => new
                    {
                        ImprovementId = c.Int(nullable: false, identity: true),
                        ComponentId = c.Int(nullable: false),
                        ValidFrom = c.DateTime(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ImprovementId)
                .ForeignKey("dbo.Reco3Component", t => t.ComponentId, cascadeDelete: true)
                .Index(t => t.ComponentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reco3Improvement", "ComponentId", "dbo.Reco3Component");
            DropIndex("dbo.Reco3Improvement", new[] { "ComponentId" });
            DropTable("dbo.Reco3Improvement");
        }
    }
}
