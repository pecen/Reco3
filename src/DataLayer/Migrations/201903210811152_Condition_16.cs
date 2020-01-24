namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reco3Improvement", "Reco3TagValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reco3Improvement", "Reco3TagValue");
        }
    }
}
