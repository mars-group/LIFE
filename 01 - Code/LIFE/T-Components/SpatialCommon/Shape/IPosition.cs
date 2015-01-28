using SpatialCommon.Transformation;

namespace SpatialCommon.Shape {
    
    /// <summary>
    /// The interface for an object, that can be contained by the environment.
    /// </summary>
    public interface IPosition {

        /// <summary>
        /// The object's position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// This event must be called, after the object's position has been changed.
        /// </summary>
        /// <remarks>Not firing this event will result in the environment giving inconsistent results.</remarks>
        event PositionChangedEvent PositionChanged;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="changedPosition"></param>
    public delegate void PositionChangedEvent(IPosition changedPosition);
}