namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Condition_13 : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("dbo.Reco3Condition");
            AddColumn("dbo.Reco3Condition", "Reco3ConditionId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Reco3Condition", "Reco3ConditionId");
            //DropColumn("dbo.Reco3Condition", "ConditionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reco3Condition", "ConditionId", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Reco3Condition");
            DropColumn("dbo.Reco3Condition", "Reco3ConditionId");
            AddPrimaryKey("dbo.Reco3Condition", "ConditionId");
        }
    }
}
