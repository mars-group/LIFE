using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using GoapCommon.Abstract;
using GoapCommon.Exceptions;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using log4net;
using TypeSafeBlackboard;

[assembly: InternalsVisibleTo("GoapTests")]

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
        ///     without any parameter.
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

            CheckActionsNotEmpty(configClass.GetAllActions());
            CheckActionsForDoubledWorldstateSymbolKeys(configClass.GetAllActions());
            CheckGoalsNotEmpty(configClass.GetAllGoals());
            CheckGoalsForDoubledWorldstateSymbolKeys(configClass.GetAllGoals());

            return new GoapManager(blackboard, configClass);
        }

        /// <summary>
        ///     Here the goap manager can be created with a config class that needs
        ///     additional object instances for creation.
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <param name="blackboard"></param>
        /// <param name="configParams"></param>
        /// <returns></returns>
        public static AbstractGoapSystem LoadGoapConfigurationWithSelfReference
            (string nameOfConfigClass, string namespaceOfConfigClass, Blackboard blackboard, object[] configParams) {
            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            Type typeOfConfigClass = Type.GetType
                (namespaceOfConfigClass + "." + nameOfConfigClass + ", " + assembly.FullName);

            IGoapAgentConfig configClass =
                (IGoapAgentConfig) Activator.CreateInstance(typeOfConfigClass, configParams);
            
            if (configClass == null) {
                throw new NullReferenceException("The configuration class for the Goap manager was not found");
            }

            CheckActionsNotEmpty(configClass.GetAllActions());
            CheckActionsForDoubledWorldstateSymbolKeys(configClass.GetAllActions());
            CheckGoalsNotEmpty(configClass.GetAllGoals());
            CheckGoalsForDoubledWorldstateSymbolKeys(configClass.GetAllGoals());

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

        internal static void CheckActionsNotEmpty(List<AbstractGoapAction> actions) {
            if (IsEmptyParam(actions)) {
                throw new ArgumentException
                    ("GoapManager: Goap manager cannot start with empty action list");
            }
        }

        internal static void CheckGoalsNotEmpty(List<AbstractGoapGoal> goals) {
            if (IsEmptyParam(goals)) {
                throw new ArgumentException
                    ("GoapManager: Goap manager cannot start with empty goal list");
            }
        }

        internal static void CheckGoalsForDoubledWorldstateSymbolKeys(List<AbstractGoapGoal> goals) {
            foreach (AbstractGoapGoal goal in goals) {
                if (HasDoubleKeys(goal.TargetWorldState)) {
                    throw new GoalDesignException("GoapComponent: A goal has a doubled key in the target worldstate");
                }
            }
        }

        internal static void CheckActionsForDoubledWorldstateSymbolKeys(List<AbstractGoapAction> actionList) {
            foreach (AbstractGoapAction action in actionList) {
                if (HasDoubleKeys(action.PreConditions)) {
                    throw new ActionDesignException("GoapComponent: An Action has a doubled key in the preconditions");
                }
                if (HasDoubleKeys(action.Effects)) {
                    throw new ActionDesignException("GoapComponent: An Action has a doubled key in the effects");
                }
            }
        }

        internal static bool HasDoubleKeys(List<WorldstateSymbol> worldstateSymbols) {
            List<Enum> usedSymbolKeys = new List<Enum>();
            foreach (WorldstateSymbol symbol in worldstateSymbols) {
                if (usedSymbolKeys.Contains(symbol.EnumName)) {
                    return true;
                }
                usedSymbolKeys.Add(symbol.EnumName);
            }
            return false;
        }
    }

}