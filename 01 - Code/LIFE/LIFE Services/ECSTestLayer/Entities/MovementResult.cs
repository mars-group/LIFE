namespace ESCTestLayer.Entities {
    #region Namespace imports

    using System;
    using System.Collections.Generic;
    using CommonTypes.TransportTypes;
    using GenericAgentArchitectureCommon.Interfaces;

    #endregion

    public class MovementResult {
        public bool Success { get; private set; }
        public TVector Position { get; private set; } //TODO obsolet
        public IEnumerable<ISpatialEntity> Collisions { get; private set; }
        public Dictionary<String, Object> Information { get; private set; }

        public MovementResult(bool success)
        {
            Success = success;
        }
        public MovementResult(bool success, IEnumerable<ISpatialEntity> collisions)
        {
            Success = success;
            Collisions = collisions;
        }

        public MovementResult(bool success, TVector position, Dictionary<String, Object> information) {
            Success = success;
            Position = position;
            Information = information;
        }

        public MovementResult(TVector position) :
            this(true, position, new Dictionary<String, Object>()) {}
    }
}