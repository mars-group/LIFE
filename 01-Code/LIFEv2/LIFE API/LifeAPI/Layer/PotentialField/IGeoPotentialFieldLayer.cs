using System;
using GeoCommon;

namespace LifeAPI.Layer.PotentialField
{
    public interface IGeoPotentialFieldLayer : ILayer
    {
        /// <summary>
        /// Searches for the closest cell with full potential (with breadth first search).
        /// This method should be used if the current cell has no potential and you want to search with endless sight.
        /// Normally #ExploreClosestFullPotentialField should be used, because the potential propagation represents
        /// an agents sight range.
        /// </summary>
        /// <param name="lat">latitude value of an agent</param>
        /// <param name="lon">longitude value of an agent</param>
        /// <returns>GeoCoordinate of the closest cell with full potential</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is no cell with full potential. A potential field without a cell with full potential is useless.</exception>
        GeoCoordinate ExploreClosestWithEndlessSight(double lat, double lon);

        /// <summary>
        /// Searches for the closest cell with full potential.
        /// </summary>
        /// <param name="lat">latitude value of an agent</param>
        /// <param name="lon">longitude value of an agent</param>
        /// <returns>GeoCoordinate of the closest field with full potential or null if the current field has no potential</returns>
        GeoCoordinate ExploreClosestFullPotentialField(double lat, double lon);

        /// <summary>
        /// Returns if the current cell has full potential.
        /// </summary>
        /// <param name="lat">latitude value of an agent</param>
        /// <param name="lon">longitude value of an agent</param>
        /// <returns>If the current cell has full potential</returns>
        bool HasFullPotential(double lat, double lon);
    }
}