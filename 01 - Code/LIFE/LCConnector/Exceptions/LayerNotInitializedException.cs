using System;
using LCConnector.TransportTypes;

namespace LCConnector.Exceptions {
    public class LayerNotInitializedException : Exception {
        public TLayerInstanceId Id { get; protected set; }

        public LayerNotInitializedException(string message, TLayerInstanceId id)
            : base(message) {
            Id = id;
        }

        public LayerNotInitializedException(TLayerInstanceId id) {
            Id = id;
        }
    }
}