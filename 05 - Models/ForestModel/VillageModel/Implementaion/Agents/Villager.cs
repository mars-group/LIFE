using System;
using CommonModelTypes.Interface.AgentObjects;
using CommonModelTypes.Interface.SimObjects;
using ConfigurationAdapter.Interface;
using ForestModel.Interface;
using VillageModel.Implementaion.Agents.VillagerUseCase;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion.Agents
{
    public class Villager : AbstractAgent
    {

        #region fields
        private Village _homeVillage;
        private int _woodStorage;
        private int _dailyWoodUsage;
        private VillagerConfig _config;
        private ChuckWoodPlan _chuckWoodPlan;
        private ForestLayerContainer _forestLayer;

        #endregion

        #region constructors
        public Villager(int id, Vector3D position, Village homeVillage)
            : base(id, position)
        {
            _config = Configuration.Load<VillagerConfig>();

            _homeVillage = homeVillage;
            _woodStorage = 0;
            _dailyWoodUsage = _config.DailyWoodUsage;
            _pathHome = new Area(_homeVillage.Position);
        }
        #endregion

        #region public methods
        public override void Tick()
        {
            if (_woodStorage > 0)
            {
                //sit in the village and do stuff consume wood
                _woodStorage = _woodStorage - _dailyWoodUsage;
            }
            else
            {
                _chuckWoodPlan = new ChuckWoodPlan(_config.treesPerPlan, _homeVillage.GetRandomLocationInsideVillageRadius(), _forestLayer);
                _chuckWoodPlan.GetNextAction().Execute(this);
            }

        }

        public void StoreWood(int wood)
        {
            _woodStorage = _woodStorage + wood;
        }

        #endregion

    }
}
