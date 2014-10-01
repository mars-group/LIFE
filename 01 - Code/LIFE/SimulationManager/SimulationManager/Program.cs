using System;
using System.IO;
using log4net;
using log4net.Config;
using SimulationManagerFacade.Interface;
using System.Linq;

namespace SimulationManager {
    internal class Program {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args) {
          
            Logger.Info("SimulationManager trying to start up.");

            try {
                Console.WriteLine("Initializing components and building application core...");

                IApplicationCore core = ApplicationCoreFactory.GetProductionApplicationCore();

                Logger.Info("SimulationManager successfully started.");

                Console.WriteLine("SimulationManager up and running. Press 'q' to quit.");

                if (args.Any(x => x.Equals("-cli"))) {
                    //Console input requested
                    Console.WriteLine("Please input the number before the model that will be run.");

                    // listing all available models
                    var i = 1;
                    foreach (var modelDescription in core.GetAllModels()) {
                        Console.Write(i + ": ");
                        Console.WriteLine(modelDescription.Name);
                        i++;
                    }

                    // read selected model number from console and start it
                    int nr = int.Parse(Console.ReadLine()) - 1;
                    Console.WriteLine("For how many steps is the simukation supposed to run?");
                    int ticks = int.Parse(Console.ReadLine());
                    core.StartSimulationWithModel(core.GetAllModels().ToList()[nr], ticks);
                }


                ConsoleKeyInfo info = Console.ReadKey();
                while (info.Key != ConsoleKey.Q) {
                    info = Console.ReadKey();
                }
            }
            catch(Exception exception) {
                Logger.Fatal("SimulationManager crashed fatally. Exception:\n {0}", exception);
            }
            

            Logger.Info("SimulationController shuttign down.");

            // This will shutdown the log4net system
            LogManager.Shutdown();
        }
    }
}