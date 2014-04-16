using System.Linq;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using CommonModelTypes.Interface.Datatypes;
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

        private Area _chuckingArea;
        private VillagerConfig _config;
        private Village _village;
        private TreeAgent[] _chuckingTargets;
        private ForestLayerContainer _forestLayer;



        private ChuckWoodPlan(VillagerConfig config)
        {
            _config = config;
            _chuckingTargets = new TreeAgent[0];
        }

        private ITickAction ChuckWoodUseCasePlan()
        {




        }




        public ITickAction GetNextAction()
        {
            return ChuckWoodUseCasePlan();
        }
    }

}
