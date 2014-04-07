using System;

namespace LCConnector.TransportTypes {
    [Serializable]
    public class TLayerDescription : IEquatable<TLayerDescription> {
        /// <summary>
        /// The layer's name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// In a version xx.yyyyyy, this is the xx.
        /// </summary>
        public int MajorVersion { get; protected set; }
        
        /// <summary>
        /// /// In a version xx.yyyyyy, this is the yyyyyy.
        /// </summary>
        public int MinorVersion { get; protected set; }

        /// <summary>
        /// The filename of the original dll holding the binary code.
        /// </summary>
        public string FileName { get; protected set; }

        #region Object Contracts

        public bool Equals(TLayerDescription other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && MajorVersion == other.MajorVersion && MinorVersion == other.MinorVersion;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TLayerDescription) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode*397) ^ MajorVersion;
                hashCode = (hashCode*397) ^ MinorVersion;
                return hashCode;
            }
        }

        #endregion
    }
}