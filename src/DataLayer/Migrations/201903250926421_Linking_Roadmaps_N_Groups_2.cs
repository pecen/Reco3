namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Linking_Roadmaps_N_Groups_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roadmaps", "CurrentYear", c => c.Int(nullable: false));
            DropColumn("dbo.Roadmaps", "OwnerSss");
            DropColumn("dbo.Roadmaps", "CreationTime");
            DropColumn("dbo.Roadmaps", "StartYear");
            DropColumn("dbo.Roadmaps", "EndYear");
            DropColumn("dbo.Roadmaps", "XML");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roadmaps", "XML", c => c.String());
            AddColumn("dbo.Roadmaps", "EndYear", c => c.Int(nullable: false));
            AddColumn("dbo.Roadmaps", "StartYear", c => c.Int(nullable: false));
            AddColumn("dbo.Roadmaps", "CreationTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Roadmaps", "OwnerSss", c => c.String());
            DropColumn("dbo.Roadmaps", "CurrentYear");
        }
    }
}
