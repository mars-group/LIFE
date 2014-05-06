using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using CommonModelTypes.Interface.SimObjects;
using ForestModel.Implementation.Agents;
using ForestModel.Interface;
using VillageModel.Implementaion.Agents.VillagerUseCase;
using VillageModel.Implementaion.Agents.VillagerUseCase.Actions;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion.Agents
{
    internal class ChuckWoodPlan : IPlan
    {
        private readonly int _numberOfTreesToChuck;
        private ForestLayerContainer _forestLayer;
        private RectangleF _chuckingPostion;


        public ChuckWoodPlan(int numberOfTreesToChuck,  RectangleF chuckingPostion, ForestLayerContainer forestLayer)
        {
            _numberOfTreesToChuck = numberOfTreesToChuck;
            _numberOfTreesToChuck = numberOfTreesToChuck;
            _forestLayer = forestLayer;
            this._chuckingPostion = chuckingPostion;
        }

        

        private ITickAction ChuckWoodPlanUseCase()
        {
            int chuckedWood = 0;
            var chuckingTargets = _forestLayer.GetPatchForPosition(_chuckingPostion).Trees;
            return new ChuckAction(chuckingTargets, 101);
        }
        

        public ITickAction GetNextAction()
        {
            return ChuckWoodPlanUseCase();
        }
    }

}
