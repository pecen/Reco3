﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace Scania.Baseline.SchemaA {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class vehicles {
        
        private vehiclesVehicle[] vehicleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("vehicle")]
        public vehiclesVehicle[] vehicle {
            get {
                return this.vehicleField;
            }
            set {
                this.vehicleField = value;
            }
        }
	}
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicle {

		public static implicit operator vehiclesVehicle(Scania.Baseline.vehiclesVehicle vehicle)
		{
			var convertedVehicle = new vehiclesVehicle();
			convertedVehicle.Manufacturer = vehicle.Manufacturer;
			convertedVehicle.ManufacturerAddress = vehicle.ManufacturerAddress;
			convertedVehicle.VIN = vehicle.VIN;
			convertedVehicle.Model = vehicle.Model;
			convertedVehicle.Date = vehicle.Date;
			convertedVehicle.DevelopmentLevel = vehicle.DevelopmentLevel;
			convertedVehicle.LegislativeClass = vehicle.LegislativeClass;
			convertedVehicle.VehicleCategory = vehicle.VehicleCategory;
			convertedVehicle.AxleConfiguration = vehicle.AxleConfiguration;
			convertedVehicle.CurbMassChassis = vehicle.CurbMassChassis;
			convertedVehicle.GrossVehicleMass = vehicle.GrossVehicleMass;
			convertedVehicle.IdlingSpeed = vehicle.IdlingSpeed;
			convertedVehicle.RetarderType = vehicle.RetarderType;
			convertedVehicle.RetarderRatio = vehicle.RetarderRatio;
			convertedVehicle.AngledriveType = vehicle.AngledriveType;
			convertedVehicle.PTO = new vehiclesVehiclePTO
			{
				PTOOtherElements = vehicle.PTO.PTOOtherElements,
				PTOShaftsGearWheels = vehicle.PTO.PTOShaftsGearWheels
			};
			convertedVehicle.ZeroEmissionVehicle = vehicle.ZeroEmissionVehicle;
			convertedVehicle.VocationalVehicle = vehicle.VocationalVehicle;
			convertedVehicle.SleeperCab = vehicle.SleeperCab;
			if (vehicle.NgTankSystem.Length > 1)
			{
				convertedVehicle.NgTankSystem = vehicle.NgTankSystem;
			}
			convertedVehicle.ADAS = new vehiclesVehicleADAS
			{
				EcoRollWithEngineStop = vehicle.ADAS.EcoRollWithEngineStop,
				EcoRollWithoutEngineStop = vehicle.ADAS.EcoRollWithoutEngineStop,
				EngineStopStart = vehicle.ADAS.EngineStopStart,
				PredictiveCruiseControl = vehicle.ADAS.PredictiveCruiseControl
			};
			convertedVehicle.Components = new vehiclesVehicleComponents
			{
				AirDrag = new vehiclesVehicleComponentsAirDrag
				{
					AirDragModel = vehicle.Components.AirDrag.AirDragModel,
					AirDragPD = vehicle.Components.AirDrag.AirDragPD
				},
				Auxiliaries = new vehiclesVehicleComponentsAuxiliaries
				{
					Data = new vehiclesVehicleComponentsAuxiliariesData
					{
						ElectricSystem = new vehiclesVehicleComponentsAuxiliariesDataElectricSystem
						{
							Technology = vehicle.Components.Auxiliaries.Data.ElectricSystem.Technology
						},
						Fan = new vehiclesVehicleComponentsAuxiliariesDataFan
						{
							Technology = vehicle.Components.Auxiliaries.Data.Fan.Technology
						},
						HVAC = new vehiclesVehicleComponentsAuxiliariesDataHVAC
						{
							Technology = vehicle.Components.Auxiliaries.Data.HVAC.Technology
						},
						PneumaticSystem = new vehiclesVehicleComponentsAuxiliariesDataPneumaticSystem
						{
							Technology = vehicle.Components.Auxiliaries.Data.PneumaticSystem.Technology
						},
						SteeringPump = new vehiclesVehicleComponentsAuxiliariesDataSteeringPump
						{
							Technology = vehicle.Components.Auxiliaries.Data.SteeringPump.Technology
						}
					}
				},
				AxleGearPD = vehicle.Components.AxleGearPD,
				AxleWheels = new vehiclesVehicleComponentsAxleWheels
				{
					Data = new vehiclesVehicleComponentsAxleWheelsData
					{
						Axles =	new vehiclesVehicleComponentsAxleWheelsDataAxle[vehicle.Components.AxleWheels.Data.Axles.Length]			
					}					
				},
				Engine = new vehiclesVehicleComponentsEngine
				{
					EnginePD = vehicle.Components.Engine.EnginePD,
					EngineStrokeVolume = vehicle.Components.Engine.EngineStrokeVolume
				},
				GearBoxPD = vehicle.Components.GearBoxPD,
				RetarderPD = vehicle.Components.RetarderPD,
				TorqueConverterPD = vehicle.Components.TorqueConverterPD
			};

			for(int i = 0; i < convertedVehicle.Components.AxleWheels.Data.Axles.Length; i++)
			{
				convertedVehicle.Components.AxleWheels.Data.Axles[i] = vehicle.Components.AxleWheels.Data.Axles[i];
			}

			return convertedVehicle;
		}

		private string manufacturerField;
        
        private string manufacturerAddressField;
        
        private string vINField;
        
        private string modelField;
        
        private System.DateTime dateField;
        
        private byte developmentLevelField;
        
        private string legislativeClassField;
        
        private string vehicleCategoryField;
        
        private string axleConfigurationField;
        
        private string curbMassChassisField;
        
        private string grossVehicleMassField;
        
        private string idlingSpeedField;
        
        private string retarderTypeField;
        
        private string retarderRatioField;
        
        private string angledriveTypeField;
        
        private vehiclesVehiclePTO pTOField;
        
        private bool vocationalVehicleField;
        
        private bool sleeperCabField;
        
        private string ngTankSystemField;
        
        private vehiclesVehicleADAS aDASField;
        
        private bool zeroEmissionVehicleField;
        
        private bool hybridElectricHDVField;
        
        private bool dualFuelVehicleField;
        
        private vehiclesVehicleComponents componentsField;
        
        public vehiclesVehicle() {
            this.manufacturerField = "Scania";
            this.manufacturerAddressField = "Soedertaelje";
            this.retarderTypeField = "None";
            this.angledriveTypeField = "None";
        }
        
        /// <remarks/>
        public string Manufacturer {
            get {
                return this.manufacturerField;
            }
            set {
                this.manufacturerField = value;
            }
        }
        
        /// <remarks/>
        public string ManufacturerAddress {
            get {
                return this.manufacturerAddressField;
            }
            set {
                this.manufacturerAddressField = value;
            }
        }
        
        /// <remarks/>
        public string VIN {
            get {
                return this.vINField;
            }
            set {
                this.vINField = value;
            }
        }
        
        /// <remarks/>
        public string Model {
            get {
                return this.modelField;
            }
            set {
                this.modelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime Date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }
        
        /// <remarks/>
        public byte DevelopmentLevel {
            get {
                return this.developmentLevelField;
            }
            set {
                this.developmentLevelField = value;
            }
        }
        
        /// <remarks/>
        public string LegislativeClass {
            get {
                return this.legislativeClassField;
            }
            set {
                this.legislativeClassField = value;
            }
        }
        
        /// <remarks/>
        public string VehicleCategory {
            get {
                return this.vehicleCategoryField;
            }
            set {
                this.vehicleCategoryField = value;
            }
        }
        
        /// <remarks/>
        public string AxleConfiguration {
            get {
                return this.axleConfigurationField;
            }
            set {
                this.axleConfigurationField = value;
            }
        }
        
        /// <remarks/>
        public string CurbMassChassis {
            get {
                return this.curbMassChassisField;
            }
            set {
                this.curbMassChassisField = value;
            }
        }
        
        /// <remarks/>
        public string GrossVehicleMass {
            get {
                return this.grossVehicleMassField;
            }
            set {
                this.grossVehicleMassField = value;
            }
        }
        
        /// <remarks/>
        public string IdlingSpeed {
            get {
                return this.idlingSpeedField;
            }
            set {
                this.idlingSpeedField = value;
            }
        }
        
        /// <remarks/>
        public string RetarderType {
            get {
                return this.retarderTypeField;
            }
            set {
                this.retarderTypeField = value;
            }
        }
        
        /// <remarks/>
        // CODEGEN Warning: DefaultValue attribute on members of type System.Object is not supported in this version of the .Net Framework.
        // CODEGEN Warning: 'default' attribute supported only for primitive types.  Ignoring default='0' attribute.
        public string RetarderRatio {
            get {
                return this.retarderRatioField;
            }
            set {
                this.retarderRatioField = value;
            }
        }
        
        /// <remarks/>
        public string AngledriveType {
            get {
                return this.angledriveTypeField;
            }
            set {
                this.angledriveTypeField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehiclePTO PTO {
            get {
                return this.pTOField;
            }
            set {
                this.pTOField = value;
            }
        }
        
        /// <remarks/>
        public bool VocationalVehicle {
            get {
                return this.vocationalVehicleField;
            }
            set {
                this.vocationalVehicleField = value;
            }
        }
        
        /// <remarks/>
        public bool SleeperCab {
            get {
                return this.sleeperCabField;
            }
            set {
                this.sleeperCabField = value;
            }
        }
        
        /// <remarks/>
        public string NgTankSystem {
            get {
                return this.ngTankSystemField;
            }
            set {
                this.ngTankSystemField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleADAS ADAS {
            get {
                return this.aDASField;
            }
            set {
                this.aDASField = value;
            }
        }
        
        /// <remarks/>
        public bool ZeroEmissionVehicle {
            get {
                return this.zeroEmissionVehicleField;
            }
            set {
                this.zeroEmissionVehicleField = value;
            }
        }
        
        /// <remarks/>
        public bool HybridElectricHDV {
            get {
                return this.hybridElectricHDVField;
            }
            set {
                this.hybridElectricHDVField = value;
            }
        }
        
        /// <remarks/>
        public bool DualFuelVehicle {
            get {
                return this.dualFuelVehicleField;
            }
            set {
                this.dualFuelVehicleField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponents Components {
            get {
                return this.componentsField;
            }
            set {
                this.componentsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehiclePTO {
        
        private string pTOShaftsGearWheelsField;
        
        private string pTOOtherElementsField;
        
        public vehiclesVehiclePTO() {
            this.pTOShaftsGearWheelsField = "none";
            this.pTOOtherElementsField = "none";
        }
        
        /// <remarks/>
        public string PTOShaftsGearWheels {
            get {
                return this.pTOShaftsGearWheelsField;
            }
            set {
                this.pTOShaftsGearWheelsField = value;
            }
        }
        
        /// <remarks/>
        public string PTOOtherElements {
            get {
                return this.pTOOtherElementsField;
            }
            set {
                this.pTOOtherElementsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleADAS {
        
        private bool engineStopStartField;
        
        private bool ecoRollWithoutEngineStopField;
        
        private bool ecoRollWithEngineStopField;
        
        private string predictiveCruiseControlField;
        
        public vehiclesVehicleADAS() {
            this.predictiveCruiseControlField = "none";
        }
        
        /// <remarks/>
        public bool EngineStopStart {
            get {
                return this.engineStopStartField;
            }
            set {
                this.engineStopStartField = value;
            }
        }
        
        /// <remarks/>
        public bool EcoRollWithoutEngineStop {
            get {
                return this.ecoRollWithoutEngineStopField;
            }
            set {
                this.ecoRollWithoutEngineStopField = value;
            }
        }
        
        /// <remarks/>
        public bool EcoRollWithEngineStop {
            get {
                return this.ecoRollWithEngineStopField;
            }
            set {
                this.ecoRollWithEngineStopField = value;
            }
        }
        
        /// <remarks/>
        public string PredictiveCruiseControl {
            get {
                return this.predictiveCruiseControlField;
            }
            set {
                this.predictiveCruiseControlField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponents {
        
        private string gearBoxPDField;
        
        private string axleGearPDField;
        
        private string retarderPDField;
        
        private string torqueConverterPDField;
        
        private vehiclesVehicleComponentsEngine engineField;
        
        private vehiclesVehicleComponentsAirDrag airDragField;
        
        private vehiclesVehicleComponentsAuxiliaries auxiliariesField;
        
        private vehiclesVehicleComponentsAxleWheels axleWheelsField;
        
        /// <remarks/>
        public string GearBoxPD {
            get {
                return this.gearBoxPDField;
            }
            set {
                this.gearBoxPDField = value;
            }
        }
        
        /// <remarks/>
        public string AxleGearPD {
            get {
                return this.axleGearPDField;
            }
            set {
                this.axleGearPDField = value;
            }
        }
        
        /// <remarks/>
        public string RetarderPD {
            get {
                return this.retarderPDField;
            }
            set {
                this.retarderPDField = value;
            }
        }
        
        /// <remarks/>
        public string TorqueConverterPD {
            get {
                return this.torqueConverterPDField;
            }
            set {
                this.torqueConverterPDField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsEngine Engine {
            get {
                return this.engineField;
            }
            set {
                this.engineField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAirDrag AirDrag {
            get {
                return this.airDragField;
            }
            set {
                this.airDragField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliaries Auxiliaries {
            get {
                return this.auxiliariesField;
            }
            set {
                this.auxiliariesField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAxleWheels AxleWheels {
            get {
                return this.axleWheelsField;
            }
            set {
                this.axleWheelsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsEngine {
        
        private string enginePDField;
        
        private string engineStrokeVolumeField;
        
        /// <remarks/>
        public string EnginePD {
            get {
                return this.enginePDField;
            }
            set {
                this.enginePDField = value;
            }
        }
        
        /// <remarks/>
        public string EngineStrokeVolume {
            get {
                return this.engineStrokeVolumeField;
            }
            set {
                this.engineStrokeVolumeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAirDrag {
        
        private string airDragPDField;
        
        private string airDragModelField;
        
        /// <remarks/>
        public string AirDragPD {
            get {
                return this.airDragPDField;
            }
            set {
                this.airDragPDField = value;
            }
        }
        
        /// <remarks/>
        public string AirDragModel {
            get {
                return this.airDragModelField;
            }
            set {
                this.airDragModelField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliaries {
        
        private vehiclesVehicleComponentsAuxiliariesData dataField;
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliariesData Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliariesData {
        
        private vehiclesVehicleComponentsAuxiliariesDataFan fanField;
        
        private vehiclesVehicleComponentsAuxiliariesDataSteeringPump steeringPumpField;
        
        private vehiclesVehicleComponentsAuxiliariesDataElectricSystem electricSystemField;
        
        private vehiclesVehicleComponentsAuxiliariesDataPneumaticSystem pneumaticSystemField;
        
        private vehiclesVehicleComponentsAuxiliariesDataHVAC hVACField;
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliariesDataFan Fan {
            get {
                return this.fanField;
            }
            set {
                this.fanField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliariesDataSteeringPump SteeringPump {
            get {
                return this.steeringPumpField;
            }
            set {
                this.steeringPumpField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliariesDataElectricSystem ElectricSystem {
            get {
                return this.electricSystemField;
            }
            set {
                this.electricSystemField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliariesDataPneumaticSystem PneumaticSystem {
            get {
                return this.pneumaticSystemField;
            }
            set {
                this.pneumaticSystemField = value;
            }
        }
        
        /// <remarks/>
        public vehiclesVehicleComponentsAuxiliariesDataHVAC HVAC {
            get {
                return this.hVACField;
            }
            set {
                this.hVACField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliariesDataFan {
        
        private string technologyField;
        
        /// <remarks/>
        public string Technology {
            get {
                return this.technologyField;
            }
            set {
                this.technologyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliariesDataSteeringPump {
        
        private string technologyField;
        
        /// <remarks/>
        public string Technology {
            get {
                return this.technologyField;
            }
            set {
                this.technologyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliariesDataElectricSystem {
        
        private string technologyField;
        
        /// <remarks/>
        public string Technology {
            get {
                return this.technologyField;
            }
            set {
                this.technologyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliariesDataPneumaticSystem {
        
        private string technologyField;
        
        /// <remarks/>
        public string Technology {
            get {
                return this.technologyField;
            }
            set {
                this.technologyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAuxiliariesDataHVAC {
        
        private string technologyField;
        
        /// <remarks/>
        public string Technology {
            get {
                return this.technologyField;
            }
            set {
                this.technologyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAxleWheels {
        
        private vehiclesVehicleComponentsAxleWheelsData dataField;
        
        /// <remarks/>
        public vehiclesVehicleComponentsAxleWheelsData Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAxleWheelsData {
        
        private vehiclesVehicleComponentsAxleWheelsDataAxle[] axlesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Axle", IsNullable=false)]
        public vehiclesVehicleComponentsAxleWheelsDataAxle[] Axles {
            get {
                return this.axlesField;
            }
            set {
                this.axlesField = value;
            }
        }

	}
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class vehiclesVehicleComponentsAxleWheelsDataAxle {

		public static implicit operator vehiclesVehicleComponentsAxleWheelsDataAxle(Scania.Baseline.vehiclesVehicleComponentsAxleWheelsDataAxle axle)
		{
			var convertedAxle = new vehiclesVehicleComponentsAxleWheelsDataAxle();
			convertedAxle.axleNumber = axle.axleNumber;
			convertedAxle.AxleType = axle.AxleType;
			convertedAxle.TwinTyres = axle.TwinTyres;
			convertedAxle.Steered = axle.Steered;
			convertedAxle.TyrePD = axle.TyrePD;

			return convertedAxle;
		}

		private string axleTypeField;
        
        private uint twinTyresField;
        
        private uint steeredField;
        
        private string tyrePDField;
        
        private uint axleNumberField;
        
        /// <remarks/>
        public string AxleType {
            get {
                return this.axleTypeField;
            }
            set {
                this.axleTypeField = value;
            }
        }
        
        /// <remarks/>
        public uint TwinTyres {
            get {
                return this.twinTyresField;
            }
            set {
                this.twinTyresField = value;
            }
        }
        
        /// <remarks/>
        public uint Steered {
            get {
                return this.steeredField;
            }
            set {
                this.steeredField = value;
            }
        }
        
        /// <remarks/>
        public string TyrePD {
            get {
                return this.tyrePDField;
            }
            set {
                this.tyrePDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint axleNumber {
            get {
                return this.axleNumberField;
            }
            set {
                this.axleNumberField = value;
            }
        }
    }
}
