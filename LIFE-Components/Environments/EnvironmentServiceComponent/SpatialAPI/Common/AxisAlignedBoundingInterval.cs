using System;

namespace LIFE.Components.ESC.SpatialAPI.Common {

  public class AxisAlignedBoundingInterval {

    private readonly double _min;
    private readonly double _max;


    public AxisAlignedBoundingInterval(double val1, double val2) {
      // Set smaller value as minimum.
      if (val1 < val2) {
        _min = val1;
        _max = val2;
      } else {
        _min = val2;
        _max = val1;
      }
    }


    /// <summary>
    ///   Checks for a collision of this and another axis interval.
    ///   In total, there are 12 possible cases of interval alignment.
    ///   Eight of them shall be classified as "overlap".
    /// </summary>
    /// <param name="other">The other interval to check.</param>
    /// <returns>"True": collision, "false": no collision.</returns>
    public bool Collide(AxisAlignedBoundingInterval other) {
      if (ReferenceEquals(this, other)) return false;    //| Overlap with ...
      if ((_min >= other._min && _min < other._max)  ||  //| 1) Left interval
          (other._min >= _min && other._min < _max)  ||  //| 2) Right interval
          (_min <= other._min && _max >= other._max) ||  //| 3) inner interval
          (other._min <= _min && other._max >= _max))    //| 4) outer interval.
            return true;
      return false;
    }


    public bool Equals(AxisAlignedBoundingInterval other) {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Math.Abs(_min - other._min) <= float.Epsilon
          && Math.Abs(_max - other._max) <= float.Epsilon;
    }


    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != GetType()) return false;
      return Equals((AxisAlignedBoundingInterval)obj);
    }


    public override int GetHashCode() {
      unchecked {
        return (int)(_min * 397) ^ (int)_max;
      }
    }


    public static bool operator ==(AxisAlignedBoundingInterval left, AxisAlignedBoundingInterval right) {
      return Equals(left, right);
    }


    public static bool operator !=(AxisAlignedBoundingInterval left, AxisAlignedBoundingInterval right) {
      return !Equals(left, right);
    }


    public override string ToString() {
      return String.Format("({0,6:0.00} →{1,6:0.00})", _min,_max);
    }
  }
}
