namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendedSimulationJob : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SimulationJob", "PackageName", c => c.String());
            AddColumn("dbo.SimulationJob", "PackageUploadedDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageUploaded", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageValidatedDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageValidated", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageConvertedDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageConverted", c => c.Boolean(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageSimulatedDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.SimulationJob", "PackageSimulated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SimulationJob", "PackageSimulated");
            DropColumn("dbo.SimulationJob", "PackageSimulatedDateTime");
            DropColumn("dbo.SimulationJob", "PackageConverted");
            DropColumn("dbo.SimulationJob", "PackageConvertedDateTime");
            DropColumn("dbo.SimulationJob", "PackageValidated");
            DropColumn("dbo.SimulationJob", "PackageValidatedDateTime");
            DropColumn("dbo.SimulationJob", "PackageUploaded");
            DropColumn("dbo.SimulationJob", "PackageUploadedDateTime");
            DropColumn("dbo.SimulationJob", "PackageName");
        }
    }
}
