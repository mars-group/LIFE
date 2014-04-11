using System;
using System.IO;
using LayerContainerFacade.Interfaces;
using log4net;
using log4net.Config;

namespace LayerContainer {
    public class Program {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args) {
            Logger.Info("LayerContainer trying to startup.");
            var _facade = ApplicationCoreFactory.GetLayerContainerFacade();

            // This will instruct log4net to look for a configuration file
            // called config.log4net in the root directory of the device
            XmlConfigurator.Configure(new FileInfo(@"\config.log4net"));


            Logger.Info("Initializing components and building application core...");

            Console.WriteLine("LayerContainer up and running. Press 'q' to quit.");

            ConsoleKeyInfo info = Console.ReadKey();
            while (info.Key != ConsoleKey.Q)
            {
                info = Console.ReadKey();
            }

            Logger.Info("LayerContainer shutting down.");

            // This will shutdown the log4net system
            LogManager.Shutdown();
        }
    }
}