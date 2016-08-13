﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using LayerContainerFacade.Interfaces;
using log4net;

namespace LayerContainer {
    public class LayerContainerStarter {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (LayerContainerStarter));



        private static void Main(string[] args) {

                Logger.Info("LayerContainer trying to startup.");
                try {
                    Logger.Info("Initializing components and building application core...");

                    ILayerContainerFacade _facade = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();

                    Logger.Info("LayerContainer successfully started.");

                    Console.WriteLine("LayerContainer up and running. Press 'q' to quit.");

                    ConsoleKeyInfo info = Console.ReadKey();
                    while (info.Key != ConsoleKey.Q) {
                        info = Console.ReadKey();
                    }

                }
                catch (Exception exception) {
                    Logger.Fatal("LayerContainer crashed fatally. Exception:\n {0}. Restarting LayerContainer...", exception);
                }

    
            Logger.Info("LayerContainer shutting down.");

            // This will shutdown the log4net system
            LogManager.Shutdown();
            Console.ReadKey();
        }
    }
}