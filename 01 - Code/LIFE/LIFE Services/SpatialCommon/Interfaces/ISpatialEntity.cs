using System;

namespace SpatialCommon.Interfaces {

  public interface ISpatialEntity : ISpatialObject {

        /// <summary>
        ///   Return the information type specified by this object.
        /// </summary>
        /// <returns>Information type (as enum value).</returns>
        Enum GetCollisionType();
    }
}