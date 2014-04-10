using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using ConfigurationAdapter.Interface;
using VillageModel.Interface.Configuration;

namespace VillageModel.Implementaion.Agents
{
    class Villiager : AbstractAgent
    {

        #region fields
        private Village _homeVillage;
        private int _woodStorage;
        private int _dailyWoodUsage;
        private Configuration<VillagerConfig> _config;
        private Vector3D _nextWoodCuttingLocation;
        private ChuckWoodUseCase chuckWoodUseCase;
        #endregion

        #region constructors
        public Villiager(int id, Vector3D position, Village homeVillage)
            : base(id, position)
        {
            _config = new Configuration<VillagerConfig>(new VillagerConfig());

            _homeVillage = homeVillage;
            _woodStorage = 0;
            _dailyWoodUsage = _config.Content.DailyWoodUsage;
            _nextWoodCuttingLocation = GetRandomLocationInsideVillageRadius();
        }
        #endregion

        #region public methods
        public override void tick()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private methods
        private Vector3D GetRandomLocationInsideVillageRadius()
        {
         return  _homeVillage.GetRandomLocationInsideVillageRadius();
        }
        #endregion

    }
}
