using System.Reflection;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapBetaActionSystem.Implementation {
    /// <summary>
    ///     main access to create an instance of the goap component
    /// </summary>
    public static class GoapComponent {
        /// <summary>
        ///     load the configuration of the agent by the config class
        /// </summary>
        /// <param name="nameOfConfigClass"></param>
        /// <param name="namespaceOfConfigClass"></param>
        /// <param name="blackboard"></param>
        /// <returns></returns>
        public static AbstractGoapSystem LoadGoapConfiguration
            (string nameOfConfigClass, string namespaceOfConfigClass, Blackboard blackboard) {
            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            IGoapAgentConfig configClass =
                (IGoapAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);

            return new GoapManager
                (configClass.GetAllActions(),
                    configClass.GetAllGoals(),
                    blackboard,
                    configClass.GetStartWorldstate());
        }
    }
}