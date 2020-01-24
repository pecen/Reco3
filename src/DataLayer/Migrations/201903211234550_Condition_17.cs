namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_17 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reco3IntroductionPoint",
                c => new
                    {
                        Reco3IntroductionPointId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IntroductionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Reco3IntroductionPointId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reco3IntroductionPoint");
        }
    }
}
