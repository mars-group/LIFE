namespace ESCTestLayer.Entities {
    #region Namespace imports

    using System.Collections.Generic;
    using System.Linq;
    using CommonTypes.TransportTypes;
    using GenericAgentArchitectureCommon.Interfaces;

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