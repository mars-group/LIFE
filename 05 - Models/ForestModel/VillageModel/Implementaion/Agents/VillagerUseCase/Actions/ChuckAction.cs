using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using ForestModel.Implementation.Agents;
using LayerAPI.Interfaces;

namespace VillageModel.Implementaion.Agents.VillagerUseCase.Actions
{
    public class ChuckAction : ITickAction
    {
        private List<TreeAgent> _chuckingTargets;
        private int _dmgPerSwing;

        public ChuckAction(List<TreeAgent> chuckingTargets, int dmgPerSwing)
        {
            _chuckingTargets = chuckingTargets;
            _dmgPerSwing = dmgPerSwing;
        }

        public void Execute(IAgent target)
        {
            if (target is Villager)
            {
                var villager = (Villager) target;

                var chuckedWood = 0;

                foreach (var chuckingTarget in _chuckingTargets)
                {
                    chuckedWood += chuckingTarget.Chuck(_dmgPerSwing);
                }

                villager.StoreWood(chuckedWood);
                
            }
            
        }
    }
}
