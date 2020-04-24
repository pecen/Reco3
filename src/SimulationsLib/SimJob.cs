
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TUGraz.VectoAPI;
using static TUGraz.VectoCore.Models.Simulation.Impl.JobContainer;

namespace SimulationsLib
{
    public class SimJob
    {
        public enum VectoStatus
        {
            enPending = 0,
            enProcessing,
            enFinishedWithSuccess,
            enFinishedWithFailure,
            enAborted
        }

        protected VectoStatus m_vStatus;
        public VectoStatus Status { get { return m_vStatus; } set { m_vStatus = value; } }

        protected IVectoApiRun m_IVectoApiRun;
        public IVectoApiRun VectoApiRun { get { return m_IVectoApiRun; } set { m_IVectoApiRun = value; } }
        

        public SimJob(IVectoApiRun vecto)
        {
            m_vStatus = VectoStatus.enPending;
            m_IVectoApiRun = vecto;
        }

        public bool ProcessData(XmlReader reader)
        {
            try
            {
                VectoApiRun = VectoApi.VectoInstance(reader);

                VectoApiRun.WaitFinished = false;  // RunSimulation is non-blocking!
                VectoApiRun.RunSimulation();
                m_vStatus = VectoStatus.enProcessing;
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool WriteVSum(string strFilename, string strCurrentDateTimeFormatted)
        {
            try
            {
                // # VECTO 3.2.1.1133 - 16.03.2018 13:45
                // Job[-],Input File[-], Cycle[-], Status, vehicle manufacturer[-],VIN number, vehicle model[-],HDV CO2 vehicle class [-],Corrected Actual Curb Mass[kg], Loading[kg], Total vehicle mass[kg], Engine manufacturer[-],Engine model[-], Engine fuel type[-], Engine rated power[kW], Engine idling speed[rpm], Engine rated speed[rpm], Engine displacement[ccm],Engine WHTCUrban, Engine WHTCRural,Engine WHTCMotorway, Engine BFColdHot,Engine CFRegPer, Engine actual CF, Declared CdxA[m²],CdxA[m²],total RRC[-], weighted RRC w/o trailer[-], r_dyn[m], Number axles vehicle driven[-],Number axles vehicle non-driven[-],Number axles trailer[-],Gearbox manufacturer[-], Gearbox model[-],Gearbox type[-], Gear ratio first gear[-],Gear ratio last gear[-], Torque converter manufacturer[-], Torque converter model[-], Retarder manufacturer[-],Retarder model[-], Retarder type[-],Angledrive manufacturer[-], Angledrive model[-],Angledrive ratio[-], Axle manufacturer[-],Axle model[-], Axle gear ratio[-], Auxiliary technology STP[-], Auxiliary technology FAN[-], Auxiliary technology AC[-], Auxiliary technology PS[-], Auxiliary technology ES[-], Cargo Volume[m³],time[s],distance[km],speed[km / h],altitudeDelta[m],FC-Map[g / h],FC-Map[g / km],FC-AUXc[g / h],FC-AUXc[g / km],FC-WHTCc[g / h],FC-WHTCc[g / km],FC-AAUX[g / h],FC-AAUX[g / km],FC-Final[g / h],FC-Final[g / km],FC-Final[l / 100km],FC-Final[l / 100tkm],FC-Final[l / 100m³km],CO2[g / km],CO2[g / tkm],CO2[g / m³km],P_wheel_in_pos[kW],P_fcmap_pos[kW],E_fcmap_pos[kWh],E_fcmap_neg[kWh],E_powertrain_inertia[kWh],E_aux_FAN[kWh],E_aux_STP[kWh],E_aux_AC[kWh],E_aux_PS[kWh],E_aux_ES[kWh],E_PTO_TRANSM[kWh],E_PTO_CONSUM[kWh],E_aux_sum[kWh],E_clutch_loss[kWh],E_tc_loss[kWh],E_shift_loss[kWh],E_gbx_loss[kWh],E_ret_loss[kWh],E_angle_loss[kWh],E_axl_loss[kWh],E_brake[kWh],E_vehi_inertia[kWh],E_air[kWh],E_roll[kWh],E_grad[kWh],a[m / s ^ 2],a_pos[m / s ^ 2],a_neg[m / s ^ 2],AccelerationTimeShare[%],DecelerationTimeShare[%],CruiseTimeShare[%],max.speed[km / h],max.acc[m / s²],max.dec[m / s²],n_eng_avg[rpm],n_eng_max[rpm],gear shifts[-], StopTimeShare[%], Engine max.Load time share[%],CoastingTimeShare[%],BrakingTImeShare[%],Gear 0 TimeShare[%],Gear 1 TimeShare[%],Gear 2 TimeShare[%],Gear 3 TimeShare[%],Gear 4 TimeShare[%],Gear 5 TimeShare[%],Gear 6 TimeShare[%]
                // 1 - 0,VEH - 2139673,LongHaul.vdri,Success,Scania,VEH - 2139673,6.G 320 B6x2 * 4NA,9,8116,2600,18316,Scania,DC09_126,Diesel CI,239,600,1900,9290,1.0647,1.0272,1.0038,1.0038,1,1.0143388962,5.32,6.82,0.00625248387802151,0.00638859570896161,0.522314692238982,1,2,2,Allison,GA766_std_val,ATSerial,3.49,0.65,,,n.a.,n.a.,None,n.a.,n.a.,n.a.,SCANIA CV AB,R780,5.25,Fixed displacement, Crankshaft mounted - Electronically controlled visco clutch,Default,Large Supply +mech.clutch,"Standard technology - LED headlights, all",101.4,4543.537534602,100.185,79.379997909843,-2.54939404748258,21516.8503054207,271.061361451014,21516.8503054207,271.061361451014,21825.3781885012,274.948082176693,21825.3781885012,274.948082176693,21825.3781885012,274.948082176693,32.8885265761594,12.6494332985229,0.324344443551868,860.587497213049,330.995191235788,8.48705618553302,86.5995704037625,108.69417862619,137.182244550231,1.33103509758995,1.05476121663944E-17,0.779973943440008,0.908707506920399,0.441732815864083,1.00967500768933,2.0734397479331,,,5.21352902184779,0,0.173624198766047,0.0981401049656224,16.7135958958999,0,0,7.24731733700971,6.32194498240692,1.0034886277784E-10,69.0519255081191,31.2502098017368,-0.120937281722657,4.63810939303296E-12,0.469378790402199,-0.48365404709372,4.20483981950087,4.0261948001319,90.2943434698018,85.5339473136944,1,0.993500147005175,1406.24109914567,1592.19699752029,37,1.47462191056531,6.51027116454176,1.05857519455007,1.93467075436221,1.70114945262873,0.18237457575954,1.12774039202251,0.416504150892854,1.61800419857325,0.946549502037951,94.0076777280851

                using (StreamWriter w = File.AppendText(string.Format(strFilename, Helper.GetOutputPath())))
                {
                    string strVSumHeader = string.Format("# VECTO 3.2.1.1133 - {0}", strCurrentDateTimeFormatted);
                    w.WriteLine(strVSumHeader);
                    foreach (string str in VectoApiRun.SumEntries)
                    {
                        w.WriteLine(str);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public string CreateVSumBlob(string strCurrentDateTimeFormatted)
        {
            try
            {
                // # VECTO 3.2.1.1133 - 16.03.2018 13:45
                // Job[-],Input File[-], Cycle[-], Status, vehicle manufacturer[-],VIN number, vehicle model[-],HDV CO2 vehicle class [-],Corrected Actual Curb Mass[kg], Loading[kg], Total vehicle mass[kg], Engine manufacturer[-],Engine model[-], Engine fuel type[-], Engine rated power[kW], Engine idling speed[rpm], Engine rated speed[rpm], Engine displacement[ccm],Engine WHTCUrban, Engine WHTCRural,Engine WHTCMotorway, Engine BFColdHot,Engine CFRegPer, Engine actual CF, Declared CdxA[m²],CdxA[m²],total RRC[-], weighted RRC w/o trailer[-], r_dyn[m], Number axles vehicle driven[-],Number axles vehicle non-driven[-],Number axles trailer[-],Gearbox manufacturer[-], Gearbox model[-],Gearbox type[-], Gear ratio first gear[-],Gear ratio last gear[-], Torque converter manufacturer[-], Torque converter model[-], Retarder manufacturer[-],Retarder model[-], Retarder type[-],Angledrive manufacturer[-], Angledrive model[-],Angledrive ratio[-], Axle manufacturer[-],Axle model[-], Axle gear ratio[-], Auxiliary technology STP[-], Auxiliary technology FAN[-], Auxiliary technology AC[-], Auxiliary technology PS[-], Auxiliary technology ES[-], Cargo Volume[m³],time[s],distance[km],speed[km / h],altitudeDelta[m],FC-Map[g / h],FC-Map[g / km],FC-AUXc[g / h],FC-AUXc[g / km],FC-WHTCc[g / h],FC-WHTCc[g / km],FC-AAUX[g / h],FC-AAUX[g / km],FC-Final[g / h],FC-Final[g / km],FC-Final[l / 100km],FC-Final[l / 100tkm],FC-Final[l / 100m³km],CO2[g / km],CO2[g / tkm],CO2[g / m³km],P_wheel_in_pos[kW],P_fcmap_pos[kW],E_fcmap_pos[kWh],E_fcmap_neg[kWh],E_powertrain_inertia[kWh],E_aux_FAN[kWh],E_aux_STP[kWh],E_aux_AC[kWh],E_aux_PS[kWh],E_aux_ES[kWh],E_PTO_TRANSM[kWh],E_PTO_CONSUM[kWh],E_aux_sum[kWh],E_clutch_loss[kWh],E_tc_loss[kWh],E_shift_loss[kWh],E_gbx_loss[kWh],E_ret_loss[kWh],E_angle_loss[kWh],E_axl_loss[kWh],E_brake[kWh],E_vehi_inertia[kWh],E_air[kWh],E_roll[kWh],E_grad[kWh],a[m / s ^ 2],a_pos[m / s ^ 2],a_neg[m / s ^ 2],AccelerationTimeShare[%],DecelerationTimeShare[%],CruiseTimeShare[%],max.speed[km / h],max.acc[m / s²],max.dec[m / s²],n_eng_avg[rpm],n_eng_max[rpm],gear shifts[-], StopTimeShare[%], Engine max.Load time share[%],CoastingTimeShare[%],BrakingTImeShare[%],Gear 0 TimeShare[%],Gear 1 TimeShare[%],Gear 2 TimeShare[%],Gear 3 TimeShare[%],Gear 4 TimeShare[%],Gear 5 TimeShare[%],Gear 6 TimeShare[%]
                // 1 - 0,VEH - 2139673,LongHaul.vdri,Success,Scania,VEH - 2139673,6.G 320 B6x2 * 4NA,9,8116,2600,18316,Scania,DC09_126,Diesel CI,239,600,1900,9290,1.0647,1.0272,1.0038,1.0038,1,1.0143388962,5.32,6.82,0.00625248387802151,0.00638859570896161,0.522314692238982,1,2,2,Allison,GA766_std_val,ATSerial,3.49,0.65,,,n.a.,n.a.,None,n.a.,n.a.,n.a.,SCANIA CV AB,R780,5.25,Fixed displacement, Crankshaft mounted - Electronically controlled visco clutch,Default,Large Supply +mech.clutch,"Standard technology - LED headlights, all",101.4,4543.537534602,100.185,79.379997909843,-2.54939404748258,21516.8503054207,271.061361451014,21516.8503054207,271.061361451014,21825.3781885012,274.948082176693,21825.3781885012,274.948082176693,21825.3781885012,274.948082176693,32.8885265761594,12.6494332985229,0.324344443551868,860.587497213049,330.995191235788,8.48705618553302,86.5995704037625,108.69417862619,137.182244550231,1.33103509758995,1.05476121663944E-17,0.779973943440008,0.908707506920399,0.441732815864083,1.00967500768933,2.0734397479331,,,5.21352902184779,0,0.173624198766047,0.0981401049656224,16.7135958958999,0,0,7.24731733700971,6.32194498240692,1.0034886277784E-10,69.0519255081191,31.2502098017368,-0.120937281722657,4.63810939303296E-12,0.469378790402199,-0.48365404709372,4.20483981950087,4.0261948001319,90.2943434698018,85.5339473136944,1,0.993500147005175,1406.24109914567,1592.19699752029,37,1.47462191056531,6.51027116454176,1.05857519455007,1.93467075436221,1.70114945262873,0.18237457575954,1.12774039202251,0.416504150892854,1.61800419857325,0.946549502037951,94.0076777280851
                StringBuilder builder = new StringBuilder();
                string strVSumHeader = string.Format("# VECTO 3.2.1.1133 - {0}", strCurrentDateTimeFormatted);
                builder.AppendLine(strVSumHeader);
                foreach (string str in VectoApiRun.SumEntries)
                {
                    builder.AppendLine(str);
                }

                return builder.ToString();
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        public bool SaveReportesToDisc(string strCustomerReportXML, string strManufacturerReportXML)
        {
            try
            {
                if (strCustomerReportXML.Length>0)
                    VectoApiRun.XMLCustomerReport.Save(strCustomerReportXML);
                if (strManufacturerReportXML.Length > 0)
                    VectoApiRun.XMLManufacturerReport.Save(strManufacturerReportXML);
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool ContainsAbortedCycles()
        {
            try
            {
                foreach (string str in VectoApiRun.SumEntries)
                {
                    // Job [-],Input File [-],Cycle [-],Status,......
                    CSVHelper csv = new CSVHelper(str);
                    string strStatus = csv[0][3];
                    if (true == strStatus.Contains("Aborted"))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
