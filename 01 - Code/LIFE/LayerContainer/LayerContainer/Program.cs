using System;
using LayerContainerFacade.Interfaces;

namespace LayerContainer {
    public class Program {
        private static void Main(string[] args) {
            var _facade = ApplicationCoreFactory.GetLayerContainerFacade();

            Console.ReadLine();
        }
    }
}