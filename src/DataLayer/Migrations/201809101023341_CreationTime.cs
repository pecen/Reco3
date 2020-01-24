namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreationTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "CreationTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "CreationTime");
        }
    }
}
