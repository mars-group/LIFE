using System;
using System.Runtime.Serialization;

namespace LIFE.Components.TimeSeriesLayer.Exceptions
{
    [Serializable]
    public class InvalidTimeSeriesLayerInitConfigException : Exception
    {
        public InvalidTimeSeriesLayerInitConfigException(string msg) : base(msg)
        {
        }

        public InvalidTimeSeriesLayerInitConfigException(SerializationInfo serializationInfo, StreamingContext context)
        {
        }
    }
}