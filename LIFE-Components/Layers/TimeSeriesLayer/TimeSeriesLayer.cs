using System;
using System.Collections.Concurrent;
using ConfigService;
using Hik.Communication.ScsServices.Service;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.API.Layer.TimeSeries;
using LIFE.Components.TimeSeriesLayer.Exceptions;

namespace LIFE.Components.TimeSeriesLayer
{
    public abstract class TimeSeriesLayer : ScsService, ITimeSeriesLayer
    {
        private readonly ConcurrentDictionary<DateTime, object> _valueCache =
            new ConcurrentDictionary<DateTime, object>();

        private const int NumberOfTicksToPreload = 10;
        internal string HostName = "influxdb";

        private DateTime _currentSimulationTime;
        private long _currentTick;
        private TimeSpan _oneTickTimeSpan;
        private TimeSpan? _queryTimeSpan;
        private readonly AggregationFunction _aggregationFunction;

        //TODO enable different start time for time series layer (not the simulation time)
        private DateTime _timeSeriesStartTime;

        private InfluxDbDao _influxDbDao;
        internal IConfigServiceClient ConfigService;

        protected TimeSeriesLayer(
            AggregationFunction function = AggregationFunction.None,
            TimeSpan? timeSpan = null)
        {
            if (timeSpan != null)
            {
                _queryTimeSpan = timeSpan.Value;
            }
            _aggregationFunction = function;
        }

        public long GetCurrentTick()
        {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick)
        {
            _currentTick = currentTick;

            //calculate current simulation time
            _currentSimulationTime = _timeSeriesStartTime.Add(TimeSpan.FromTicks(_oneTickTimeSpan.Ticks * currentTick));

            PreloadWithOffset(_currentSimulationTime);
        }

        public bool InitLayer
            (TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            AssertTimeSeriesInitInfosAreSet(layerInitData.TimeSeriesInitInfo);

            if (_queryTimeSpan == null)
            {
                _queryTimeSpan = layerInitData.OneTickTimeSpan;
            }

            //for testing: you can inject a ConfigService mock.
            if (ConfigService == null)
            {
                ConfigService = new ConfigServiceClient(layerInitData.MARSConfigAddress);
            }

            _influxDbDao = new InfluxDbDao(layerInitData, ConfigService, HostName);

            _oneTickTimeSpan = layerInitData.OneTickTimeSpan;
            _currentSimulationTime = layerInitData.SimulationWallClockStartDate;
            _timeSeriesStartTime = layerInitData.SimulationWallClockStartDate;

            InitialPreload(_currentSimulationTime);

            return true;
        }

        private static void AssertTimeSeriesInitInfosAreSet(TimeSeriesInitConfig timeSeriesInitConfig)
        {
            if (timeSeriesInitConfig == null)
                throw new InvalidTimeSeriesLayerInitConfigException("TimeSeriesInitConfig is null");
            if (string.IsNullOrEmpty(timeSeriesInitConfig.ClearColumnName))
                throw new InvalidTimeSeriesLayerInitConfigException("missing clear column name in time series layer");
            if (string.IsNullOrEmpty(timeSeriesInitConfig.TableName))
                throw new InvalidTimeSeriesLayerInitConfigException("missing table name in time series layer");
            if (string.IsNullOrEmpty(timeSeriesInitConfig.DatabaseColumnName))
                throw new InvalidTimeSeriesLayerInitConfigException(
                    "missing database column name in time series layer");
        }

        /// <summary>
        ///   Returns the value of the specified time series for the current simulation time.
        ///   By providing a TimeResolution and an AggregationFunction in the constructor for a concrete TimeSeriesLayer
        ///   you can adapt the way to query the current value.
        /// </summary>
        /// <example>
        /// This sample shows how to create a concrete TimeSeriesLayer with non default TimeResolution and AggregationFunction.
        /// <code>
        /// public class HourAverageTimeSeriesLayer : TimeSeriesLayer.TimeSeriesLayer
        ///    {
        ///        public HourAverageTimeSeriesLayer() : base(AggregationFunction.Average, TimeResolution.Hour)
        ///        {
        ///        }
        ///    }
        /// </code>
        /// </example>
        /// <returns>The value for current simulation time depending on the time resolution.</returns>
        public object GetValueForCurrentSimulationTime()
        {
            if (_valueCache.ContainsKey(_currentSimulationTime))
            {
                return _valueCache[_currentSimulationTime];
            }
            return null;
        }

        /// <summary>
        ///   At the beginning of the simulation we preload x values to avoid that the simulation is faster than the preloading
        ///   functionality.
        /// </summary>
        /// <param name="requestTime">Request time.</param>
        private void InitialPreload(DateTime requestTime)
        {
            for (var i = 0; i <= NumberOfTicksToPreload; i++)
            {
                var additionalTimeSpan = new TimeSpan(_oneTickTimeSpan.Ticks * i);
                var dateTimeToQuery = requestTime.Add(additionalTimeSpan);
                LoadValue(dateTimeToQuery);
            }
        }

        /// <summary>
        ///   After initial preload we always query with an offset
        /// </summary>
        /// <param name="requestTime">Request time.</param>
        private void PreloadWithOffset(DateTime requestTime)
        {
            var additionalTimeSpan = new TimeSpan(_oneTickTimeSpan.Ticks * NumberOfTicksToPreload);
            LoadValue(requestTime.Add(additionalTimeSpan));
        }

        private void LoadValue(DateTime requestTime)
        {
            var value = _influxDbDao.GetValueForTimeRange(_aggregationFunction, GetRangeStartDate(requestTime),
                requestTime);
            _valueCache.TryAdd(requestTime, value);
        }

        private DateTime GetRangeStartDate(DateTime simulationTime)
        {
            return simulationTime.Subtract(_queryTimeSpan.Value);
        }
    }
}