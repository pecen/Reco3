namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removal_of_datetime2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SimulationJob", "CreatedDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SimulationJob", "CreatedDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
