// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;

namespace LCConnector.TransportTypes {
    [Serializable]
    public class TLayerDescription : IEquatable<TLayerDescription> {
        /// <summary>
        ///     The layer's name.
        /// </summary>
        public string Name { get { return _name; } set { } }

        /// <summary>
        ///     In a version xx.yyyyyy, this is the xx.
        /// </summary>
        public int MajorVersion { get { return _majorVersion; } set { } }

        /// <summary>
        ///     /// In a version xx.yyyyyy, this is the yyyyyy.
        /// </summary>
        public int MinorVersion { get { return _minorVersion; } set { } }

        /// <summary>
        ///     The filename of the original dll holding the binary code.
        /// </summary>
        public string FileName { get { return _fileName; } set { } }

        private readonly string _name;
        private readonly int _majorVersion;
        private readonly int _minorVersion;
        private readonly string _fileName;

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