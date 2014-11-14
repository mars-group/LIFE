using System;
using System.Collections.Generic;
using System.Reflection;
using GoapBetaActionSystem.Implementation;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GoapBetaCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapUser {

    internal static class GoapTrigger {
        private static void Main(string[] args) {
            Console.WriteLine("-----------------------------------");

            Blackboard blackboard = new Blackboard();
            AbstractGoapSystem goapActionSystem = GoapComponent.LoadGoapConfiguration
                ("AgentConfig1", "GoapModelTest", blackboard);


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