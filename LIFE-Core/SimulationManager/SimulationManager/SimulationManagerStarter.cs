//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using SimulationManagerFacade.Interface;
using Mono.Options;

namespace SimulationManager
{
    internal class SimulationManagerStarter
    {
        private static void ShowHelp(String message, OptionSet optionSet, bool exitWithError)
        {
            Console.WriteLine(message);
            optionSet.WriteOptionDescriptions(Console.Out);
            if (exitWithError)
            {
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Start simulation of a model as defined by launcher arguments.
        /// -h / --help / -? shows quick help
        /// -l / --list lists all available models
        /// -m / --model followed by the name of a model starts specified model
        /// -c / --count specifies the number of ticks to simulate
        /// finally -cli starts an interactive shell to choose a model.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <param name="core">Core.</param>
        private static void ParseArgsAndStart(string[] args, ISimulationManagerApplicationCore core)
        {
            bool help = false;
            bool listModels = false;
            int numOfTicks = 0;
            string numOfTicksS = "0";
            string modelPath = "";
            bool interactive = false;

            OptionSet optionSet = new OptionSet()
                .Add("?|h|help", "Shows short usage", option => help = option != null)
                .Add("c=|count=", "Specifies number of ticks to simulate",
                                      option => numOfTicksS = option)
                .Add("l|list", "List all available models",
                                      option => listModels = option != null)
                .Add("m=|model=", "Model to simulate", option => modelPath = option)
                .Add("cli", "Use interactive model chooser",
                                      option => interactive = option != null);

            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException)
            {
                ShowHelp("Usage is:", optionSet, true);
            }

            if (help)
            {
                ShowHelp("Usage is:", optionSet, false);
                Environment.Exit(0);
            }
            else
            {
                if (!modelPath.Equals(""))
                {
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

                    SMConnector.TransportTypes.TModelDescription model = null;

                    model = core.GetModelDescription(modelPath);

                    if (model == null)
                    {
                        ShowHelp("Model " + modelPath + " not exists", optionSet, true);
                    }
                    else
                    {
                        core.StartSimulationWithModel(Guid.NewGuid(), model, numOfTicks);
                    }
                }
            }
        }

        private static void Main(string[] args)
        {

            Console.WriteLine("SimulationManager trying to start up.");

            try
            {
                Console.WriteLine("Initializing components and building application core...");

                ISimulationManagerApplicationCore core = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore();

                Console.WriteLine("SimulationManager successfully started.");

                Console.WriteLine("SimulationManager up and running. Press 'q' to quit.");

                ParseArgsAndStart(args, core);

                ConsoleKeyInfo info = Console.ReadKey();
                while (info.Key != ConsoleKey.Q)
                {
                    info = Console.ReadKey();
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("SimulationManager crashed fatally. Exception:\n {0}", exception);
				throw;
            }


           Console.WriteLine("SimulationController shutting down.");
        }
    }
}