namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reco3Condition", "ComponentId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reco3Condition", "ComponentId");
        }
    }
}
