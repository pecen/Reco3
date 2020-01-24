using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace DataLayer.Database
{


    // Map-class used by CSV-helper to map Vecto's output to an actual object
    public class VSumRecordMap : ClassMap<VSumRecord>
    {
        public VSumRecordMap()
        {
            Map(m => m.Job).Name("Job [-]").Default(" ");
            Map(m => m.InputFile).Name("Input File [-]").Default(" ");
            Map(m => m.Cycle).Name("Cycle [-]").Default(" ");
            Map(m => m.Status).Name("Status").Default(" ");
            Map(m => m.vehicle).Name("vehicle manufacturer [-]").Default(" ");
            Map(m => m.VIN).Name("VIN number").Default(" ");
            Map(m => m.VehicleModel).Name("vehicle model [-]").Default(" ");
            Map(m => m.HDVCO2VehicleClass).Name("HDV CO2 vehicle class [-]").Default(" ");
            Map(m => m.CorrectedActualCurbMass).Name("Corrected Actual Curb Mass [kg]").Default(" ");
            Map(m => m.Loading).Name("Loading [kg]").Default(" ");
            Map(m => m.TotalVehicleMass).Name("Total vehicle mass [kg]").Default(" ");
            Map(m => m.EngineManufacturer).Name("Engine manufacturer [-]").Default(" ");
            Map(m => m.EngineModel).Name("Engine model [-]").Default(" ");
            Map(m => m.EngineFuelType).Name("Engine fuel type [-]").Default(" ");
            Map(m => m.EngineRatedPower).Name("Engine rated power [kW]").Default(" ");
            Map(m => m.EngineIdlingSpeed).Name("Engine idling speed [rpm]").Default(" ");
            Map(m => m.EngineRatedSpeed).Name("Engine rated speed [rpm]").Default(" ");
            Map(m => m.EngineDisplacement).Name("Engine displacement [ccm]").Default(" ");
            Map(m => m.EngineWHTCUrban).Name("Engine WHTCUrban").Default(" ");
            Map(m => m.EngineWHTCRural).Name("Engine WHTCRural").Default(" ");
            Map(m => m.EngineWHTCMotorway).Name("Engine WHTCMotorway").Default(" ");
            Map(m => m.EngineBFColdHot).Name("Engine BFColdHot").Default(" ");
            Map(m => m.EngineCFRegPer).Name("Engine CFRegPer").Default(" ");
            Map(m => m.EngineActualCF).Name("Engine actual CF").Default(" ");
            Map(m => m.DeclaredCdxA).Name("Declared CdxA [m²]").Default(" ");
            Map(m => m.CdxA).Name("CdxA [m²]").Default(" ");
            Map(m => m.TotalRRC).Name("total RRC [-]").Default(" ");
            Map(m => m.WeightedRRCwoTrailer).Name("weighted RRC w/o trailer [-]").Default(" ");
            Map(m => m.r_dyn).Name("r_dyn [m]").Default(" ");
            Map(m => m.NumberAxlesVehicleDriven).Name("Number axles vehicle driven [-]").Default(" ");
            Map(m => m.NumberAxlesVehicleNonDriven).Name("Number axles vehicle non-driven [-]").Default(" ");
            Map(m => m.NumberAxlesVehicleTrailer).Name("Number axles trailer [-]").Default(" ");
            Map(m => m.GearboxManufacturer).Name("Gearbox manufacturer [-]").Default(" ");
            Map(m => m.GearboxModel).Name("Gearbox model [-]").Default(" ");
            Map(m => m.GearboxType).Name("Gearbox type [-]").Default(" ");
            Map(m => m.GearRatioFirstGear).Name("Gear ratio first gear [-]").Default(" ");
            Map(m => m.GearRatioLastGear).Name("Gear ratio last gear [-]").Default(" ");
            Map(m => m.TorqueConverterManufacturer).Name("Torque converter manufacturer [-]").Default(" ");
            Map(m => m.TorqueConverterModel).Name("Torque converter model [-]").Default(" ");
            Map(m => m.RetarderManufacturer).Name("Retarder manufacturer [-]").Default(" ");
            Map(m => m.RetarderModel).Name("Retarder model [-]").Default(" ");
            Map(m => m.RetarderType).Name("Retarder type [-]").Default(" ");
            Map(m => m.AngledriveManufacturer).Name("Angledrive manufacturer [-]").Default(" ");
            Map(m => m.AngledriveModel).Name("Angledrive model [-]").Default(" ");
            Map(m => m.AngledriveRatio).Name("Angledrive ratio [-]").Default(" ");
            Map(m => m.AxleManufacturer).Name("Axle manufacturer [-]").Default(" ");
            Map(m => m.AxleModel).Name("Axle model [-]").Default(" ");
            Map(m => m.AxleGearRatio).Name("Axle gear ratio [-]").Default(" ");
            Map(m => m.AuxiliaryTechnologySTP).Name("Auxiliary technology STP [-]").Default(" ");
            Map(m => m.AuxiliaryTechnologyFAN).Name("Auxiliary technology FAN [-]").Default(" ");
            Map(m => m.AuxiliaryTechnologyAC).Name("Auxiliary technology AC [-]").Default(" ");
            Map(m => m.AuxiliaryTechnologyPS).Name("Auxiliary technology PS [-]").Default(" ");
            Map(m => m.AuxiliaryTechnologyES).Name("Auxiliary technology ES [-]").Default(" ");
            Map(m => m.CargoVolume).Name("Cargo Volume [m³]").Default(" ");
            Map(m => m.Time).Name("time [s]").Default(" ");
            Map(m => m.Distance).Name("distance [km]").Default(" ");
            Map(m => m.Speed).Name("speed [km/h]").Default(" ");
            Map(m => m.AltitudeDelta).Name("altitudeDelta [m]").Default(" ");
            Map(m => m.FCMap_g_h).Name("FC-Map [g/h]").Default(" ");
            Map(m => m.FCMap_g_km).Name("FC-Map [g/km]").Default(" ");
            Map(m => m.FCAUXc_g_h).Name("FC-AUXc [g/h]").Default(" ");
            Map(m => m.FCAUXc_g_km).Name("FC-AUXc [g/km]").Default(" ");
            Map(m => m.FCWHTCc_g_h).Name("FC-WHTCc [g/h]").Default(" ");
            Map(m => m.FCWHTCc_g_km).Name("FC-WHTCc [g/km]").Default(" ");
            Map(m => m.FCAAUX_g_h).Name("FC-AAUX [g/h]").Default(" ");
            Map(m => m.FCAAUX_g_km).Name("FC-AAUX [g/km]").Default(" ");
            Map(m => m.FCFinal_g_h).Name("FC-Final [g/h]").Default(" ");
            Map(m => m.FCFinal_g_km).Name("FC-Final [g/km]").Default(" ");
            Map(m => m.FCFinal_l_100km).Name("FC-Final [l/100km]").Default(" ");
            Map(m => m.FCFinal_l_100tkm).Name("FC-Final [l/100tkm]").Default(" ");
            Map(m => m.FCFinal_l_100m3km).Name("FC-Final [l/100m³km]").Default(" ");
            Map(m => m.CO2_l_100km).Name("CO2 [g/km]").Default(" ");
            Map(m => m.CO2_l_100tkm).Name("CO2 [g/tkm]").Default(" ");
            Map(m => m.CO2_l_100m3km).Name("CO2 [g/m³km]").Default(" ");
            Map(m => m.P_wheel_in_pos).Name("P_wheel_in_pos [kW]").Default(" ");
            Map(m => m.P_fcmap_pos).Name("P_fcmap_pos [kW]").Default(" ");
            Map(m => m.E_fcmap_pos).Name("E_fcmap_pos [kWh]").Default(" ");
            Map(m => m.E_fcmap_neg).Name("E_fcmap_neg [kWh]").Default(" ");
            Map(m => m.E_powertrain_inertia).Name("E_powertrain_inertia [kWh]").Default(" ");
            Map(m => m.E_aux_FAN).Name("E_aux_FAN [kWh]").Default(" ");
            Map(m => m.E_aux_STP).Name("E_aux_STP [kWh]").Default(" ");
            Map(m => m.E_aux_AC).Name("E_aux_AC [kWh]").Default(" ");
            Map(m => m.E_aux_PS).Name("E_aux_PS [kWh]").Default(" ");
            Map(m => m.E_aux_ES).Name("E_aux_ES [kWh]").Default(" ");
            Map(m => m.E_PTO_TRANSM).Name("E_PTO_TRANSM [kWh]").Default(" ");
            Map(m => m.E_PTO_CONSUM).Name("E_PTO_CONSUM [kWh]").Default(" ");
            Map(m => m.E_aux_sum).Name("E_aux_sum [kWh]").Default(" ");
            Map(m => m.E_clutch_loss).Name("E_clutch_loss [kWh]").Default(" ");
            Map(m => m.E_tc_loss).Name("E_tc_loss [kWh]").Default(" ");
            Map(m => m.E_shift_loss).Name("E_shift_loss [kWh]").Default(" ");
            Map(m => m.E_gbx_loss).Name("E_gbx_loss [kWh]").Default(" ");
            Map(m => m.E_ret_loss).Name("E_ret_loss [kWh]").Default(" ");
            Map(m => m.E_angle_loss).Name("E_angle_loss [kWh]").Default(" ");
            Map(m => m.E_axl_loss).Name("E_axl_loss [kWh]").Default(" ");
            Map(m => m.E_brake).Name("E_brake [kWh]").Default(" ");
            Map(m => m.E_vehi_inertia).Name("E_vehi_inertia [kWh]").Default(" ");
            Map(m => m.E_air).Name("E_air [kWh]").Default(" ");
            Map(m => m.E_roll).Name("E_roll [kWh]").Default(" ");
            Map(m => m.E_grad).Name("E_grad [kWh]").Default(" ");
            Map(m => m.a).Name("a [m/s^2]").Default(" ");
            Map(m => m.a_pos).Name("a_pos [m/s^2]").Default(" ");
            Map(m => m.a_neg).Name("a_neg [m/s^2]").Default(" ");
            Map(m => m.AccelerationTimeShare).Name("AccelerationTimeShare [%]").Default(" ");
            Map(m => m.DecelerationTimeShare).Name("DecelerationTimeShare [%]").Default(" ");
            Map(m => m.CruiseTimeShare).Name("CruiseTimeShare [%]").Default(" ");
            Map(m => m.MaxSpeed).Name("max. speed [km/h]").Default(" ");
            Map(m => m.MaxAcc).Name("max. acc [m/s²]").Default(" ");
            Map(m => m.MaxDec).Name("max. dec [m/s²]").Default(" ");
            Map(m => m.n_eng_avg).Name("n_eng_avg [rpm]").Default(" ");
            Map(m => m.n_eng_max).Name("n_eng_max [rpm]").Default(" ");
            Map(m => m.GearShifts).Name("gear shifts [-]").Default(" ");
            Map(m => m.StopTimeShare).Name("StopTimeShare [%]").Default(" ");
            Map(m => m.EngineMaxLoadTimeShare).Name("Engine max. Load time share [%]").Default(" ");
            Map(m => m.CoastingTimeShare).Name("CoastingTimeShare [%]").Default(" ");
            Map(m => m.BrakingTimeShare).Name("BrakingTImeShare [%]").Default(" ");
            Map(m => m.Gear0TimeShare).Name("Gear 0 TimeShare [%]").Default(" ");
            Map(m => m.Gear1TimeShare).Name("Gear 1 TimeShare [%]").Default(" ");
            Map(m => m.Gear2TimeShare).Name("Gear 2 TimeShare [%]").Default(" ");
            Map(m => m.Gear3TimeShare).Name("Gear 3 TimeShare [%]").Default(" ");
            Map(m => m.Gear4TimeShare).Name("Gear 4 TimeShare [%]").Default(" ");
            Map(m => m.Gear5TimeShare).Name("Gear 5 TimeShare [%]").Default(" ");
            Map(m => m.Gear6TimeShare).Name("Gear 6 TimeShare [%]").Default(" ");
            Map(m => m.Gear7TimeShare).Name("Gear 7 TimeShare [%]").Default(" ");
            Map(m => m.Gear8TimeShare).Name("Gear 8 TimeShare [%]").Default(" ");
            Map(m => m.Gear9TimeShare).Name("Gear 9 TimeShare [%]").Default(" ");
            Map(m => m.Gear10TimeShare).Name("Gear 10 TimeShare [%]").Default(" ");
            Map(m => m.Gear11TimeShare).Name("Gear 11 TimeShare [%]").Default(" ");
            Map(m => m.Gear12TimeShare).Name("Gear 12 TimeShare [%]").Default(" ");


        }
    }
    

    [DataContract]
    public class VSumRecord
    {
        [DataMember]
        public int VSumRecordId { get; set; }
        [DataMember]
        public string VectoApiVersion { get; set; }
        [DataMember]
        public DateTime SimulationTimeStamp { get; set; }
        [DataMember]
        [Index]
        public int VehicleId { get; set; }
        [DataMember]
        public int SimulationId { get; set; }
        [DataMember]
        public string Job { get; set; }
        [DataMember]
        public string InputFile { get; set; }
        [DataMember]
        public string Cycle { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string vehicle { get; set; }
        [DataMember]
        public string VIN { get; set; }
        [DataMember]
        public string VehicleModel { get; set; }
        [DataMember]
        public string HDVCO2VehicleClass { get; set; }
        [DataMember]
        public string CorrectedActualCurbMass { get; set; }
        [DataMember]
        public string Loading { get; set; }
        [DataMember]
        public string TotalVehicleMass { get; set; }
        [DataMember]
        public string EngineManufacturer { get; set; }
        [DataMember]
        public string EngineModel { get; set; }
        [DataMember]
        public string EngineFuelType { get; set; }
        [DataMember]
        public string EngineRatedPower { get; set; }
        [DataMember]
        public string EngineIdlingSpeed { get; set; }
        [DataMember]
        public string EngineRatedSpeed { get; set; }
        [DataMember]
        public string EngineDisplacement { get; set; }
        [DataMember]
        public string EngineWHTCUrban { get; set; }
        [DataMember]
        public string EngineWHTCRural { get; set; }
        [DataMember]
        public string EngineWHTCMotorway { get; set; }
        [DataMember]
        public string EngineBFColdHot { get; set; }
        [DataMember]
        public string EngineCFRegPer { get; set; }
        [DataMember]
        public string EngineActualCF { get; set; }
        [DataMember]
        public string DeclaredCdxA { get; set; }
        [DataMember]
        public string CdxA { get; set; }
        [DataMember]
        public string TotalRRC { get; set; }
        [DataMember]
        public string WeightedRRCwoTrailer { get; set; }
        [DataMember]
        public string r_dyn { get; set; }
        [DataMember]
        public string NumberAxlesVehicleDriven { get; set; }
        [DataMember]
        public string NumberAxlesVehicleNonDriven { get; set; }
        [DataMember]
        public string NumberAxlesVehicleTrailer { get; set; }
        [DataMember]
        public string GearboxManufacturer { get; set; }
        [DataMember]
        public string GearboxModel { get; set; }
        [DataMember]
        public string GearboxType { get; set; }
        [DataMember]
        public string GearRatioFirstGear { get; set; }
        [DataMember]
        public string GearRatioLastGear { get; set; }
        [DataMember]
        public string TorqueConverterManufacturer { get; set; }
        [DataMember]
        public string TorqueConverterModel { get; set; }
        [DataMember]
        public string RetarderManufacturer { get; set; }
        [DataMember]
        public string RetarderModel { get; set; }
        [DataMember]
        public string RetarderType { get; set; }
        [DataMember]
        public string AngledriveManufacturer { get; set; }
        [DataMember]
        public string AngledriveModel { get; set; }
        [DataMember]
        public string AngledriveRatio { get; set; }
        [DataMember]
        public string AxleManufacturer { get; set; }
        [DataMember]
        public string AxleModel { get; set; }
        [DataMember]
        public string AxleGearRatio { get; set; }
        [DataMember]
        public string AuxiliaryTechnologySTP { get; set; }
        [DataMember]
        public string AuxiliaryTechnologyFAN { get; set; }
        [DataMember]
        public string AuxiliaryTechnologyAC { get; set; }
        [DataMember]
        public string AuxiliaryTechnologyPS { get; set; }
        [DataMember]
        public string AuxiliaryTechnologyES { get; set; }
        [DataMember]
        public string CargoVolume { get; set; }
        [DataMember]
        public string Time { get; set; }
        [DataMember]
        public string Distance { get; set; }
        [DataMember]
        public string Speed { get; set; }
        [DataMember]
        public string AltitudeDelta { get; set; }
        [DataMember]
        public string FCMap_g_h { get; set; }
        [DataMember]
        public string FCMap_g_km { get; set; }
        [DataMember]
        public string FCAUXc_g_h { get; set; }
        [DataMember]
        public string FCAUXc_g_km { get; set; }
        [DataMember]
        public string FCWHTCc_g_h { get; set; }
        [DataMember]
        public string FCWHTCc_g_km { get; set; }
        [DataMember]
        public string FCAAUX_g_h { get; set; }
        [DataMember]
        public string FCAAUX_g_km { get; set; }
        [DataMember]
        public string FCFinal_g_h { get; set; }
        [DataMember]
        public string FCFinal_g_km { get; set; }
        [DataMember]
        public string FCFinal_l_100km { get; set; }
        [DataMember]
        public string FCFinal_l_100tkm { get; set; }
        [DataMember]
        public string FCFinal_l_100m3km { get; set; }
        [DataMember]
        public string CO2_l_100km { get; set; }
        [DataMember]
        public string CO2_l_100tkm { get; set; }
        [DataMember]
        public string CO2_l_100m3km { get; set; }
        [DataMember]
        public string P_wheel_in_pos { get; set; }
        [DataMember]
        public string P_fcmap_pos { get; set; }
        [DataMember]
        public string E_fcmap_pos { get; set; }
        [DataMember]
        public string E_fcmap_neg { get; set; }
        [DataMember]
        public string E_powertrain_inertia { get; set; }
        [DataMember]
        public string E_aux_FAN { get; set; }
        [DataMember]
        public string E_aux_STP { get; set; }
        [DataMember]
        public string E_aux_AC { get; set; }
        [DataMember]
        public string E_aux_PS { get; set; }
        [DataMember]
        public string E_aux_ES { get; set; }
        [DataMember]
        public string E_PTO_TRANSM { get; set; }
        [DataMember]
        public string E_PTO_CONSUM { get; set; }
        [DataMember]
        public string E_aux_sum { get; set; }
        [DataMember]
        public string E_clutch_loss { get; set; }
        [DataMember]
        public string E_tc_loss { get; set; }
        [DataMember]
        public string E_shift_loss { get; set; }
        [DataMember]
        public string E_gbx_loss { get; set; }
        [DataMember]
        public string E_ret_loss { get; set; }
        [DataMember]
        public string E_angle_loss { get; set; }
        [DataMember]
        public string E_axl_loss { get; set; }
        [DataMember]
        public string E_brake { get; set; }
        [DataMember]
        public string E_vehi_inertia { get; set; }
        [DataMember]
        public string E_air { get; set; }
        [DataMember]
        public string E_roll { get; set; }
        [DataMember]
        public string E_grad { get; set; }
        [DataMember]
        public string a { get; set; }
        [DataMember]
        public string a_pos { get; set; }
        [DataMember]
        public string a_neg { get; set; }
        [DataMember]
        public string AccelerationTimeShare { get; set; }
        [DataMember]
        public string DecelerationTimeShare { get; set; }
        [DataMember]
        public string CruiseTimeShare { get; set; }
        [DataMember]
        public string MaxSpeed { get; set; }
        [DataMember]
        public string MaxAcc { get; set; }
        [DataMember]
        public string MaxDec { get; set; }
        [DataMember]
        public string n_eng_avg { get; set; }
        [DataMember]
        public string n_eng_max { get; set; }
        [DataMember]
        public string GearShifts { get; set; }
        [DataMember]
        public string StopTimeShare { get; set; }
        [DataMember]
        public string EngineMaxLoadTimeShare { get; set; }
        [DataMember]
        public string CoastingTimeShare { get; set; }
        [DataMember]
        public string BrakingTimeShare { get; set; }
        [DataMember]
        public string Gear0TimeShare { get; set; }
        [DataMember]
        public string Gear1TimeShare { get; set; }
        [DataMember]
        public string Gear2TimeShare { get; set; }
        [DataMember]
        public string Gear3TimeShare { get; set; }
        [DataMember]
        public string Gear4TimeShare { get; set; }
        [DataMember]
        public string Gear5TimeShare { get; set; }
        [DataMember]
        public string Gear6TimeShare { get; set; }
        [DataMember]
        public string Gear7TimeShare { get; set; }
        [DataMember]
        public string Gear8TimeShare { get; set; }
        [DataMember]
        public string Gear9TimeShare { get; set; }
        [DataMember]
        public string Gear10TimeShare { get; set; }
        [DataMember]
        public string Gear11TimeShare { get; set; }
        [DataMember]
        public string Gear12TimeShare { get; set; }

        public VSumRecord Clone()
        {
            VSumRecord clonedVSum = new VSumRecord();
            clonedVSum.SimulationTimeStamp = SimulationTimeStamp;
            clonedVSum.VehicleId = VehicleId;
            clonedVSum.SimulationId = SimulationId;
            clonedVSum.Job = Job;
            clonedVSum.InputFile = InputFile;
            clonedVSum.Cycle = Cycle;
            clonedVSum.Status = Status;
            clonedVSum.vehicle = vehicle;
            clonedVSum.VIN = VIN;
            clonedVSum.VehicleModel = VehicleModel;
            clonedVSum.HDVCO2VehicleClass = HDVCO2VehicleClass;
            clonedVSum.CorrectedActualCurbMass = CorrectedActualCurbMass;
            clonedVSum.Loading = Loading;
            clonedVSum.TotalVehicleMass = TotalVehicleMass;
            clonedVSum.EngineManufacturer = EngineManufacturer;
            clonedVSum.EngineModel = EngineModel;
            clonedVSum.EngineFuelType = EngineFuelType;
            clonedVSum.EngineRatedPower = EngineRatedPower;
            clonedVSum.EngineIdlingSpeed = EngineIdlingSpeed;
            clonedVSum.EngineRatedSpeed = EngineRatedSpeed;
            clonedVSum.EngineDisplacement = EngineDisplacement;
            clonedVSum.EngineWHTCUrban = EngineWHTCUrban;
            clonedVSum.EngineWHTCRural = EngineWHTCRural;
            clonedVSum.EngineWHTCMotorway = EngineWHTCMotorway;
            clonedVSum.EngineBFColdHot = EngineBFColdHot;
            clonedVSum.EngineCFRegPer = EngineCFRegPer;
            clonedVSum.EngineActualCF = EngineActualCF;
            clonedVSum.DeclaredCdxA = DeclaredCdxA;
            clonedVSum.CdxA = CdxA;
            clonedVSum.TotalRRC = TotalRRC;
            clonedVSum.WeightedRRCwoTrailer = WeightedRRCwoTrailer;
            clonedVSum.r_dyn = r_dyn;
            clonedVSum.NumberAxlesVehicleDriven = NumberAxlesVehicleDriven;
            clonedVSum.NumberAxlesVehicleNonDriven = NumberAxlesVehicleNonDriven;
            clonedVSum.NumberAxlesVehicleTrailer = NumberAxlesVehicleTrailer;
            clonedVSum.GearboxManufacturer = GearboxManufacturer;
            clonedVSum.GearboxModel = GearboxModel;
            clonedVSum.GearboxType = GearboxType;
            clonedVSum.GearRatioFirstGear = GearRatioFirstGear;
            clonedVSum.GearRatioLastGear = GearRatioLastGear;
            clonedVSum.TorqueConverterManufacturer = TorqueConverterManufacturer;
            clonedVSum.TorqueConverterModel = TorqueConverterModel;
            clonedVSum.RetarderManufacturer = RetarderManufacturer;
            clonedVSum.RetarderModel = RetarderModel;
            clonedVSum.RetarderType = RetarderType;
            clonedVSum.AngledriveManufacturer = AngledriveManufacturer;
            clonedVSum.AngledriveModel = AngledriveModel;
            clonedVSum.AngledriveRatio = AngledriveRatio;
            clonedVSum.AxleManufacturer = AxleManufacturer;
            clonedVSum.AxleModel = AxleModel;
            clonedVSum.AxleGearRatio = AxleGearRatio;
            clonedVSum.AuxiliaryTechnologySTP = AuxiliaryTechnologySTP;
            clonedVSum.AuxiliaryTechnologyFAN = AuxiliaryTechnologyFAN;
            clonedVSum.AuxiliaryTechnologyAC = AuxiliaryTechnologyAC;
            clonedVSum.AuxiliaryTechnologyPS = AuxiliaryTechnologyPS;
            clonedVSum.AuxiliaryTechnologyES = AuxiliaryTechnologyES;
            clonedVSum.CargoVolume = CargoVolume;
            clonedVSum.Time = Time;
            clonedVSum.Distance = Distance;
            clonedVSum.Speed = Speed;
            clonedVSum.AltitudeDelta = AltitudeDelta;
            clonedVSum.FCMap_g_h = FCMap_g_h;
            clonedVSum.FCMap_g_km = FCMap_g_km;
            clonedVSum.FCAUXc_g_h = FCAUXc_g_h;
            clonedVSum.FCAUXc_g_km = FCAUXc_g_km;
            clonedVSum.FCWHTCc_g_h = FCWHTCc_g_h;
            clonedVSum.FCWHTCc_g_km = FCWHTCc_g_km;
            clonedVSum.FCAAUX_g_h = FCAAUX_g_h;
            clonedVSum.FCAAUX_g_km = FCAAUX_g_km;
            clonedVSum.FCFinal_g_h = FCFinal_g_h;
            clonedVSum.FCFinal_g_km = FCFinal_g_km;
            clonedVSum.FCFinal_l_100km = FCFinal_l_100km;
            clonedVSum.FCFinal_l_100tkm = FCFinal_l_100tkm;
            clonedVSum.FCFinal_l_100m3km = FCFinal_l_100m3km;
            clonedVSum.CO2_l_100km = CO2_l_100km;
            clonedVSum.CO2_l_100tkm = CO2_l_100tkm;
            clonedVSum.CO2_l_100m3km = CO2_l_100m3km;
            clonedVSum.P_wheel_in_pos = P_wheel_in_pos;
            clonedVSum.P_fcmap_pos = P_fcmap_pos;
            clonedVSum.E_fcmap_pos = E_fcmap_pos;
            clonedVSum.E_fcmap_neg = E_fcmap_neg;
            clonedVSum.E_powertrain_inertia = E_powertrain_inertia;
            clonedVSum.E_aux_FAN = E_aux_FAN;
            clonedVSum.E_aux_STP = E_aux_STP;
            clonedVSum.E_aux_AC = E_aux_AC;
            clonedVSum.E_aux_PS = E_aux_PS;
            clonedVSum.E_aux_ES = E_aux_ES;
            clonedVSum.E_PTO_TRANSM = E_PTO_TRANSM;
            clonedVSum.E_PTO_CONSUM = E_PTO_CONSUM;
            clonedVSum.E_aux_sum = E_aux_sum;
            clonedVSum.E_clutch_loss = E_clutch_loss;
            clonedVSum.E_tc_loss = E_tc_loss;
            clonedVSum.E_shift_loss = E_shift_loss;
            clonedVSum.E_gbx_loss = E_gbx_loss;
            clonedVSum.E_ret_loss = E_ret_loss;
            clonedVSum.E_angle_loss = E_angle_loss;
            clonedVSum.E_axl_loss = E_axl_loss;
            clonedVSum.E_brake = E_brake;
            clonedVSum.E_vehi_inertia = E_vehi_inertia;
            clonedVSum.E_air = E_air;
            clonedVSum.E_roll = E_roll;
            clonedVSum.E_grad = E_grad;
            clonedVSum.a = a;
            clonedVSum.a_pos = a_pos;
            clonedVSum.a_neg = a_neg;
            clonedVSum.AccelerationTimeShare = AccelerationTimeShare;
            clonedVSum.DecelerationTimeShare = DecelerationTimeShare;
            clonedVSum.CruiseTimeShare = CruiseTimeShare;
            clonedVSum.MaxSpeed = MaxSpeed;
            clonedVSum.MaxAcc = MaxAcc;
            clonedVSum.MaxDec = MaxDec;
            clonedVSum.n_eng_avg = n_eng_avg;
            clonedVSum.n_eng_max = n_eng_max;
            clonedVSum.GearShifts = GearShifts;
            clonedVSum.StopTimeShare = StopTimeShare;
            clonedVSum.EngineMaxLoadTimeShare = EngineMaxLoadTimeShare;
            clonedVSum.CoastingTimeShare = CoastingTimeShare;
            clonedVSum.BrakingTimeShare = BrakingTimeShare;
            clonedVSum.Gear0TimeShare = Gear0TimeShare;
            clonedVSum.Gear1TimeShare = Gear1TimeShare;
            clonedVSum.Gear2TimeShare = Gear2TimeShare;
            clonedVSum.Gear3TimeShare = Gear3TimeShare;
            clonedVSum.Gear4TimeShare = Gear4TimeShare;
            clonedVSum.Gear5TimeShare = Gear5TimeShare;
            clonedVSum.Gear6TimeShare = Gear6TimeShare;
            clonedVSum.Gear7TimeShare = Gear7TimeShare;
            clonedVSum.Gear8TimeShare = Gear8TimeShare;
            clonedVSum.Gear9TimeShare = Gear9TimeShare;
            clonedVSum.Gear10TimeShare = Gear10TimeShare;
            clonedVSum.Gear11TimeShare = Gear11TimeShare;
            clonedVSum.Gear12TimeShare = Gear12TimeShare;
            return clonedVSum;
        }
    }

    public class VSumEntry
    {
        public List<VSumRecord> Records { get; set; }

        public string ApiVersion { get; set; }

        public VSumEntry()
        {

        }


        public void SetSimulationId(int nSimulationID, DateTime dtStamp, int nVehicleId)
        {
            foreach (var record in Records)
            {
                record.SimulationId = nSimulationID;
                record.SimulationTimeStamp = dtStamp;
                record.VehicleId = nVehicleId;
                record.VectoApiVersion = ApiVersion;
            }
        }

        public VSumEntry(string strFilename)
        {
            LoadFile(strFilename);
        }
        public bool LoadFile(string strFilename)
        {
            try
            {

                using (var textReader = File.OpenText(strFilename))
                {
                    using (var csv = new CsvReader(textReader))
                    {

                        csv.Configuration.AllowComments = true;


                        csv.Configuration.HasHeaderRecord = true;

                        // Ignore missing field.
                        csv.Configuration.MissingFieldFound = null;

                        // Log missing field.
                        csv.Configuration.MissingFieldFound = (headerNames, index, context) =>
                        {
                            //logger.WriteLine($"Field with names ['{string.Join("', '", headerNames)}'] at index '{index}' was not found. );
                        };

                        // Turn off header validation.
                        csv.Configuration.HeaderValidated = null;

                        // Log instead of throwing an exception.
                        csv.Configuration.HeaderValidated = (isValid, headerNames, headerNameIndex, context) =>
                        {
                            if (!isValid)
                            {
                                //logger.WriteLine($"Header matching ['{string.Join("', '", headerNames)}'] names at index {headerNameIndex} was not found.");
                            }
                        };
                        csv.Configuration.RegisterClassMap<VSumRecordMap>();

                        string[] headerRow = csv.Context.HeaderRecord;

                        Records = csv.GetRecords<VSumRecord>().ToList();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }

            return false;
        }

        public bool LoadString(string strVectoResult)
        {
            try
            {

                using (var textReader = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(strVectoResult.ToString()))))
                {
                    using (var csv = new CsvReader(textReader))
                    {

                        csv.Configuration.AllowComments = true;


                        csv.Configuration.HasHeaderRecord = true;

                        // Ignore missing field.
                        csv.Configuration.MissingFieldFound = null;

                        // Log missing field.
                        csv.Configuration.MissingFieldFound = (headerNames, index, context) =>
                        {
                            //logger.WriteLine($"Field with names ['{string.Join("', '", headerNames)}'] at index '{index}' was not found. );
                        };

                        // Turn off header validation.
                        csv.Configuration.HeaderValidated = null;

                        // Log instead of throwing an exception.
                        csv.Configuration.HeaderValidated = (isValid, headerNames, headerNameIndex, context) =>
                        {
                            if (!isValid)
                            {
                                //logger.WriteLine($"Header matching ['{string.Join("', '", headerNames)}'] names at index {headerNameIndex} was not found.");
                            }
                        };
                        csv.Configuration.RegisterClassMap<VSumRecordMap>();

                        string[] headerRow = csv.Context.HeaderRecord;

                        Records = csv.GetRecords<VSumRecord>().ToList();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }

            return false;
        }
    }
}
