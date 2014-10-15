using System.Reflection;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapActionSystem.Implementation {
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
        public static AbstractGoapSystem LoadGoapConfiguration(string nameOfConfigClass, string namespaceOfConfigClass, Blackboard blackboard ) {
          
                Assembly assembly = Assembly.Load(namespaceOfConfigClass);
                var configClass =
                    (IAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);

                return new GoapManager
                    (configClass.GetAllActions(), configClass.GetAllGoals(), blackboard,
                        configClass.GetStartWorldstate());

            
        }
    }
}