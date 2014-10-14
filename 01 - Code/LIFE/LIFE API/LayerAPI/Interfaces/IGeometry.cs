namespace LayerAPI.Interfaces {
    #region Namespace imports

    using CommonTypes.TransportTypes;

    #endregion

    public interface IGeometry {
        /// <summary>
        ///     The center position of this geometry.
        /// </summary>
        /// <returns>The center position as transport vector.</returns>
        TVector GetPosition();

        /// <summary>
        ///     The axis-aligned-bounding-box for this geometry.
        /// </summary>
        /// <returns>The corresponding AABB object.</returns>
        AABB GetAABB();
  
        /// <summary>
        ///     Check, if a given position is inside this perception range.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True, if position is in range, false otherwise.</returns>
        bool IsInRange(TVector position);
    }
}