﻿using System;

namespace LCConnector.TransportTypes {
    [Serializable]
    public class TLayerDescription : IEquatable<TLayerDescription> {
        private string _name;
        private int _majorVersion;
        private int _minorVersion;
        private string _fileName;

        /// <summary>
        ///     The layer's name.
        /// </summary>
        public string Name {
            get { return _name; }
            set { }
        }

        /// <summary>
        ///     In a version xx.yyyyyy, this is the xx.
        /// </summary>
        public int MajorVersion {
            get { return _majorVersion; }
            set { }
        }

        /// <summary>
        ///     /// In a version xx.yyyyyy, this is the yyyyyy.
        /// </summary>
        public int MinorVersion {
            get { return _minorVersion; }
            set { }
        }

        /// <summary>
        ///     The filename of the original dll holding the binary code.
        /// </summary>
        public string FileName {
            get { return _fileName; }
            set { }
        }

        public TLayerDescription(string name, int majorVersion, int minorVersion, string fileName) {
            _name = name;
            _majorVersion = majorVersion;
            _minorVersion = minorVersion;
            _fileName = fileName;
        }

        #region Object Contracts

        /// <summary>
        ///     Parameterless constructor for serialization. (DO NOT USE!)
        /// </summary>
        public TLayerDescription() {}

        public bool Equals(TLayerDescription other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && MajorVersion == other.MajorVersion &&
                   MinorVersion == other.MinorVersion;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
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