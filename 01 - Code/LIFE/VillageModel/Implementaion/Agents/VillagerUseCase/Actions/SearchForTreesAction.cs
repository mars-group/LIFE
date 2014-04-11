using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using ForestModel.Implementation.Agents;

namespace VillageModel.Implementaion.Agents.VillagerUseCase.Actions
{
    class SearchForTreesAction : ITickAction
    {

        public SearchForTreesAction( out TreeAgent[] chuckingTargets) {
            //TODO
            chuckingTargets = new TreeAgent[] {};
        }

        public void Execute() {
            throw new NotImplementedException();
        }
    }
}
