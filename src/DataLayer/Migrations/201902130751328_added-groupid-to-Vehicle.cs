namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedgroupidtoVehicle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicle", "GroupId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicle", "GroupId");
        }
    }
}
