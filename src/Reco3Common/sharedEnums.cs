using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reco3Common
{
    public class Security_Enums
    {
        public enum UserRole
        {
            [Display(Name = @"Role_Reco3_Pending")]
            Role_Reco3_Pending = 0,
            [Display(Name = @"Role_Reco3_Guest")]
            Role_Reco3_Guest = 1,
            [Display(Name = @"Role_Reco3_Simulator")]
            Role_Reco3_Simulator = 2,
            [Display(Name = @"Role_Reco3_Administrator")]
            Role_Reco3_Administrator = 3,
            [Display(Name = @"Role_Reco3_Unkown")]
            Role_Reco3_Unkown = 100

        };
    }

    public class Reco3_Defines
    {
        public const string PrefixDeclarationNamespace = "tns";
        public const string DeclarationNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
        public const string PrefixEngineeringNamespace = "tns";
        public const string EngineeringNamespace = "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v0.7";

        public const string PrefixEngineeringDSigNamespace = "di";
        public const string EngineeringDSigNamespace = "http://www.w3.org/2000/09/xmldsig#";
        public const string PrefixDeclarationDSigNamespace = "di";
        public const string DeclarationDSigNamespace = "http://www.w3.org/2000/09/xmldsig#";
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()?
                            .GetMember(enumValue.ToString())?
                            .First()?
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name;
        }
    }

    public class Reco3_Enums
    {

        public enum DataConsumer
        {
            [Display(Name = @"Batch")]
            dcBatch = 0,
            [Display(Name = @"Co2Roadmap")]
            dcCo2Roadmap = 1,
            [Display(Name = @"CVV")]
            dcCVV = 2
        }
        public enum VehicleMode
        {
            [Display(Name = @"VectoDeclaration")]
            VectoDeclaration = 0,
            [Display(Name = @"VectoEngineering")]
            VectoEngineering = 1
        }

        public enum ComponentType
        {
            [Display(Name = @"Engine")]
            ctEngine = 1,
            [Display(Name = @"Gearbox")]
            ctGearbox = 2,
            [Display(Name = @"Axle")]
            ctAxle = 3,
            [Display(Name = @"Retarder")]
            ctRetarder = 4,
            [Display(Name = @"Tyre")]
            ctTyre = 5,
            [Display(Name = @"Airdrag")]
            ctAirdrag = 6,
            [Display(Name = @"TorqueConverter")]
            ctTorqueConverter = 7,
            [Display(Name = @"Unknown")]
            ctUnknown = 0
        }

        public enum PDStatus
        {
            [Display(Name = @"Released")]
            ctReleased = 1,
            [Display(Name = @"In Work")]
            ctInWork = 2,
            [Display(Name = @"Unknown")]
            ctUnknown = 0
        }

        public enum PDSource
        {
            [Display(Name = @"3dExperience")]
            ct3dExperience = 1,
            [Display(Name = @"YDMC")]
            ctYDMC = 2,
            [Display(Name = @"Unknown")]
            ctUnknown = 0
        }

        public enum Reco3MsgType
        {
            [Display(Name = @"Unknown")]
            UnknownType = 0,
            [Display(Name = @"PendingExtraction")]
            PendingExtraction = 1,
            [Display(Name = @"PendingValidation")]
            PendingValidation = 2,
            [Display(Name = @"PendingConversion")]
            PendingConversion = 3,
            [Display(Name = @"PendingSimulation")]
            PendingSimulation = 4,
            [Display(Name = @"PushConversion")]
            PushConversion = 5,
            [Display(Name = @"PushHealth")]
            PushHealth = 6,
            [Display(Name = @"UpdateComponentData")]
            UpdateComponentData = 7,
            [Display(Name = @"PendingRoadmapValidation")]
            PendingRoadmapValidation = 10,
            [Display(Name = @"QueueRoadmapSimulation")]
            QueueRoadmapSimulation = 11,
            [Display(Name = @"PendingRoadmapSimulation")]
            PendingRoadmapSimulation = 12
        }

        public enum ValidationStatus
        {
            [Display(Name = @"Pending")]
            Pending = 0,
            [Display(Name = @"Processing")]
            Processing = 1,
            [Display(Name = @"Validated with success")]
            ValidatedWithSuccess = 2,
            [Display(Name = @"Validated with failure")]
            ValidatedWithFailures = 3
        }

        public enum ConvertToVehicleInputStatus
        {
            [Display(Name = @"Pending")]
            Pending = 0,
            [Display(Name = @"Processing")]
            Processing = 1,
            [Display(Name = @"Converted with success")]
            ConvertedWithSuccess = 2,
            [Display(Name = @"Converted with failure")]
            ConvertedWithFailures = 3
        }

        public enum Reco3EventSource
        {
            [Display(Name = @"Unknown")]
            ecUnknown = 0,
            [Display(Name = @"Conversion")]
            ecConversion = 1,
            [Display(Name = @"Validation")]
            ecValidation = 2,
            [Display(Name = @"Simulation")]
            ecSimulation = 3,
            [Display(Name = @"Client")]
            ecClient = 4,
            [Display(Name = @"Backend")]
            ecBackend = 5
        }

        public enum Reco3SubEventSource
        {
            [Display(Name = @"Unknown")]
            secUnknown = 0,
            [Display(Name = @"Conversion")]
            secConversion = 10,
            [Display(Name = @"Validation")]
            secValidation = 20,
            [Display(Name = @"Schema-failure")]
            secSchemaFailure = 21,
            [Display(Name = @"Missing-pd")]
            secMissingPD = 22,
            [Display(Name = @"Simulation")]
            secSimulation = 30,
            [Display(Name = @"Simulation-failure")]
            secSimulationFailed = 31,
            [Display(Name = @"Client")]
            secClient = 40,
            [Display(Name = @"Backend")]
            secBackend = 50
        }
    }
}
