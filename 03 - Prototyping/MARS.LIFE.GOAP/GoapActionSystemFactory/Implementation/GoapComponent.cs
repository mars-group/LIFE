using System;
using System.Collections.Generic;
using System.Reflection;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;
using log4net;

namespace GoapActionSystemFactory.Implementation {
    /// <summary>
    ///     main access to create an instance of the goap component
    /// </summary>
    public static class GoapComponent {
        private static readonly ILog Logger = LogManager.GetLogger("Logger");

        /// <summary>
        ///     singleton goap manager
        /// </summary>
        private static IActionSystem _goapManager;

        /// <summary>
        ///     singleton goap manager per process
        /// </summary>
        /// <returns>IActionSytem</returns>
        public static IActionSystem GetGoap() {
            if (_goapManager != null) return _goapManager;
            return _goapManager = new GoapManager();
        }


        public static IActionSystem GetFullModelTestGoap() {
            var actionList = new List<AbstractGoapAction> {new ActionGetToy(), new ActionClean(), new ActionPlay()};

            var startState = new List<IGoapWorldstate> {
                new Happy(true),
                new HasMoney(false),
                new HasToy(false)
            };

            var goalList = new List<IGoapGoal> {new GoalBeHappy()};

            if (_goapManager != null) return _goapManager;
            return _goapManager = new GoapManager(actionList, goalList);
        }

        /// <summary>
        /// load the configuration of the agent by the config class
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <returns></returns>
        public static IActionSystem LoadAgentConfiguration(string nameOfConfigClass, string namespaceOfConfigClass) {
            try {
                Assembly assembly = Assembly.Load(namespaceOfConfigClass);
                var configClass = (IAgentConfig)assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);

                return _goapManager =
                        new GoapManager(configClass.GetAllActions(), configClass.GetAllGoals(),
                            configClass.GetStartWorldstate());
            }

            catch (Exception e) {
                Logger.Info( nameOfConfigClass + " Not Found");
                Console.WriteLine(e.Message);
                throw new ArgumentException("No config class with name " +  nameOfConfigClass + " or assembly with name " + namespaceOfConfigClass + " found");
            }
        }


        /*
        public static IActionSystem GetGoap(List<string> actionsToUseList, List<string> worldstatesToUseList) {
            string assemblyName = "GoapModelTest";
            // find the correct assembly
            string currentDir = Environment.CurrentDirectory;
            string wholeProject = "../../../" + currentDir;

            Assembly assembly = Assembly.LoadFrom("../../../" + assemblyName + "/bin/Release/" + assemblyName + ".dll");

            List<IGoapAction> actionList = new List<IGoapAction>();

            foreach (string actionName in actionsToUseList) {
                var type = assembly.GetType(assemblyName + ".Actions." + actionName, true);
                IGoapAction action = (IGoapAction) Activator.CreateInstance(type);
                actionList.Add(action);
            }

            List<IGoapGoal> goalList = new List<IGoapGoal>();


            if (_goapManager != null) return _goapManager;
            return _goapManager = new GoapManager(actionList, goalList);
        }*/
    }


    public class Help {
        public static void PrintA(object[] args) {
            foreach (var o in args) {
                Console.WriteLine(o);
            }
        }
    }
}