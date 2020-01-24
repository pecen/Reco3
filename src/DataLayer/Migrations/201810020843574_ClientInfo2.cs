namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientInfo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientInfo", "ClientVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientInfo", "ClientVersion");
        }
    }
}
