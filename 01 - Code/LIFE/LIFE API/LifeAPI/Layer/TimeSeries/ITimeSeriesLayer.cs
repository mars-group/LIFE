namespace LifeAPI.Layer.TimeSeries {

    public interface ITimeSeriesLayer : ISteppedLayer {
        /// <summary>
        /// Get a value for the current simulation time.
        /// </summary>
        /// <returns>The value for the current simulation time. Returns null, if the stored value is empty or explicit null. 
        /// Returns a NoDataFound object, if the there is no value for the requested simulation time</returns>
        object GetValueForCurrentSimulationTime();
    }

}