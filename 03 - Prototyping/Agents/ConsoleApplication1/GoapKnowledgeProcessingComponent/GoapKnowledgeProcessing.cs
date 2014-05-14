using System.Collections.Generic;
using System.Linq;
using Common.Interfaces;
using Common.Types;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapKnowledgeProcessingComponent {

    public class GoapKnowledgeProcessing {
      private readonly IPerception _perception;

      private readonly List<GoapWorldState> _aggregatedGoapWorldStates = new List<GoapWorldState>();
        private GoapKnowledgeCache _knowledgeCache = new GoapKnowledgeCache();

        public GoapKnowledgeProcessing(IPerception perception) {
          _perception = perception;
        }

      /// <summary>
        /// only for test
        /// </summary>
        public GoapKnowledgeProcessing() {
            // create predefined states and fill into list of states
            _aggregatedGoapWorldStates.Add(new GoapWorldStateIsHungry(true));
            _aggregatedGoapWorldStates.Add(new GoapWorldStateSunIsShining(true));
            _aggregatedGoapWorldStates.Add(new GoapWorldStateGotIce(false));
        }

        internal void AddWorldState(GoapWorldState worldstate) {
            this.AggregatedGoapWorldStates.Add(worldstate);
        }

        internal bool IsSunShining() {
            return _aggregatedGoapWorldStates.Any(aggregatedGoapWorldState => aggregatedGoapWorldState is GoapWorldStateSunIsShining && aggregatedGoapWorldState.GetState() == true);
        }

        internal bool IsHungry() {
            return _aggregatedGoapWorldStates.Any(aggregatedGoapWorldState => aggregatedGoapWorldState is GoapWorldStateIsHungry && aggregatedGoapWorldState.GetState() == true);
        }

        internal bool GotIce() {
            return _aggregatedGoapWorldStates.Any(aggregatedGoapWorldState => aggregatedGoapWorldState is GoapWorldStateGotIce && aggregatedGoapWorldState.GetState() == true);
        }


        /// <summary>
        ///     some worldstates change their values by tick
        /// </summary>
        public void Tick() {
            _aggregatedGoapWorldStates.ForEach(worldState => worldState.Tick());
        }

        public List<GoapWorldState> AggregatedGoapWorldStates {
            get { return _aggregatedGoapWorldStates; }
        }


        public void SenseAll() {

          var isSunShining = _perception.GetData<ISunInput>().GetSunshine();

          // TODO in  knowledgeCache die neuen Infos einbringen
        }

        public void TriggerUpdateWorldStates() {
            _aggregatedGoapWorldStates.ForEach(worldState => worldState.CalculateIfWorldStateIsFullfilled());
        }

        
    }
}