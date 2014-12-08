using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using log4net;
using TypeSafeBlackboard;

namespace GoapActionSystem.Implementation {

    /// <summary>
    ///     main access to create an instance of the goap component
    /// </summary>
    public static class GoapComponent {
        /// <summary>
        ///     Logger instance for the goap action system
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     load the configuration of the agent by the config class
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <param name="blackboard"></param>
        /// <param name="ignoreFinishedForTesting"></param>
        /// <returns></returns>
        public static AbstractGoapSystem LoadGoapConfiguration
            (string nameOfConfigClass,
                string namespaceOfConfigClass,
                Blackboard blackboard,
                bool ignoreFinishedForTesting = false) {

            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            IGoapAgentConfig configClass =
                (IGoapAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);

            if (configClass == null) {
                throw new NullReferenceException("The configuration class for the Goap manager was not found");
            }

            List<AbstractGoapAction> actions = configClass.GetAllActions();
            List<IGoapGoal> goals = configClass.GetAllGoals();
            List<WorldstateSymbol> symbols = configClass.GetStartWorldstate();
            int maxGraphDepth = configClass.GetMaxGraphSearchDepth();

            CheckActionsValid(actions);
            CheckGoalsValid(goals);
            CheckSymbolsValid(symbols);

            return new GoapManager(actions, goals, blackboard, symbols, maxGraphDepth, ignoreFinishedForTesting);
        }


        public static AbstractGoapSystem LoadGoapConfigurationWithSelfreference
            (string nameOfConfigClass, string namespaceOfConfigClass, Blackboard blackboard, object agent) {
            Assembly assembly = Assembly.Load(namespaceOfConfigClass);

            Type type = Type.GetType(namespaceOfConfigClass + "." + nameOfConfigClass + ", " + assembly.FullName);

            object[] parameterArray = {agent, blackboard};
            IGoapAgentConfig configClass = (IGoapAgentConfig) Activator.CreateInstance(type, parameterArray);


            if (configClass == null) {
                throw new NullReferenceException("The configuration class for the Goap manager was not found");
            }

            List<AbstractGoapAction> actions = configClass.GetAllActions();
            List<IGoapGoal> goals = configClass.GetAllGoals();
            List<WorldstateSymbol> symbols = configClass.GetStartWorldstate();
            int maxGraphDepth = configClass.GetMaxGraphSearchDepth();

            CheckActionsValid(actions);
            CheckGoalsValid(goals);
            CheckSymbolsValid(symbols);


            return new GoapManager(actions, goals, blackboard, symbols, maxGraphDepth, configClass);
        }

        /// <summary>
        ///     helper method for checking parameter
        /// </summary>
        /// <param name="paramList"></param>
        /// <returns></returns>
        private static bool IsEmptyParam(IList paramList) {
            return paramList == null || paramList.Count == 0;
        }

        private static void CheckActionsValid(List<AbstractGoapAction> actions) {
            if (IsEmptyParam(actions)) {
                throw new ArgumentException
                    ("GoapManager: Goap manager cannot start with empty action list");
            }
        }

        private static void CheckGoalsValid(List<IGoapGoal> goals) {
            if (IsEmptyParam(goals)) {
                throw new ArgumentException
                    ("GoapManager: Goap manager cannot start with empty goal list");
            }
        }

        private static void CheckSymbolsValid(List<WorldstateSymbol> symbols) {
            if (IsEmptyParam(symbols)) {
                throw new ArgumentException
                    ("GoapManager: Goap manager cannot start with empty symbols list");
            }
        }
    }

}