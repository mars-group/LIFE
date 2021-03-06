﻿using System;
using System.Reflection;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest;

namespace GoapActionSystemFactory.Implementation {
    /// <summary>
    ///     main access to create an instance of the goap component
    /// </summary>
    public static class GoapComponent {
        /// <summary>
        ///     load the configuration of the agent by the config class
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <returns></returns>
        public static IActionSystem LoadAgentConfiguration(string nameOfConfigClass, string namespaceOfConfigClass) {
            try {
                //Assembly assembly = Assembly.LoadFrom("../../../" + namespaceOfConfigClass + "/bin/release/" + namespaceOfConfigClass + ".dll");
                
                
                Assembly assembly = Assembly.Load(namespaceOfConfigClass);
                var configClass =
                    (IAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);

                return new GoapManager(configClass.GetAllActions(), configClass.GetAllGoals(),
                    configClass.GetStartWorldstate());
                 
            }

            catch (Exception e) {
                Console.WriteLine(e.Message);
                throw new ArgumentException("No config class with name " + nameOfConfigClass + " or assembly with name " +
                                            namespaceOfConfigClass + " found. " + e.Message);
            }
        }
    }
}