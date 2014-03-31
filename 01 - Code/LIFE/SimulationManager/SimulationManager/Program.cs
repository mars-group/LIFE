
using System;
using SimulationManagerFacade.Interface;

namespace SimulationManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing components and building application core...");

            IApplicationCore core = ApplicationCoreFactory.GetProductionApplicationCore();

            Console.WriteLine("SimulationManager up and running. Press 'q' to quit.");

            ConsoleKeyInfo info = Console.ReadKey();
            while (info.Key != ConsoleKey.Q)
            {
                info = Console.ReadKey();
            }
        }
    }
}
