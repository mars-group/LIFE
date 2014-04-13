using System;
using System.IO;
using log4net;
using log4net.Config;
using SimulationManagerFacade.Interface;

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