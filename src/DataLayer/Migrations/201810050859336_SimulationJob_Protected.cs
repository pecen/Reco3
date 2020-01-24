namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimulationJob_Protected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "Protected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "Protected");
        }
    }
}
