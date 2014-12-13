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
    ///     Main access to create an instance of the goap component. 
    /// </summary>
    public static class GoapComponent {

        /// <summary>
        ///     Logger instance for the goap action system
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     Here the goap manager can be created with a config class
        ///      without any parameter.     
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <param name="blackboard"></param>
        /// <returns></returns>
        public static AbstractGoapSystem LoadGoapConfiguration
            (string nameOfConfigClass,
                string namespaceOfConfigClass,
                Blackboard blackboard) {

            Assembly assembly = Assembly.Load(namespaceOfConfigClass);

            IGoapAgentConfig configClass =
                (IGoapAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);

            if (configClass == null) {
                throw new NullReferenceException("The configuration class for the Goap manager was not found");
            }

            CheckActionsValid(configClass.GetAllActions());
            CheckGoalsValid(configClass.GetAllGoals());
            
            return new GoapManager(blackboard, configClass);
        }

        /// <summary>
        ///     Here the goap manager can be created with a config class that needs 
        ///     additional object instances for creation.
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <param name="blackboard"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public static AbstractGoapSystem LoadGoapConfigurationWithSelfReference
            (string nameOfConfigClass, string namespaceOfConfigClass, Blackboard blackboard, object agent) {

            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            Type typeOfConfigClass = Type.GetType(namespaceOfConfigClass + "." + nameOfConfigClass + ", " + assembly.FullName);

            object[] parameterArray = {agent, blackboard};
            IGoapAgentConfig configClass = (IGoapAgentConfig) Activator.CreateInstance(typeOfConfigClass, parameterArray);


            if (configClass == null) {
                throw new NullReferenceException("The configuration class for the Goap manager was not found");
            }

            List<AbstractGoapAction> actions = configClass.GetAllActions();
            List<AbstractGoapGoal> goals = configClass.GetAllGoals();
            List<WorldstateSymbol> symbols = configClass.GetStartWorldstate();
            int maxGraphDepth = configClass.GetMaxGraphSearchDepth();

            CheckActionsValid(actions);
            CheckGoalsValid(goals);

            return new GoapManager(blackboard, configClass);
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

        private static void CheckGoalsValid(List<AbstractGoapGoal> goals){
            if (IsEmptyParam(goals)) {
                throw new ArgumentException
                    ("GoapManager: Goap manager cannot start with empty goal list");
            }
        }
    }
}