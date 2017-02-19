//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using LayerContainerFacade.Interfaces;

namespace LayerContainer {
    public class LayerContainerStarter {



        private static void Main(string[] args) {

                Console.WriteLine("LayerContainer trying to startup.");
                try {
                    Console.WriteLine("Initializing components and building application core...");

                    ILayerContainerFacade _facade = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();

                    Console.WriteLine("LayerContainer successfully started.");

                    Console.WriteLine("LayerContainer up and running. Press 'q' to quit.");

                    ConsoleKeyInfo info = Console.ReadKey();
                    while (info.Key != ConsoleKey.Q) {
                        info = Console.ReadKey();
                    }

                }
                catch (Exception exception) {
                    Console.Error.WriteLine("LayerContainer crashed fatally. Exception:\n {0}. Restarting LayerContainer...", exception);
                }


            Console.WriteLine("LayerContainer shutting down.");

            Console.ReadKey();
        }
    }
}