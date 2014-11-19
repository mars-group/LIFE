namespace ESCTestLayer.Entities {
    #region Namespace imports

    using System.Collections.Generic;
    using System.Linq;
    using GenericAgentArchitectureCommon.Interfaces;
    using GenericAgentArchitectureCommon.TransportTypes;

    #endregion

    public class MovementResult {
        public bool Success { get { return Collisions == null || !Collisions.Any(); } }
        public TVector Position { get; private set; } //TODO obsolet
        public IEnumerable<ISpatialEntity> Collisions { get; private set; }

        public MovementResult(IEnumerable<ISpatialEntity> collisions = null) {
            Collisions = collisions;
        }


    }
}