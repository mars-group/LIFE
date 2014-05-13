using System.Collections.Generic;
using System.Linq;
using Common.Interfaces;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapKnowledgeProcessingComponent {

    public class GoapKnowledgeProcessing {

        private readonly List<GoapWorldState> _aggregatedGoapWorldStates = new List<GoapWorldState>();
        private GoapKnowledgeCache _knowledgeCache = new GoapKnowledgeCache();

        public GoapKnowledgeProcessing(IPerception perception) {
            // create predefined states and fill into list of states
            var worldStateIsHungry = new GoapWorldStateIsHungry(true);
            var worldStateSunIsShining = new GoapWorldStateSunIsShining(true);

            _aggregatedGoapWorldStates.Add(worldStateIsHungry);
            _aggregatedGoapWorldStates.Add(worldStateSunIsShining);
        }

        public GoapKnowledgeProcessing( )
        {
            
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
            // TODO in  knowledgeCache die neuen Infos einbringen
        }

        public void TriggerUpdateWorldStates() {
            _aggregatedGoapWorldStates.ForEach(worldState => worldState.CalculateIfWorldStateIsFullfilled());
        }

        
    }
}