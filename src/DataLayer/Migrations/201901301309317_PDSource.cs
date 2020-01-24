namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PDSource : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reco3Component", "PD_Source", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reco3Component", "PD_Source");
        }
    }
}
