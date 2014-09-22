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
            Logger.Info("SimulationController trying to startup.");

            // Uncomment the next line to enable log4net internal debugging
            //log4net.Util.LogLog.InternalDebugging = true;

            // This will instruct log4net to look for a configuration file
            // called config.log4net in the root directory of the device
            XmlConfigurator.Configure(new FileInfo(@"\config.log4net"));

            Console.WriteLine("Initializing components and building application core...");

            IApplicationCore core = ApplicationCoreFactory.GetProductionApplicationCore();
            
            Console.WriteLine("SimulationManager up and running. Press 'q' to quit.");

			if(args.Any (x => x.Equals ("-cli"))) {
				//Console input requested
				Console.WriteLine ("Please input the number before the model that will be run.");

				// listing all available models
				var i = 1;
				foreach (var modelDescription in core.GetAllModels()) {
					Console.Write (i + ": ");
					Console.WriteLine(modelDescription.Name);
					i++;
				}

				// read selected model number from console and start it
				int nr = int.Parse (Console.ReadLine ()) - 1;
				Console.WriteLine ("For how many steps is the simukation supposed to run?");
				int ticks = int.Parse (Console.ReadLine ());
				core.StartSimulationWithModel (core.GetAllModels ().ToList ()[nr], ticks);
			}


            ConsoleKeyInfo info = Console.ReadKey();
            while (info.Key != ConsoleKey.Q) {
                info = Console.ReadKey();
            }

            Logger.Info("SimulationController trying to startup.");

            // This will shutdown the log4net system
            LogManager.Shutdown();
        }
    }
}