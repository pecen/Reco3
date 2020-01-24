namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedSimulationJob2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "PackageExtracted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "PackageExtracted");
        }
    }
}
