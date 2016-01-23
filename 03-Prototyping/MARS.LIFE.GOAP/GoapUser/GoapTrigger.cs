using System;
using System.Reflection;
using CommonTypes.Interfaces;
using GoapActionSystemFactory.Implementation;
using GoapCommon.Interfaces;

namespace GoapUser {
    internal static class GoapTrigger {
        private static void Main(string[] args) {
            Console.WriteLine("-----------------------------------");


            IActionSystem goapActionSystem = GoapComponent.LoadAgentConfiguration("AgentConfig1", "GoapModelTest");

            Console.WriteLine(goapActionSystem.GetNextAction().GetType());

            

            //return new GoapManager(configClass.GetAllActions(), configClass.GetAllGoals(),configClass.GetStartWorldstate());

            //System.Configuration.ConfigurationManager.AppSettings[]


            Console.ReadKey();
        }

        private static void GetAssemblyAndConfigClass() {
            const string namespaceOfConfigClass = "GoapModelTest";


            const string nameOfConfigClass = "AgentConfig1";


            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            var configClass =
                (IAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);
        }

        private static void ShowAvailableAssemblies() {
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly1 in ass) {
                Console.WriteLine(assembly1.FullName);
            }
        }
    }
}