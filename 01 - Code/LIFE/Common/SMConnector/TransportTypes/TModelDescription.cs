using System;

namespace SMConnector.TransportTypes
{
    public class TModelDescription : IEquatable<TModelDescription> {
        public string Name { get; private set; }


        public TModelDescription(string name) {
            Name = name;
        }

        public bool Equals(TModelDescription other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TModelDescription) obj);
        }

        public override int GetHashCode() {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
