using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConfigService;
using Hik.Communication.ScsServices.Service;
using InfluxDB.Net;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Models;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.API.Layer.TimeSeries;
using LIFE.Components.TimeSeriesLayer.Exceptions;

[assembly: InternalsVisibleTo("LIFETimeSeriesLayerTest")]

namespace LIFE.Components.TimeSeriesLayer
{
    public abstract class TimeSeriesLayer : ScsService, ITimeSeriesLayer
    {
        public enum TimeResolution
        {
            Exact,
            Minute,
            Hour,
            Day,
            Month
        }

        private readonly ConcurrentDictionary<DateTime, object> _valueCache =
            new ConcurrentDictionary<DateTime, object>();

        private DateTime _currentSimulationTime;

        private long _currentTick;
        private readonly string _databaseName = "timeseries";
        private string _dbColumnName;
        private TimeSpan _oneTickTimeSpan;

        internal string hostName = "influxdb";

        private string _tableName;

        //TODO enable different start time for time series layer (not the simulation time)
        private DateTime _timeSeriesStartTime;

        internal IInfluxDb InfluxDbClient;
        private const int NumberOfTicksToPreload = 10;

        internal IConfigServiceClient MarsConfigService { get; set; }

        #region ITimeLineLayer Members

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
            var timeSeriesInitConfig = layerInitData.TimeSeriesInitInfo;
            AssertTimeSeriesInitInfosAreSet(timeSeriesInitConfig);

            _tableName = timeSeriesInitConfig.TableName;
            _dbColumnName = timeSeriesInitConfig.DatabaseColumnName;

            _oneTickTimeSpan = layerInitData.OneTickTimeSpan;
            _currentSimulationTime = layerInitData.SimulationWallClockStartDate;
            _timeSeriesStartTime = layerInitData.SimulationWallClockStartDate;

            //for testing: you can inject a ConfigService mock.
            if (MarsConfigService == null)
                MarsConfigService = new ConfigServiceClient(layerInitData.MARSConfigAddress);
            // retreive port, user and password of influxdb
            var influxDbUser = MarsConfigService.Get("influxdb/user");
            var influxDbPassword = MarsConfigService.Get("influxdb/password");

            Console.WriteLine("----------------" + hostName);

            InfluxDbClient = new InfluxDb("http://" + hostName + ":8086", influxDbUser, influxDbPassword,
                InfluxVersion.v096);

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
        ///   By providing a TimeResolution you can handle different time scales between simulation and time series.
        ///   Example: Time series has a monthly resolution (value for the first day of the month), but the simulation has a daily
        ///   resolution.
        ///   Set TimeResolution.Month as time resolution and for every day of a month this method will return the monthly value.
        /// </summary>
        /// <returns>The value for current simulation time depending on the time resolution.</returns>
        /// <param name="timeResolution">Time resolution.</param>
        public object GetValueForCurrentSimulationTime(TimeResolution timeResolution)
        {
            var timeToQuery = _currentSimulationTime;
            switch (timeResolution)
            {
                case TimeResolution.Exact:
                    break;
                case TimeResolution.Month:
                    timeToQuery = new DateTime(_currentSimulationTime.Year, _currentSimulationTime.Month, 1);
                    break;
                default:
                    throw new NotImplementedException("Implement other time Resolutions");
            }

            //use cache
            if (_valueCache.ContainsKey(timeToQuery))
                return _valueCache[timeToQuery];
            return null;
        }

        /// <summary>
        ///   Returns the value of the specified time series for the current simulation time.
        /// </summary>
        /// <returns>The value for current simulation time.</returns>
        public object GetValueForCurrentSimulationTime()
        {
            return GetValueForCurrentSimulationTime(TimeResolution.Exact);
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
            var formatedRequestTime = requestTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK",
                DateTimeFormatInfo.InvariantInfo);
            try
            {
                var continueTask = InfluxDbClient.QueryAsync(_databaseName,
                    $"select last({_dbColumnName}) from {_tableName} where time <= '{formatedRequestTime}Z'");

                continueTask.Wait();

                IEnumerable<Serie> seriesList = continueTask.Result;
                if (!seriesList.Any())
                {
                    Console.Error.WriteLine($"Na value found for: {formatedRequestTime}");
                    _valueCache.TryAdd(requestTime, null);
                }
                IList<IList<object>> seriesValues = seriesList.First().Values;
                var currentValues = seriesValues.First();
                if (currentValues.Count < 2)
                {
                    Console.Error.WriteLine($"Na value found for: {formatedRequestTime}");
                    _valueCache.TryAdd(requestTime, null);
                }
                else
                {
                    //We query the time column and the value column - therefore the first value is the datetime and the second our value
                    var date = (DateTime) currentValues[0];
                    var value = currentValues[1];
                    _valueCache.TryAdd(date, value);
                }
            }
            catch (TaskCanceledException)
            {
                Console.Error.WriteLine("Catched a TaskCanceledException during TimeSeries Loading...");
            }
        }

        #endregion
    }
}