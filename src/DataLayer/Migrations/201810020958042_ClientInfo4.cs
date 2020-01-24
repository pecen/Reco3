namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientInfo4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientInfo", "Sleeping", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientInfo", "Sleeping");
        }
    }
}
