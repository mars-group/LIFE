using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCTestLayer
{
    public class AxisAlignedBoundingInterval {
        private readonly float _min;
        private readonly float _max;

        public AxisAlignedBoundingInterval(float val1, float val2) {
          
          // Set smaller value as minimum.
          if (val1 < val2) {
            _min = val1;
            _max = val2;
          } else {
            _min = val2;
            _max = val1;
          }
        }

        public bool Collide(AxisAlignedBoundingInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this._min <= other._min) {
                return this._max >= other._min;
            }
            // so this._min > other._min
            return this._min <= other._max;
        }

        public bool Equals(AxisAlignedBoundingInterval other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this._min == other._min && this._max == other._max;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AxisAlignedBoundingInterval)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (int)(_min * 397) ^ (int)_max;
            }
        }

        public static bool operator ==(AxisAlignedBoundingInterval left, AxisAlignedBoundingInterval right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AxisAlignedBoundingInterval left, AxisAlignedBoundingInterval right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return String.Format("({0}-{1})", _min,_max);
        }
    }
}
