﻿using System;

namespace LayerAPI.Interfaces {
    public interface IGenericDataSource {
        
        /// <summary>
        ///   Get information of a given type from a data source.
        /// </summary>
        /// <param name="informationType">The information type to query (enum).</param>
        /// <param name="geometry">A geometry object representing the agent's perception.</param>
        /// <returns>An object containing the asked information. Obviously, it has to be casted.</returns>
        Object GetData(int informationType, IGeometry geometry);
    }
}