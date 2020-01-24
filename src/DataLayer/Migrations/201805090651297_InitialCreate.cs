namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agent",
                c => new
                    {
                        AgentId = c.Int(nullable: false, identity: true),
                        AgentGuid = c.Guid(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.AgentId);
            
            CreateTable(
                "dbo.Simulation",
                c => new
                    {
                        SimulationId = c.Int(nullable: false, identity: true),
                        SimulationJobId = c.Int(nullable: false),
                        AgentId = c.Int(),
                        VehicleId = c.Int(nullable: false),
                        VSumRecordId = c.Int(),
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
                    })
                .PrimaryKey(t => t.SimulationJobId);
            
            CreateTable(
                "dbo.vehicle",
                c => new
                    {
                        VehicleId = c.Int(nullable: false, identity: true),
                        VIN = c.String(),
                        XML = c.String(),
                    })
                .PrimaryKey(t => t.VehicleId);
            
            CreateTable(
                "dbo.VSumRecord",
                c => new
                    {
                        VSumRecordId = c.Int(nullable: false, identity: true),
                        SimulationId = c.Int(nullable: false),
                        Job = c.String(),
                        InputFile = c.String(),
                        Cycle = c.String(),
                        Status = c.String(),
                        vehicle = c.String(),
                        VIN = c.String(),
                        VehicleModel = c.String(),
                        HDVCO2VehicleClass = c.String(),
                        CorrectedActualCurbMass = c.String(),
                        Loading = c.String(),
                        TotalVehicleMass = c.String(),
                        EngineManufacturer = c.String(),
                        EngineModel = c.String(),
                        EngineFuelType = c.String(),
                        EngineRatedPower = c.String(),
                        EngineIdlingSpeed = c.String(),
                        EngineRatedSpeed = c.String(),
                        EngineDisplacement = c.String(),
                        EngineWHTCUrban = c.String(),
                        EngineWHTCRural = c.String(),
                        EngineWHTCMotorway = c.String(),
                        EngineBFColdHot = c.String(),
                        EngineCFRegPer = c.String(),
                        EngineActualCF = c.String(),
                        DeclaredCdxA = c.String(),
                        CdxA = c.String(),
                        TotalRRC = c.String(),
                        WeightedRRCwoTrailer = c.String(),
                        r_dyn = c.String(),
                        NumberAxlesVehicleDriven = c.String(),
                        NumberAxlesVehicleNonDriven = c.String(),
                        NumberAxlesVehicleTrailer = c.String(),
                        GearboxManufacturer = c.String(),
                        GearboxModel = c.String(),
                        GearboxType = c.String(),
                        GearRatioFirstGear = c.String(),
                        GearRatioLastGear = c.String(),
                        TorqueConverterManufacturer = c.String(),
                        TorqueConverterModel = c.String(),
                        RetarderManufacturer = c.String(),
                        RetarderModel = c.String(),
                        RetarderType = c.String(),
                        AngledriveManufacturer = c.String(),
                        AngledriveModel = c.String(),
                        AngledriveRatio = c.String(),
                        AxleManufacturer = c.String(),
                        AxleModel = c.String(),
                        AxleGearRatio = c.String(),
                        AuxiliaryTechnologySTP = c.String(),
                        AuxiliaryTechnologyFAN = c.String(),
                        AuxiliaryTechnologyAC = c.String(),
                        AuxiliaryTechnologyPS = c.String(),
                        AuxiliaryTechnologyES = c.String(),
                        CargoVolume = c.String(),
                        Time = c.String(),
                        Distance = c.String(),
                        Speed = c.String(),
                        AltitudeDelta = c.String(),
                        FCMap_g_h = c.String(),
                        FCMap_g_km = c.String(),
                        FCAUXc_g_h = c.String(),
                        FCAUXc_g_km = c.String(),
                        FCWHTCc_g_h = c.String(),
                        FCWHTCc_g_km = c.String(),
                        FCAAUX_g_h = c.String(),
                        FCAAUX_g_km = c.String(),
                        FCFinal_g_h = c.String(),
                        FCFinal_g_km = c.String(),
                        FCFinal_l_100km = c.String(),
                        FCFinal_l_100tkm = c.String(),
                        FCFinal_l_100m3km = c.String(),
                        CO2_l_100km = c.String(),
                        CO2_l_100tkm = c.String(),
                        CO2_l_100m3km = c.String(),
                        P_wheel_in_pos = c.String(),
                        P_fcmap_pos = c.String(),
                        E_fcmap_pos = c.String(),
                        E_fcmap_neg = c.String(),
                        E_powertrain_inertia = c.String(),
                        E_aux_FAN = c.String(),
                        E_aux_STP = c.String(),
                        E_aux_AC = c.String(),
                        E_aux_PS = c.String(),
                        E_aux_ES = c.String(),
                        E_PTO_TRANSM = c.String(),
                        E_PTO_CONSUM = c.String(),
                        E_aux_sum = c.String(),
                        E_clutch_loss = c.String(),
                        E_tc_loss = c.String(),
                        E_shift_loss = c.String(),
                        E_gbx_loss = c.String(),
                        E_ret_loss = c.String(),
                        E_angle_loss = c.String(),
                        E_axl_loss = c.String(),
                        E_brake = c.String(),
                        E_vehi_inertia = c.String(),
                        E_air = c.String(),
                        E_roll = c.String(),
                        E_grad = c.String(),
                        a = c.String(),
                        a_pos = c.String(),
                        a_neg = c.String(),
                        AccelerationTimeShare = c.String(),
                        DecelerationTimeShare = c.String(),
                        CruiseTimeShare = c.String(),
                        MaxSpeed = c.String(),
                        MaxAcc = c.String(),
                        MaxDec = c.String(),
                        n_eng_avg = c.String(),
                        n_eng_max = c.String(),
                        GearShifts = c.String(),
                        StopTimeShare = c.String(),
                        EngineMaxLoadTimeShare = c.String(),
                        CoastingTimeShare = c.String(),
                        BrakingTimeShare = c.String(),
                        Gear0TimeShare = c.String(),
                        Gear1TimeShare = c.String(),
                        Gear2TimeShare = c.String(),
                        Gear3TimeShare = c.String(),
                        Gear4TimeShare = c.String(),
                        Gear5TimeShare = c.String(),
                        Gear6TimeShare = c.String(),
                        Gear7TimeShare = c.String(),
                        Gear8TimeShare = c.String(),
                        Gear9TimeShare = c.String(),
                        Gear10TimeShare = c.String(),
                        Gear11TimeShare = c.String(),
                        Gear12TimeShare = c.String(),
                    })
                .PrimaryKey(t => t.VSumRecordId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VSumRecord");
            DropTable("dbo.vehicle");
            DropTable("dbo.SimulationJob");
            DropTable("dbo.Simulation");
            DropTable("dbo.Agent");
        }
    }
}
