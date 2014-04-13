using System.Linq;
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
    private bool _reachedArea;


    private ChuckWoodPlan(Vector3D chuckingAreaPosition, VillagerConfig config) {
        this._chuckingArea = new MoveToAction(chuckingAreaPosition);
        _config = config;
        _chuckingTargets = new TreeAgent[0];
        _reachedArea = false;
    }

        private ITickAction ChuckWoodUseCasePlan()
        {

            //if not at are move to move to it
            if (! _chuckingArea.AtWayPoint() || ! _reachedArea)
            {
                //if i was once at the area set flag
                _reachedArea = _chuckingArea.AtWayPoint();
                return _chuckingArea;
            }

            // at this point the agent has reached the chucking are.

            //which trees to chuck, thats the question
            if (_chuckingTargets.Length == 0)
            {
                //Todo delegate?
                //find trees
                return new SearchForTreesAction(out _chuckingTargets);
            }
            var chukcingTree = _chuckingTargets.First();
            //have spottet some trees to chuck
            var treePostion =  new MoveToAction(chukcingTree.Position);
            
            //move to tree
            if (! treePostion.AtWayPoint()) return treePostion;

            //at tree....CHUCK IT
        }




        public ITickAction GetNextAction() {
            return ChuckWoodUseCasePlan();
        }
    }

}
