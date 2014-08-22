using System;

namespace ESCTestLayer.Entities
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

        public bool Collide(AxisAlignedBoundingInterval other) {
          
          /* In total, there are 12 possible cases of interval alignment.
             Eight of them shall be classified as "overlap".              */
          if (ReferenceEquals(this, other)) return false;    //| Overlap with ...  
          if ((_min >= other._min && _min < other._max)  ||  //| 1) left interval
              (other._min >= _min && other._min < _max)  ||  //| 2) right interval
              (_min <= other._min && _max >= other._max) ||  //| 3) inner interval
              (other._min <= _min && other._max >= _max))    //| 4) outer interval.
                return true; // (). 
          return false;


          /*
          if (ReferenceEquals(null, other)) return false;
          if (ReferenceEquals(this, other)) return false; //true; Skip own reference.
            if (this._min <= other._min) {
                return this._max >= other._min;
            }
            // so this._min > other._min
            return this._min <= other._max;
           */
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
            return String.Format("({0,6:0.00} →{1,6:0.00})", _min,_max);
        }
    }
}
