//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 30.12.2015
//  *******************************************************/

using System;
using System.Linq;
using log4net;
using log4net.Config;
using LayerContainerFacade.Interfaces;
using Mono.Options;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;
using CommonTypes;

namespace MARSLocalStarter {

  public class Program {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

    private static TModelDescription _chosenModel;
    private static ISimulationManagerApplicationCore simCore;
    private static ILayerContainerFacade layerContainerCore;

	private static void Main(string[] args)
	{
		XmlConfigurator.Configure();
		Logger.Info("MARS LIFE trying to start up.");

		try
		{
			Logger.Info("Initializing components and building application core...");




			// parse for any given parameters and act accordingly
			ParseArgsAndStart(args);

			Logger.Info("MARS LIFE up and running...");

			simCore.WaitForSimulationToFinish(_chosenModel);
		}
		catch (Exception exception)
		{
			Logger.FatalFormat("MARS LIFE crashed fatally. Exception:\n {0}.\n InnerException:\n {1}", exception,
			  exception.InnerException);

			//Get log file
			/*var rootAppender = ((Hierarchy)LogManager.GetRepository())
						.Root.Appenders.OfType<FileAppender>()
						.FirstOrDefault();
					var filename = rootAppender != null ? rootAppender.File : string.Empty;*/
			LogManager.Shutdown();

            //Report error to jira
            //JiraErrorReporter.ReportError(filename, exception);

            Console.Error.WriteLine($"MARS LIFE crashed with an error. Error was: {exception.Message}");
            Environment.Exit(1);
            }


		Logger.Info("MARS LIFE shutting down.");

		// This will shutdown the log4net system
		LogManager.Shutdown();
		Environment.Exit(0);
	}


	/// <summary>
	///   Start simulation of a model as defined by launcher arguments.
	///   -h / --help / -? shows quick help
	///   -l / --list lists all available models
	///   -m / --model followed by the name of a model starts specified model
	///   -c / --count specifies the number of ticks to simulate
	///   finally -cli starts an interactive shell to choose a model.
	/// </summary>
	/// <param name="args">Arguments.</param>
	/// <param name="core">Core.</param>
	private static void ParseArgsAndStart(string[] args)
	{
		var help = false;
		var listModels = false;
		var numOfTicksS = "0";
		var modelName = string.Empty;
		var interactive = false;
		var simulationId = Guid.NewGuid();
		var marsConfigAddress = string.Empty;
		var simConfigToUse = String.Empty;
        string clusterName = null;

		var optionSet = new OptionSet()
		  .Add("?|h|help", "Shows short usage", option => help = option != null)
		  .Add("c=|count=", "Specifies number of ticks to simulate",
			option => numOfTicksS = option)
		  .Add("l|list", "List all available models",
			option => listModels = option != null)
		  .Add("m=|model=", "Model to simulate", option => modelName = option)
		  .Add("cli", "Use interactive model chooser",
			option => interactive = option != null)
		  .Add("id=", "Set SimulationID",
			option => simulationId = Guid.Parse(option))
		  .Add("mca=|marsconfigaddress=", "MARSConfig address to use",
			option => marsConfigAddress = option)
		  .Add("simconfig=|scenario=","Name of SimConfig/Scenario file to use. File must be in /layers/addins/<ModelName>/scenarios folder",
		    option => simConfigToUse = option)
          .Add("clustername=|cn=", "Optional. Provide a name for the simulation cluster. Only LIFE process with the same name join each other",
            option => clusterName = option);

		try
		{
			optionSet.Parse(args);
		}
		catch (OptionException)
		{
			ShowHelp("Usage is:", optionSet, true);
		}

		if (help || args.Length == 0)
		{
			ShowHelp("Usage is:", optionSet, false);
		}
		else {

            // initialize basic services
            simCore = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore(clusterName);
            Logger.Info("SimulationManager successfully started.");

            layerContainerCore = LayerContainerApplicationCoreFactory.GetLayerContainerFacade(clusterName);
            Logger.Info("LayerContainer successfully started.");

			if (listModels)
			{
				Console.WriteLine("Available models:");
				var i = 1;
				foreach (var modelDescription in simCore.GetAllModels())
				{
					Console.Write(i + ": ");
					Console.WriteLine(modelDescription.Name);
					i++;
				}
			}
			else if (!modelName.Equals(string.Empty))
			{
				var numOfTicks = 0;
				try
				{
					numOfTicks = Convert.ToInt32(numOfTicksS, 10);
				}
				catch (Exception ex)
				{
					if (ex is OverflowException || ex is FormatException)
					{
						ShowHelp("Please specify tick count as number!", optionSet, true);
					}
					throw;
				}

				TModelDescription model = null;
				foreach (var modelDescription in simCore.GetAllModels())
				{
					if (modelDescription.Name.Equals(modelName))
					{
						model = modelDescription;
					}
				}

				if (model == null)
				{
					ShowHelp("Model " + modelName + " not exists", optionSet, true);
				}
				else {
					_chosenModel = model;
					if (marsConfigAddress != String.Empty) {
						MARSConfigServiceSettings.Address = marsConfigAddress;	
					}
                    var simConfigName = "SimConfig.json";
					if (simConfigToUse != String.Empty)
					{
                        if (!simConfigToUse.EndsWith(".json")){
                            throw new Exception("Format of SimConfig file is not valid. Must be .json file!");
                        }
                        simConfigName = simConfigToUse;                    
					}
                    simCore.StartSimulationWithModel(simulationId, model, numOfTicks, simConfigName);
				}
			}
		}
	}




	private static void ShowHelp(string message, OptionSet optionSet, bool exitWithError) {
      Console.WriteLine(message);
      optionSet.WriteOptionDescriptions(Console.Out);
      if (exitWithError) {
        Environment.Exit(-1);
      }
    }

    /// <summary>
    ///   Shows interactive shell for choosing a model.
    /// </summary>
    /// <param name="core">Core.</param>
    private static void InteractiveModelChoosing(ISimulationManagerApplicationCore core) {
      //Console input requested
      Console.WriteLine(@"Please input the number of the model you'd like to run:");

      // list special option
      Console.WriteLine(@"0: ElephantModel via Download (EXPERIMENTAL)");

      // listing all available models
      var i = 0;
      foreach (var modelDescription in core.GetAllModels()) {
        i++;
        Console.Write(i + ": ");
        Console.WriteLine(modelDescription.Name);
      }


      var nr = 0;
      // read selected model number from console and start it
      nr = int.Parse(Console.ReadLine()) - 1;

      if (nr != -1) {
        while (!Enumerable.Range(0, i).Contains(nr)) {
          Console.WriteLine("Please input an existing model number.");
          nr = int.Parse(Console.ReadLine()) - 1;
        }
      }

      Console.WriteLine("For how many steps is the simulation supposed to run?");
      var ticks = int.Parse(Console.ReadLine());
      if (nr == -1) {}
      else {
        var models = core.GetAllModels().ToList();
        core.StartSimulationWithModel(Guid.NewGuid(), models[nr], ticks);
      }
    }

    private static int GetUnixTimeStamp() {
      return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
  }
}