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
            try {

                Logger.Info("Initializing components and building application core...");

                var _facade = ApplicationCoreFactory.GetLayerContainerFacade();

                Logger.Info("LayerContainer successfully started.");

                Console.WriteLine("LayerContainer up and running. Press 'q' to quit.");

                ConsoleKeyInfo info = Console.ReadKey();
                while (info.Key != ConsoleKey.Q)
                {
                    info = Console.ReadKey();
                }
            }
            catch (Exception exception)
            {
                Logger.Fatal("LayerContainer crashed fatally. Exception:\n {0}", exception);
            }

            Logger.Info("LayerContainer shutting down.");

            // This will shutdown the log4net system
            LogManager.Shutdown();
        }
    }
}