using System;
using System.Collections.Generic;
using System.Reflection;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapActionSystemFactory.Implementation {
    /// <summary>
    ///     main access to create an instance of the goap component
    /// </summary>
    public static class GoapComponent {
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





            List<IGoapAction> actionList = new List<IGoapAction> { new ActionGetToy(), new ActionClean(), new ActionPlay() };

            List<IGoapWorldstate> startState = new List<IGoapWorldstate> {
                new Happy(true, WorldStateEnums.Happy),
                new HasMoney(false, WorldStateEnums.HasMoney),
                new HasToy(false, WorldStateEnums.HasToy)
            };

            List<IGoapGoal> goalList = new List<IGoapGoal>{new GoalBeHappy(startState)};

            if (_goapManager != null) return _goapManager;
            return _goapManager = new GoapManager(actionList, goalList);



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
}