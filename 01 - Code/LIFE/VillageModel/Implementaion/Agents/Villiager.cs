using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using CommonModelTypes.Interface.AgentObjects;
using CommonModelTypes.Interface.SimObjects;
using ConfigurationAdapter.Interface;
using VillageModel.Implementaion.Agents.VillagerUseCase;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion.Agents
{
    public class Villiager : AbstractAgent
    {

        #region fields
        private Village _homeVillage;
        private int _woodStorage;
        private int _dailyWoodUsage;
        private Configuration<VillagerConfig> _config;
        private Vector3D _nextWoodCuttingLocation;
        private ChuckWoodUseCase chuckWoodUseCase;
        private MoveToAction _pathHome;
        #endregion

        #region constructors
        public Villiager(int id, Vector3D position, Village homeVillage)
            : base(id, position)
        {
            _config = new Configuration<VillagerConfig>();

            _homeVillage = homeVillage;
            _woodStorage = 0;
            _dailyWoodUsage = _config.Instance.DailyWoodUsage;
            _nextWoodCuttingLocation = GetRandomLocationInsideVillageRadius();
            _pathHome = new MoveToAction(_homeVillage.Position);
        }
        #endregion

        #region public methods
        public override void tick()
        {
            if (_woodStorage > 0)
            {
                // if not at village go back to village
                if (AtWayPoint(_homeVillage.Position))
                {
                    _pathHome.Execute();
                }
                //sit in the village and do stuff consume wood
                _woodStorage = _woodStorage - _dailyWoodUsage;
            }
            else
            {
                chuckWoodUseCase.GetNextAction().Execute();
            }

        }

        public void StoreWood(int wood) {
            _woodStorage = _woodStorage + wood;
        }

        #endregion

        #region private methods

        private Boolean AtWayPoint(Vector3D destination)
        {
            return ((destination.DistanceToWayPoint(Position) - _config.Instance.WayPointReachedDistance) <= 0);
        }

        private Vector3D GetRandomLocationInsideVillageRadius()
        {
            return _homeVillage.GetRandomLocationInsideVillageRadius();
        }
        #endregion

    }
}
