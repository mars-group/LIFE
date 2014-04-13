using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.AgentObjects.GoalForming;
using CommonModelTypes.Interface.SimObjects;
using ForestModel.Implementation.Agents;
using VillageModel.Implementaion.Agents.VillagerUseCase;
using VillageModel.Implementaion.Agents.VillagerUseCase.Actions;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion.Agents
{
    internal class ChuckWoodPlan : IPlan

{

    private MoveToAction _chuckingArea;
    private VillagerConfig _config;
    private Villiager _villiager;
    private TreeAgent[] _chuckingTargets;
        


    private ChuckWoodPlan(Vector3D chuckingAreaPosition, VillagerConfig config) {
        this._chuckingArea = new MoveToAction(chuckingAreaPosition);
        _config = config;
        _chuckingTargets = new TreeAgent[0];
    }
        
    private ITickAction ChuckWoodUseCasePlan() {
        //TODO später weiter machen not impl multicastadapter

        if (_chuckingArea.AtWayPoint()) {
            if (_chuckingTargets.Length == 0) {
                return new SearchForTreesAction(out _chuckingTargets);
            }

            else {
                return _chuckingArea;
            }
        }
        throw new NotImplementedException();
    }

    private TreeAgent SearchForTree() {
        //TODO ask environment for trees
        throw new NotImplementedException();
    }


        public ITickAction GetNextAction() {
            return ChuckWoodUseCasePlan();
        }
    }

}
