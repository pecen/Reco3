
BASIC USAGE

The Vecto API can be used in the following way:

	var data = XmlReader.Create("vecto_vehicle-sample.xml");
	var run = VectoApi.VectoInstance(data);
	run.RunSimulation();
	
The method 'RunSimulation' is by default blocking (i.e. the simulation
is finished when it returns). However, by setting the 'WaitFinished' 
property to false, the 'RunSimulation' method is non-blocking, i.e., 
multiple simulations can be run in parallel. In this case the user has 
to make sure the simulation is finished before accessing the results, 
for example:

	var data1 = XmlReader.Create("vecto_vehicle-sample.xml");
	var data2 = XmlReader.Create("vecto_vehicle-sample.xml");
	var run1 = VectoApi.VectoInstance(data1);
	var run2 = VectoApi.VectoInstance(data2);

	run1.WaitFinished = false;	// RunSimulation is non-blocking!
	run2.WaitFinished = false;	// RunSimulation is non-blocking!

	run1.RunSimulation();
	run2.RunSimulation();

	while (!(run1.IsFinished && run2.IsFinished)) {
		Thread.Sleep(100);
	}
	
RunSimulation by itself uses multiple threads to simulate the different 
cycles and loadings contained in a job in parallel.

REPORT AND SUM DATA

After the simulation of a job is finished, both XML reports as well as the 
summary data entries are available as XDocuments:

    run.XMLCustomerReport
	run.XMLManufacturerReport
	
The entries written to the .vsum file are available as strings (CSV) in run.SumEntries.
For DG Clima 2016 Baseline Fleet simulations it is IMPORTANT to collect and submit
the SumEntries!


EXCEPTION HANDLING

If during the simulation an error occurs an exception is thrown. There are two 
possibilities that the caller gets notified about potential exceptions.

Option 1: blocking call of RunSimulation()
  an AggregateException may be thrown when all simulations are finished. 

Option 2: non-blocking call of RunSimulation() (WaitFinished set to false)
  an AggregateException is thrown when calling WaitSimulationFinished()
  
  e.g.:
  
  	run1.WaitFinished = false;	// RunSimulation is non-blocking!
	run2.WaitFinished = false;	// RunSimulation is non-blocking!

	run1.RunSimulation();
	run2.RunSimulation();

	while (!(run1.IsFinished && run2.IsFinished)) {
		Thread.Sleep(100);
	}
	run1.WaitSimulationFinished()
	run2.WaitSimulationFinished()

Option 3: non-blocking call of RunSimulation() WITHOUT calling WaitSimulationFinished()
  In his case no exception is thrown and the user has to check for exceptions:
  
  e.g.:
  
    run1.WaitFinished = false;	// RunSimulation is non-blocking!
	run1.RunSimulation();

	while (!run1.IsFinished) {
		Thread.Sleep(100);
	}
	var progress = run1.GetProgress();
	progress.

An AggregateException is thrown when the starting thread joins its sub-threads. Hence
this exception is only thrown when using the blocking call or using WaitSimulationFinished()
  
For more details how to handle the AggregateException please see 
https://msdn.microsoft.com/de-de/library/system.aggregateexception(v=vs.110).aspx

