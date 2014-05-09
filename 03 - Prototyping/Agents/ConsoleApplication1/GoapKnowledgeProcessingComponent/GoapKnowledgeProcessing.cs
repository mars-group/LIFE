using System.Collections.Generic;
using Common.Interfaces;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapKnowledgeProcessingComponent {
    public class GoapKnowledgeProcessing {
        private readonly List<GoapWorldState> aggregatedGoapWorldStates = new List<GoapWorldState>();
        private GoapKnowledgeCache knowledgeCache = new GoapKnowledgeCache();

        public GoapKnowledgeProcessing(IPerception perception) {
            // create predefined states and fill into list of states
            var worldStateIsHungry = new GoapWorldStateIsHungry(false);
            var worldStateSunIsShining = new GoapWorldStateSunIsShining(true);

            aggregatedGoapWorldStates.Add(worldStateIsHungry);
            aggregatedGoapWorldStates.Add(worldStateSunIsShining);
        }

        /// <summary>
        ///     some worldstates change their values by tick
        /// </summary>
        public void Tick() {
            aggregatedGoapWorldStates.ForEach(worldState => worldState.Tick());
        }

        public void SenseAll() {
            // TODO in  knowledgeCache die neuen Infos einbringen
        }

        public void TriggerUpdateWorldStates() {
            aggregatedGoapWorldStates.ForEach(worldState => worldState.CalculateIfWorldStateIsFullfilled());
        }
    }
}