//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 30.12.2015
//  *******************************************************/

using System;
using LayerContainerFacade.Interfaces;
using Mono.Options;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;
using CommonTypes;

namespace MARSLocalStarter {

  public class Program {

    private static TModelDescription _chosenModel;
    private static ISimulationManagerApplicationCore simCore;
    private static ILayerContainerFacade layerContainerCore;

	private static void Main(string[] args)
	{

		Console.WriteLine("MARS LIFE 2.0 trying to start up.");

		try
		{
			Console.WriteLine("Initializing components and building application core...");

			// parse for any given parameters and act accordingly
			ParseArgsAndStart(args);

			Console.WriteLine("MARS LIFE 2.0 up and running...");

			simCore.WaitForSimulationToFinish(_chosenModel);
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"MARS LIFE crashed fatally. Exception:\n {exception}.\n InnerException:\n {exception.InnerException}");
            Environment.Exit(1);
        }


		Console.WriteLine("MARS LIFE 2.0 shutting down.");


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
		var numOfTicksS = "0";
		var modelPath = ".";
		var interactive = false;
		var simulationId = Guid.NewGuid();
		var marsConfigAddress = string.Empty;
		var simConfigToUse = String.Empty;
        string clusterName = null;

		var optionSet = new OptionSet()
		  .Add("?|h|help", "Shows short usage", option => help = option != null)
		  .Add("m=|model=", "Path to the Modelcode to simulate. Defaults to current directory.", option => modelPath = option)
		  .Add("id=", "Set SimulationID",
			option => simulationId = Guid.Parse(option))
		  .Add("mca=|marsconfigaddress=", "MARSConfig address to use",
			option => marsConfigAddress = option)
		  .Add("simconfig=|scenario=","Name of SimConfig/Scenario file to use. File must be in <ModelPath>/scenarios folder",
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
		else
		{
            // Initialize basic services
            simCore = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore(clusterName);
            Console.WriteLine("SimulationManager successfully started.");

            layerContainerCore = LayerContainerApplicationCoreFactory.GetLayerContainerFacade(clusterName);
            Console.WriteLine("LayerContainer successfully started.");


            TModelDescription model = null;
            foreach (var modelDescription in simCore.GetAllModels())
            {
                if (modelDescription.Name.Equals(modelPath))
                {
                    model = modelDescription;
                }
            }

            if (model == null)
            {
                ShowHelp("Model " + modelPath + " not exists", optionSet, true);
            }
            else
            {
                _chosenModel = model;
                if (marsConfigAddress != string.Empty)
                {
                    MARSConfigServiceSettings.Address = marsConfigAddress;
                }
                else
                {
                    throw new Exception("Please specify a MARS Config Service Address, which is valid in your context!");
                }
                var simConfigName = "SimConfig.json";
                if (simConfigToUse != string.Empty)
                {
                    if (!simConfigToUse.EndsWith(".json")){
                        throw new Exception("Format of SimConfig file is not valid. Must be .json file!");
                    }
                    simConfigName = simConfigToUse;
                }
                simCore.StartSimulationWithModel(simulationId, model, 0, simConfigName);
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

  }
}