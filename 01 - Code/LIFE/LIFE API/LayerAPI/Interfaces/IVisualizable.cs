using System.Collections.Generic;
using GeoAPI.Geometries;

namespace LayerAPI.Interfaces
{
    /// <summary>
    /// Implement this interface if you want your Layer to be visualized after each tick
    /// </summary>
	public interface IVisualizable {
        /// <summary>
        /// Returns every entity in the whole environment
        /// </summary>
        /// <returns>A list of everything visualizable in the whole environment, empty list if nothing is present</returns>
        List<BasicVisualizationMessage> GetVisData();

        /// <summary>
        /// Returns every entity which is associated with the intersection of the provided geometry
        /// and the environment
        /// </summary>
        /// <param name="geometry">The geometry to intersect with.</param>
        /// <returns>A list of everything visualizable in the whole environment, empty list if nothing is present</returns>
	    List<BasicVisualizationMessage> GetVisData(IGeometry geometry);

	}


}

