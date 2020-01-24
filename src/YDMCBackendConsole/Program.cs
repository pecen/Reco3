
using _3DX;
using BatchQueue;
using DataLayer.Database;
using SimulationsLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reco3Common;

namespace YDMCBackendConsole
{
    class Program
    {

        static void Main(string[] args)
        {


             /*
            DatabaseContext dbx = new DatabaseContext();

            

            ThreeDExperience exp = new ThreeDExperience();
            exp.Reset();
            DateTime dtThen = DateTime.Now;
            exp.AddPDNum("2808389");
            List<Reco3Component> components = exp.Query();
            dbx.Reco3Components.AddRange(components);
            dbx.SaveChanges();
           
            exp.AddPDNum("2743387");
            exp.AddPDNum("2743590");
            exp.AddPDNum("2743609");
            exp.AddPDNum("2743633");
            exp.AddPDNum("2744987");
            exp.AddPDNum("2765794");
            exp.AddPDNum("2765815");
            exp.AddPDNum("2765816");
            exp.AddPDNum("2765831");
            
            DateTime dtNow = DateTime.Now;
            var diffInSeconds = (dtNow-dtThen).TotalMilliseconds;
            exp.Reset();

            exp.AddPDNum("2766389");
            exp.AddPDNum("2766390");
            exp.AddPDNum("2766391");
            exp.AddPDNum("2766392");
            exp.AddPDNum("2766393");
            exp.AddPDNum("2766394");
            exp.AddPDNum("2766425");
            exp.AddPDNum("2766426");
            exp.AddPDNum("2766427");
            exp.AddPDNum("2766428");
            components = exp.Query();
            dbx.Reco3Components.AddRange(components);
            exp.Reset();

            exp.AddPDNum("2766429");
            exp.AddPDNum("2766430");
            exp.AddPDNum("2766431");
            exp.AddPDNum("2766432");
            exp.AddPDNum("2766433");
            exp.AddPDNum("2766434");
            exp.AddPDNum("2766435");
            exp.AddPDNum("2766436");
            exp.AddPDNum("2766437");
            exp.AddPDNum("2766438");
            components = exp.Query();
            exp.Reset();

            exp.AddPDNum("2766439");
            exp.AddPDNum("2766440");
            exp.AddPDNum("2766441");
            exp.AddPDNum("2766442");
            exp.AddPDNum("2766443");
            exp.AddPDNum("2766444");
            exp.AddPDNum("2766445");
            exp.AddPDNum("2766446");
            exp.AddPDNum("2766447");
            exp.AddPDNum("2766448");
            exp.AddPDNum("2766575");

            components = exp.Query();
            exp.Reset();
            */






            //            BatchQueue.BatchQueue SimulationQueue = new BatchQueue.BatchQueue();
            //          SimulationQueue.SetEndpoint("RD0045111", "YDMC.Batch.Simulation");


            string strMsmqHost = ConfigurationManager.AppSettings.Get("MsmqHost");
            string strMSMQConversionQueue = ConfigurationManager.AppSettings.Get("MSMQConversionQueue");
            string strMSMQHealthQueue = ConfigurationManager.AppSettings.Get("MsmqHealthQueueName");

            BatchQueue.BatchQueue ConversionQueue = new BatchQueue.BatchQueue();
            ConversionQueue.SetEndpoint(strMsmqHost, strMSMQConversionQueue, false, true);

            BatchQueue.BatchQueue ClientHealthQueue = new BatchQueue.BatchQueue();
            ClientHealthQueue.SetEndpoint(strMsmqHost, strMSMQHealthQueue, false, true);

            Thread.Sleep(100);

            ConversionQueue.SendMsg(new Reco3Msg(Reco3_Enums.Reco3MsgType.UpdateComponentData, 0, 0));
            
            //ConversionQueue.SendMsg(new Reco3Msg(Reco3Msg.Reco3MsgType.PendingConversion, 1));

            /*
            
            ConversionQueue.SendMsg(new Reco3Msg(Reco3Msg.Reco3MsgType.PendingSimulation, -1, 2, 1));
            ConversionQueue.SendMsg(new Reco3Msg(Reco3Msg.Reco3MsgType.PendingSimulation, -1, 3, 1));
            ConversionQueue.SendMsg(new Reco3Msg(Reco3Msg.Reco3MsgType.PendingSimulation, -1, 4, 1));
            */

            Helper.ToConsole("Press Enter to quit");
            Helper.GetUserResponse();
        }
    }
}
