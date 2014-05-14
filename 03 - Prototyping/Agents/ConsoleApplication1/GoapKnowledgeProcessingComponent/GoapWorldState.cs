using GoapComponent.GoapKnowledgeProcessingComponent.GoapWorldStatesStock;

namespace GoapComponent.GoapKnowledgeProcessingComponent {

    public abstract class GoapWorldState {
        protected bool IsStateFulfilled;

        /// <summary>
        /// </summary>
        /// <param name="startValue"></param>
        protected GoapWorldState(bool startValue) {
            IsStateFulfilled = startValue;
        }

        /// <summary>
        ///     worldstate could be calculated by complex variables
        /// </summary>
        public abstract void CalculateIfWorldStateIsFullfilled();

        /// <summary>
        ///     trigger used by this class to get the class specific variables neccessary for calculationg if state is fulfilled
        /// </summary>
        public abstract void PullDependingValues();

        public abstract void Tick();

        public bool GetState() {
            return IsStateFulfilled;
        }

        // TODO check if equals in der abstakten klasse ausreichend wäre
    }

   
}