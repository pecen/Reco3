namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGuid : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Agent", "AgentGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Agent", "AgentGuid", c => c.Guid(nullable: false));
        }
    }
}
