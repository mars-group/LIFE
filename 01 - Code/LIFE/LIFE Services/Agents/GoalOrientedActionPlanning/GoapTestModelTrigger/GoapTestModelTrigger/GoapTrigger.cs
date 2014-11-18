using System;
using System.Collections.Generic;
using System.Reflection;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapTestModelTrigger {

    public class GoapTrigger {
        private static void Main(string[] args) {
            Console.WriteLine("-----------------------------------");

            Blackboard blackboard = new Blackboard();
            AbstractGoapSystem goapActionSystem = GoapComponent.LoadGoapConfiguration
                ("AgentTestSearchDepth", "GoapModelTest", blackboard);


            Console.WriteLine("Agent loaded. Write n for next action");
            char cha = ' ';

            while (cha != 'e') {
                cha = char.Parse(Console.ReadLine());
                if (cha == 'n') {
                    AbstractGoapAction a = goapActionSystem.GetNextAction();
                    ExecuteAction(a, blackboard);
                }
            }
            Console.WriteLine("press any key to leave");
            Console.ReadKey();
        }

        private static void ExecuteAction(AbstractGoapAction action, Blackboard blackboard) {
            Console.WriteLine(action.GetType() + " is now executed");
            List<WorldstateSymbol> curr = blackboard.Get(AbstractGoapSystem.Worldstate);
            List<WorldstateSymbol> result = action.GetResultingWorldstate(curr);
            blackboard.Set(AbstractGoapSystem.Worldstate, result);
        }


        private static void GetAssemblyAndConfigClass() {
            const string namespaceOfConfigClass = "GoapModelTest";
            const string nameOfConfigClass = "AgentConfig1";
            Assembly assembly = Assembly.Load(namespaceOfConfigClass);
            IGoapAgentConfig configClass =
                (IGoapAgentConfig) assembly.CreateInstance(namespaceOfConfigClass + "." + nameOfConfigClass);
        }

        private static void ShowAvailableAssemblies() {
            Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly1 in ass) {
                Console.WriteLine(assembly1.FullName);
            }
        }
    }

}