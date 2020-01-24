namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PublishedFinishedSimulationJob : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "Published", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimulationJob", "Finished", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "Finished");
            DropColumn("dbo.SimulationJob", "Published");
        }
    }
}
