using System;
using LIFE.Components.TimeSeriesLayer;

namespace LIFE.Components.Layers
{
    public class ConcreteTimeSeriesLayer : TimeSeriesLayer.TimeSeriesLayer
    {
    }

    public class HourAverageTimeSeriesLayer : TimeSeriesLayer.TimeSeriesLayer
    {
        public HourAverageTimeSeriesLayer() : base(AggregationFunction.Average, TimeSpan.FromHours(1))
        {
        }
    }

    public class HourSumTimeSeriesLayer : TimeSeriesLayer.TimeSeriesLayer
    {
        public HourSumTimeSeriesLayer() : base(AggregationFunction.Sum, TimeSpan.FromHours(1))
        {
        }
    }
}