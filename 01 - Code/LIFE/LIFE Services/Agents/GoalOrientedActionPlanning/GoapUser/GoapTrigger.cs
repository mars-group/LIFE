using System;
using System.Reflection;
using GoapActionSystemFactory.Implementation;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapUser {
    internal static class GoapTrigger {
        private static void Main(string[] args) {
            Console.WriteLine("-----------------------------------");


            AbstractGoapSystem goapActionSystem = GoapComponent.LoadAgentConfiguration("AgentConfig1", "GoapModelTest");

            Console.WriteLine(goapActionSystem.GetNextAction().GetType());


            //return new GoapManager(configClass.GetAllActions(), configClass.GetAllGoals(),configClass.GetStartWorldstate());

            //System.Configuration.ConfigurationManager.AppSettings[]


            Console.ReadKey();
        }

        private static void GetAssemblyAndConfigClass() {
            const string namespaceOfConfigClass = "GoapModelTest";


            const string nameOfConfigClass = "AgentConfig1";


            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            IAgentConfig configClass =
                (IAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);
        }

        private static void ShowAvailableAssemblies() {
            Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly1 in ass) {
                Console.WriteLine(assembly1.FullName);
            }
        }
    }
}