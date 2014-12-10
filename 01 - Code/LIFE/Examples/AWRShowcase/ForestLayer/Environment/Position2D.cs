// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 09.07.2014
//  *******************************************************/

using System;

namespace ForestLayer.Environment {
    internal class Position2D : IEquatable<Position2D> {
        public int X { get; set; }

        public int Y { get; set; }

        public Position2D(int x, int y) {
            X = x;
            Y = y;
        }

        #region IEquatable<Position2D> Members

        public bool Equals(Position2D other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Y == other.Y && X == other.X;
        }

        #endregion

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Position2D) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Y*397) ^ X;
            }
        }

        public static bool operator ==(Position2D left, Position2D right) {
            return Equals(left, right);
        }

        public static bool operator !=(Position2D left, Position2D right) {
            return !Equals(left, right);
        }
    }
}