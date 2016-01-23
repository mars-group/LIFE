// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;

namespace CommonTypes.TransportTypes {
    /// <summary>
    /// </summary>
    [Serializable]
    public class TSimModel : IEquatable<TSimModel> {
        public string Path { get { return _path; } set { } }

        public string Name { get { return _name; } set { } }
        private readonly string _path;
        private readonly string _name;

        public TSimModel(string path) {
            _path = path;
            string[] tmp = path.Split(System.IO.Path.DirectorySeparatorChar);
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
            if (obj.GetType() != GetType()) return false;
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