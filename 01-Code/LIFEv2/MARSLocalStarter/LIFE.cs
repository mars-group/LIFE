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

  public static class LIFE {

    private static TModelDescription _chosenModel;
    private static ISimulationManagerApplicationCore simCore;
    private static ILayerContainerFacade layerContainerCore;

	public static void Start(string[] args)
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
			Console.WriteLine($"MARS LIFE crashed fatally. Exception:\n {exception}.\n InnerException:\n {exception.InnerException}");
		    throw exception;
            #Environment.Exit(1);
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
		var modelPath = ".";
		var simulationId = Guid.NewGuid();
		var marsConfigAddress = string.Empty;
		var scenarioConfigToUse = String.Empty;
        string clusterName = null;

		var optionSet = new OptionSet()
		  .Add("?|h|help", "Shows short usage", option => help = option != null)
		  .Add("m=|model=", "Path to the Modelcode to simulate. Defaults to current directory.", option => modelPath = option)
		  .Add("id=", "Set SimulationID (must be a valid UUIDv4!)",
			option => simulationId = Guid.Parse(option))
		  .Add("mca=|marsconfigaddress=", "MARSConfig address to use (DEPRECATED)",
			option => marsConfigAddress = option)
		  .Add("sc=|scenario=","The ID of a ScenarioConfiguration. Mandatory if the simulation requires data from the MARS Cloud!",
		    option => scenarioConfigToUse = option)
          .Add("cn=|clustername=", "Optional. Provide a name for the simulation cluster. Only LIFE process with the same name join each other",
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
             model = simCore.GetModelDescription(modelPath);


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

                Console.WriteLine($"Using ConfigServiceAddress {MARSConfigServiceSettings.Address}");

                simCore.StartSimulationWithModel(simulationId, model, 0, scenarioConfigToUse);
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