namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NodenameatAgetnt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agent", "NodeName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Agent", "NodeName");
        }
    }
}
