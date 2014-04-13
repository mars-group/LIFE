using System;

namespace SMConnector.TransportTypes
{
    public class TModelDescription : IEquatable<TModelDescription> {
        public string Name { get; private set; }

        public bool Running { get; private set; }

        public string Description { get; private set; }

        public TStatusUpdate Status { get; set; }

        public TModelDescription(string name, string description = "", string status = "Not Running", bool running = false) {
            Name = name;
            Running = running;
            this.Description = description;
            this.Status = new TStatusUpdate(status);
        }



        #region Object contracts

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

        public static bool operator ==(TModelDescription left, TModelDescription right) {
            return Equals(left, right);
        }

        public static bool operator !=(TModelDescription left, TModelDescription right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
