namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aaa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleExcelConversion", "ComponentsPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicleExcelConversion", "ComponentsPath");
        }
    }
}
