using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using Common.Interfaces;
using Common.Types;
using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapKnowledgeProcessingComponent {

    public class GoapKnowledgeProcessing {
      private readonly IPerception _perception;

        private GoapKnowledgeCache _knowledgeCache = new GoapKnowledgeCache();

        public GoapKnowledgeProcessing(IPerception perception) {
            AggregatedGoapWorldStates = new List<GoapWorldState>();
            _perception = perception;
          // create predefined states and fill into list of states
          AggregatedGoapWorldStates.Add(new GoapWorldStateIsHungry(true));
          AggregatedGoapWorldStates.Add(new GoapWorldStateSunIsShining(true));
          AggregatedGoapWorldStates.Add(new GoapWorldStateGotIce(false));
        }

        /// <summary>
          /// only for test
          /// </summary>
          public GoapKnowledgeProcessing() {
            AggregatedGoapWorldStates = new List<GoapWorldState>();
            // create predefined states and fill into list of states
              AggregatedGoapWorldStates.Add(new GoapWorldStateIsHungry(true));
              AggregatedGoapWorldStates.Add(new GoapWorldStateSunIsShining(true));
              AggregatedGoapWorldStates.Add(new GoapWorldStateGotIce(false));
          }

        internal void AddWorldState(GoapWorldState worldstate) {
            this.AggregatedGoapWorldStates.Add(worldstate);
        }

        internal bool IsSunShining() {
            return AggregatedGoapWorldStates.Any(aggregatedGoapWorldState => aggregatedGoapWorldState is GoapWorldStateSunIsShining && aggregatedGoapWorldState.GetState() == true);
        }

        internal bool IsHungry() {
            return AggregatedGoapWorldStates.Any(aggregatedGoapWorldState => aggregatedGoapWorldState is GoapWorldStateIsHungry && aggregatedGoapWorldState.GetState() == true);
        }

        internal bool GotIce() {
            return AggregatedGoapWorldStates.Any(aggregatedGoapWorldState => aggregatedGoapWorldState is GoapWorldStateGotIce && aggregatedGoapWorldState.GetState() == true);
        }


       public List<GoapWorldState> AggregatedGoapWorldStates { get; private set; }


        public void SenseAll() {

            var isSunShining = _perception.GetData<ISunInput>().GetSunshine();

            foreach (var item in AggregatedGoapWorldStates.OfType<GoapWorldStateSunIsShining>()) {
                item.SetState(isSunShining);
            }

            Tick();
            TriggerUpdateWorldStates();

            //var sunshine =  AggregatedGoapWorldStates.Find(item => item.GetType() is GoapWorldStateSunIsShining);
            //sunshine.SetState(isSunShining);
            // TODO in  knowledgeCache die neuen Infos einbringen
        }

        private void TriggerUpdateWorldStates() {
            AggregatedGoapWorldStates.ForEach(worldState => worldState.CalculateIfWorldStateIsFullfilled());
        }

       private void Tick(){
            AggregatedGoapWorldStates.ForEach(worldState => worldState.Tick());
        }

        
    }
}