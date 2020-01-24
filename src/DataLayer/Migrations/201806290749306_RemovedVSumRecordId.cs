namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedVSumRecordId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Simulation", "VSumRecordId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Simulation", "VSumRecordId", c => c.Int());
        }
    }
}
