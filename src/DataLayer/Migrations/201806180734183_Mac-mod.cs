namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Macmod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agent", "MAC", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Agent", "MAC");
        }
    }
}
