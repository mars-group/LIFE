using System;

namespace CommonTypes.TransportTypes {
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TSimModel : IEquatable<TSimModel> {
        private string _path;
        private string _name;

        public string Path {
            get { return _path; }
            set { }
        }

        public string Name {
            get { return _name; }
            set { }
        }

        public TSimModel(string path) {
            _path = path;
            var tmp = path.Split(System.IO.Path.DirectorySeparatorChar);
            _name = tmp[tmp.Length - 1];
        }

        #region Object Contracts

        public bool Equals(TSimModel other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_path, other._path);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TSimModel) obj);
        }

        public override int GetHashCode() {
            return (_path != null ? _path.GetHashCode() : 0);
        }

        public static bool operator ==(TSimModel left, TSimModel right) {
            return Equals(left, right);
        }

        public static bool operator !=(TSimModel left, TSimModel right) {
            return !Equals(left, right);
        }

        #endregion
    }
}