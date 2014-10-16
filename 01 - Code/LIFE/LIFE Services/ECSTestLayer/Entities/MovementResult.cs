namespace ESCTestLayer.Entities {
    #region Namespace imports

    using System;
    using System.Collections.Generic;
    using CommonTypes.DataTypes;
    using CommonTypes.TransportTypes;

    #endregion

    public class MovementResult {
        public TVector Position { get; private set; }

        public Dictionary<String, Object> Information { get; private set; }

        public MovementResult(TVector position, Dictionary<String, Object> information) {
            Position = position;
            Information = information;
        }

        public MovementResult(TVector position) :
            this(position, new Dictionary<String, Object>()) {}
    }
}