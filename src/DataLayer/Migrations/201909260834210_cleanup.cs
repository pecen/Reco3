namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cleanup : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.SimulationJob");
            DropTable("dbo.Simulation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Simulation",
                c => new
                    {
                        SimulationId = c.Int(nullable: false, identity: true),
                        SimulationJobId = c.Int(nullable: false),
                        AgentId = c.Int(),
                        VehicleId = c.Int(nullable: false),
                        Finished = c.Boolean(nullable: false),
                        Processing = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SimulationId);
            
            CreateTable(
                "dbo.SimulationJob",
                c => new
                    {
                        SimulationJobId = c.Int(nullable: false, identity: true),
                        OwnerSss = c.String(),
                        SimulationJobName = c.String(),
                        Protected = c.Boolean(nullable: false),
                        SimulationCount = c.Int(nullable: false),
                        Simulation_Mode = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Finished = c.Boolean(nullable: false),
                        PackageName = c.String(),
                        PackageUploadedDateTime = c.DateTime(nullable: false),
                        PackageUploaded = c.Boolean(nullable: false),
                        PackageExtracted = c.Boolean(nullable: false),
                        PackageValidatedDateTime = c.DateTime(nullable: false),
                        PackageValidated = c.Boolean(nullable: false),
                        Validation_Status = c.Int(nullable: false),
                        PackageConvertedDateTime = c.DateTime(nullable: false),
                        PackageConverted = c.Boolean(nullable: false),
                        ConvertToVehicleInput_Status = c.Int(nullable: false),
                        PackageSimulatedDateTime = c.DateTime(nullable: false),
                        PackageSimulated = c.Boolean(nullable: false),
                        Simulation_Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SimulationJobId);
            
        }
    }
}
