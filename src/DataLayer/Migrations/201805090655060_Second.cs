namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "SimulationJobName", c => c.String());
            AddColumn("dbo.SimulationJob", "CreatedDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "CreatedDateTime");
            DropColumn("dbo.SimulationJob", "SimulationJobName");
        }
    }
}
