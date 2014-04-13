using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using ForestModel.Implementation.Agents;

namespace VillageModel.Implementaion.Agents.VillagerUseCase.Actions
{
    class ChuckAction : ITickAction
    {

        private Villager _villager;
        private TreeAgent _chuckingTarget;
        private int _swingsPerTick;
        private int _dmgPerSwing;


        public void Execute()
        {
            for (int i = 0; i < _swingsPerTick; i++)
            {
                
            }
        }
    }
}
