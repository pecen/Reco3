namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CloneAComponent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reco3Component", "SourceComponentId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reco3Component", "SourceComponentId");
        }
    }
}
