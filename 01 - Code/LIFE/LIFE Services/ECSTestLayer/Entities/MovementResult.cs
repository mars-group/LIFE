namespace ESCTestLayer.Entities {
    #region Namespace imports

    using System;
    using System.Collections.Generic;
    using CommonTypes.TransportTypes;

    #endregion

    public class MovementResult {
        public bool Success { get; private set; }
        public TVector Position { get; private set; }

        public Dictionary<String, Object> Information { get; private set; }

        public MovementResult(bool success, TVector position, Dictionary<String, Object> information) {
            Success = success;
            Position = position;
            Information = information;
        }

        public MovementResult(TVector position) :
            this(true, position, new Dictionary<String, Object>()) {}
    }
}