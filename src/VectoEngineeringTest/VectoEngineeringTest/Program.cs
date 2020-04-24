using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace VectoEngineeringTest
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				JobContainer _jobContainer;

				string strFile = "E:\\Source\\XMLStuff\\Vecto\\EngineeringMode\\vehicle.xml";
				IInputDataProvider dataProvider = null;
				var xDocument = XDocument.Load(strFile);
				var rootNode = xDocument == null ? "" : xDocument.Root.Name.LocalName;
				switch (rootNode)
				{
					case "VectoInputEngineering":
						dataProvider = new XMLEngineeringInputDataProvider(strFile, true);
						break;
					case "VectoInputDeclaration":
						dataProvider = new XMLDeclarationInputDataProvider(XmlReader.Create(strFile), true);
						break;
				}
				var mode = ExecutionMode.Declaration;
				mode = ExecutionMode.Engineering;

				var fileWriter = new FileOutputWriter(strFile);
				var sumWriter = new SummaryDataContainer(fileWriter);
				_jobContainer = new JobContainer(sumWriter);

				var runsFactory = new SimulatorFactory(mode, dataProvider, fileWriter)
				{
					ModalResults1Hz = false, 
					WriteModalResults = false, 
					ActualModalData = false, 
					Validate = false, 
				};
				_jobContainer.AddRuns(runsFactory);

				bool _debugEnabled = false;
				_jobContainer.Execute(!_debugEnabled);

				while (!_jobContainer.AllCompleted)
				{
					//PrintProgress(_jobContainer.GetProgress());
					Thread.Sleep(100);
				}

				int n = 0;
			}
			catch (Exception ex)
			{

			}
		}
	}
}
