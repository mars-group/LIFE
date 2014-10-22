using System;
using System.Linq;
using LayerContainerFacade.Interfaces;
using Mono.Options;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;

namespace MARSLocalStarter
{
    public class Program
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
        /// Shows interactive shell for choosing a model.
        /// </summary>
        /// <param name="core">Core.</param>
        private static void InteractiveModelChoosing(ISimulationManagerApplicationCore core)
        {

            //Console input requested
            Console.WriteLine("Please input the number of the model you'd like to run:");

            // list special option
            Console.WriteLine("0: ElephantModel via Download (EXPERIMENTAL)");

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

            if (nr != -1)
            {
                while (!Enumerable.Range(0, i).Contains(nr))
                {
                    Console.WriteLine("Please input an existing model number.");
                    nr = int.Parse(Console.ReadLine()) - 1;
                }
            }

            Console.WriteLine("For how many steps is the simulation supposed to run?");
            int ticks = int.Parse(Console.ReadLine());
            if (nr == -1)
            {
                core.StartSimulationWithModel
                    (new TModelDescription
                        ("ElephantModel",
                            "",
                            "Not Running",
                            false,
                            "http://mc.mars.haw-hamburg.de/modeluploads/test@test.com/ElephantModel.zip"),
                        ticks);
            }
            else {
                var models = core.GetAllModels().ToList();
                core.StartSimulationWithModel(models[nr], ticks);
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



            try {
                Console.WriteLine("Initializing components and building application core...");


                var core = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore();




				var layerCountainerCore = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();



                Console.WriteLine("MARS LIFE up and running. Press 'q' to quit.");

				// parse for any given parameters and act accordingly
                ParseArgsAndStart(args, core);

                ConsoleKeyInfo info = Console.ReadKey();
                while (info.Key != ConsoleKey.Q)
                {
                    info = Console.ReadKey();
                }
            }
            catch (Exception exception)
            {

				throw;
            }


        }
    }
}
