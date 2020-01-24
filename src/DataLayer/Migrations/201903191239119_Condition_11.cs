namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reco3Condition", "ComponentId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Reco3Condition", "ComponentId", c => c.Int(nullable: false));
        }
    }
}
