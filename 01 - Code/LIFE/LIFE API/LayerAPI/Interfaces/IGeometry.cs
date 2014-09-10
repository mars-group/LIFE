using CommonTypes.DataTypes;

namespace LayerAPI.Interfaces
{
    public interface IGeometry {

        Vector3f GetPosition();

        Vector3f GetDimensionQuad();

        Vector3f GetDirectionOfQuad();

        /// <summary>
        ///   Check, if a given position is inside this perception range.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True, if position is in range, false otherwise.</returns>
        bool IsInRange(Vector3f position);
    }
}
