using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Interfaces;

namespace GoapComponent
{
    static class ScenarioIceEater
    {
        static Goap GetIceEaterGoap() {
            return new Goap();
        }

        public static void Main(string[] args) {
            Goap goap = ScenarioIceEater.GetIceEaterGoap();

            foreach (var availableGoal in goap.AllAvailableGoals) {
                Console.WriteLine(availableGoal);
                
            }
            Console.WriteLine(goap.CurrentGoal);
            IInteraction action = goap.Reason();
            Console.WriteLine(action);

            action.Execute();
        }

    }
}
