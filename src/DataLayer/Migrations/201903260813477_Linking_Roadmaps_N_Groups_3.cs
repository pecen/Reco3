namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Linking_Roadmaps_N_Groups_3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roadmaps", "ImprovedVehicleCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roadmaps", "ImprovedVehicleCount");
        }
    }
}
