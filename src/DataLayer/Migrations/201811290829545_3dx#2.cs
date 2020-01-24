namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3dx2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reco3Component",
                c => new
                    {
                        ComponentId = c.Int(nullable: false, identity: true),
                        PDNumber = c.String(),
                        DownloadedTimestamp = c.DateTime(nullable: false),
                        Description = c.String(),
                        PD_Status = c.Int(nullable: false),
                        Component_Type = c.Int(nullable: false),
                        XML = c.String(),
                    })
                .PrimaryKey(t => t.ComponentId);
            
            AddColumn("dbo.vehicle", "Vehicle_Mode", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.vehicle", "Vehicle_Mode");
            DropTable("dbo.Reco3Component");
        }
    }
}
