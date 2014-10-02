using System;
using System.IO;
using log4net;
using log4net.Config;
using SimulationManagerFacade.Interface;
using System.Linq;
using Mono.Options;

namespace SimulationManager
{
    internal class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

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
        /// Shows interactive shell for choosing a model.
        /// </summary>
        /// <param name="core">Core.</param>
        private static void InteractiveModelChoosing(IApplicationCore core)
        {

            //Console input requested
            Console.WriteLine("Please input the number before the model that will be run.");

            // listing all available models
            var i = 0;
            foreach (var modelDescription in core.GetAllModels())
            {
                i++;
                Console.Write(i + ": ");
                Console.WriteLine(modelDescription.Name);
            }

            int nr = 0;
            // read selected model number from console and start it
            nr = int.Parse(Console.ReadLine()) - 1;

            while (!Enumerable.Range(0, i).Contains(nr))
            {
                Console.WriteLine("Please input an existing model number.");
                nr = int.Parse(Console.ReadLine()) - 1;
            }

            Console.WriteLine("For how many steps is the simulation supposed to run?");
            int ticks = int.Parse(Console.ReadLine());
            core.StartSimulationWithModel(core.GetAllModels().ToList()[nr], ticks);
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
        private static void ParseArgsAndStart(string[] args, IApplicationCore core)
        {
            bool help = false;
            bool listModels = false;
            int numOfTicks = 0;
            string numOfTicksS = "0";
            string modelName = "";
            bool interactive = false;

            OptionSet optionSet = new OptionSet()
                .Add("?|h|help", "Shows short usage", option => help = option != null)
                .Add("c=|count=", "Specifies number of ticks to simulate",
                                      option => numOfTicksS = option)
                .Add("l|list", "List all available models",
                                      option => listModels = option != null)
                .Add("m=|model=", "Model to simulate", option => modelName = option)
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
                if (listModels)
                {
                    Console.WriteLine("Available models:");
                    var i = 1;
                    foreach (var modelDescription in core.GetAllModels())
                    {
                        Console.Write(i + ": ");
                        Console.WriteLine(modelDescription.Name);
                        i++;
                    }
                }
                else if (interactive)
                {
                    InteractiveModelChoosing(core);
                }
                else if (!modelName.Equals(""))
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
                    foreach (var modelDescription in core.GetAllModels())
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
                    else
                    {
                        core.StartSimulationWithModel(model, numOfTicks);
                    }
                }
            }
        }

        private static void Main(string[] args)
        {

            Logger.Info("SimulationManager trying to start up.");

            try
            {
                Console.WriteLine("Initializing components and building application core...");

                IApplicationCore core = ApplicationCoreFactory.GetProductionApplicationCore();

                Logger.Info("SimulationManager successfully started.");

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
                Logger.Fatal("SimulationManager crashed fatally. Exception:\n {0}", exception);
				throw;
            }


            Logger.Info("SimulationController shutting down.");

            // This will shutdown the log4net system
            LogManager.Shutdown();
        }
    }
}