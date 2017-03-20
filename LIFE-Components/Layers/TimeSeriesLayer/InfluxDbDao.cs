using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ConfigService;
using InfluxDB.Net;
using InfluxDB.Net.Contracts;
using InfluxDB.Net.Models;
using LIFE.API.Layer.Initialization;

namespace LIFE.Components.TimeSeriesLayer
{
    public class InfluxDbDao
    {
        private const string DatabaseName = "timeseries";
        private readonly string _dbColumnName;
        private readonly string _tableName;

        internal IInfluxDb InfluxDbClient;
        internal IConfigServiceClient ConfigService;

        internal InfluxDbDao(TInitData layerInitData, IConfigServiceClient configService, string hostName)
        {
            ConfigService = configService;

            var timeSeriesInitConfig = layerInitData.TimeSeriesInitInfo;
            _tableName = timeSeriesInitConfig.TableName;
            _dbColumnName = timeSeriesInitConfig.DatabaseColumnName;

            // retreive user and password of influxdb
            var influxDbUser = configService.Get("influxdb/user");
            var influxDbPassword = configService.Get("influxdb/password");

            InfluxDbClient = new InfluxDb("http://" + hostName + ":8086", influxDbUser, influxDbPassword);
        }

        public object GetValueForTimeRange(AggregationFunction aggregationFunction, DateTime start, DateTime end)
        {
            string influxFunctionName;
            if (AggregationFunction.None == aggregationFunction)
            {
                influxFunctionName = "LAST";
            }
            else if (aggregationFunction == AggregationFunction.Average)
            {
                influxFunctionName = "MEAN";
            }
            else if (aggregationFunction == AggregationFunction.Sum)
            {
                influxFunctionName = "SUM";
            }
            else
            {
                throw new ArgumentException($"AggregationFunction value {aggregationFunction} not allowed");
            }
            var startDateAsString = FormatDate(start);
            var endDateAsString = FormatDate(end);
            var whereClause = $"time <= '{endDateAsString}' " +
                              $"and time > '{startDateAsString}'";
            var query = $"select {influxFunctionName}(\"{_dbColumnName}\") " +
                        $"from {_tableName} where " +
                        whereClause;
            return QueryValue(query, whereClause);
        }

        private object QueryValue(string query, string whereClause)
        {
            try
            {
                var continueTask = InfluxDbClient.QueryAsync(DatabaseName, query);

                continueTask.Wait();

                IEnumerable<Serie> seriesList = continueTask.Result;
                if (!seriesList.Any())
                {
                    Console.Error.WriteLine($"No value found for time: {whereClause}");
                    return null;
                }
                IList<IList<object>> seriesValues = seriesList.First().Values;
                var currentValues = seriesValues.First();
                if (currentValues.Count < 2)
                {
                    Console.Error.WriteLine($"No value found for time: {whereClause}");
                    return null;
                }
                //We query the time column and the value column - therefore the first value is the datetime and the second our value
                var value = currentValues[1];
                return value;
            }
            catch (TaskCanceledException)
            {
                Console.Error.WriteLine("Catched a TaskCanceledException during TimeSeries Loading...");
                return null;
            }
        }

        private static string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", DateTimeFormatInfo.InvariantInfo);
        }
    }
}