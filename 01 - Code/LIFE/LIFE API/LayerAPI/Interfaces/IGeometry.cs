using CommonTypes.DataTypes;

namespace LayerAPI.Interfaces
{
    public interface IGeometry {

        Vector GetPosition();

        Vector GetDimensionQuad();

        Vector GetDirectionOfQuad();

        /// <summary>
        ///   Check, if a given position is inside this perception range.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True, if position is in range, false otherwise.</returns>
        bool IsInRange(Vector position);
    }
}
