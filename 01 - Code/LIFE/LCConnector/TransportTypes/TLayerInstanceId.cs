using System;

namespace LCConnector.TransportTypes
{
    public class TLayerInstanceId : IEquatable<TLayerInstanceId> {
        /// <summary>
        /// The layer's identity.
        /// </summary>
        public TLayerDescription LayerDescription { get; protected set; }

        /// <summary>
        /// The instance's unique number.
        /// </summary>
        public int InstanceNr { get; protected set; }

        #region Object Contracts

        public bool Equals(TLayerInstanceId other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return LayerDescription.Equals(other.LayerDescription) && InstanceNr == other.InstanceNr;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TLayerInstanceId) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (LayerDescription.GetHashCode()*397) ^ InstanceNr;
            }
        }

        #endregion
    }
}
