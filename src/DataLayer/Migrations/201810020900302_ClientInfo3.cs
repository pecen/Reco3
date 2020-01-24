namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientInfo3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientInfo", "RamAvailable", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClientInfo", "RamAvailable");
        }
    }
}
