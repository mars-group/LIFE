using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ConfigService;
using InfluxDB.Net;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Models;
using LIFE.API.Layer.Initialization;
using NSubstitute;
using NUnit.Framework;

namespace LIFE.Components.Layers
{
    [TestFixture]
    public class TimeSeriesIntegrationTest
    {
        private const string User = "mars";
        private const string Password = "sram2015";

        private const string InfluxDbHost = "artifactory.mars.haw-hamburg.de-influxdb";
        //private const string InfluxDbHost = "localhost";

        private const string DatabaseName = "timeseries";

        private const string Measurement = "my_measurement";
        private const string Column = "c_0";

        [Test]
        public void ThatCurrentValueIsRetrievedSuccessfully()
        {
            // given
            InitDatabaseWithValues();
            var timeSeriesLayer = InitTimeSeriesLayer(new ConcreteTimeSeriesLayer());

            // when
            var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();

            // then
            Assert.AreEqual(20.5, valueForCurrentSimulationTime);
        }

        [Test]
        public void ThatCurrentValueIsRetrievedSuccessfullyAfter15Ticks()
        {
            // given
            InitDatabaseWithValues();
            var timeSeriesLayer = InitTimeSeriesLayer(new ConcreteTimeSeriesLayer());

            for (int i = 1; i <= 15; i++)
            {
                timeSeriesLayer.SetCurrentTick(i);
            }

            // when
            var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();

            // then
            Assert.AreEqual(22.0, valueForCurrentSimulationTime);
        }

        [Test]
        public void ThatAverageValuesAreRetrievedSuccessfully()
        {
            // given
            InitDatabaseWithValues();
            var timeSeriesLayer = InitTimeSeriesLayer(new HourAverageTimeSeriesLayer());

            // when
            var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();

            // then
            Assert.AreEqual(20.25, valueForCurrentSimulationTime);
        }

        [Test]
        public void ThatAggregatedValuesAreRetrievedSuccessfully()
        {
            // given
            InitDatabaseWithValues();
            var timeSeriesLayer = InitTimeSeriesLayer(new HourSumTimeSeriesLayer());

            // when
            var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();

            // then
            Assert.AreEqual(121.5, valueForCurrentSimulationTime);
        }

        [Test]
        public void That10000AgentsCanQueryTheCurrentValue()
        {
            // given
            InitDatabaseWithValues();
            var timeSeriesLayer = InitTimeSeriesLayer(new HourSumTimeSeriesLayer());

            // when
            Parallel.For(1, 10000, i =>
            {
                var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();
                // then
                Assert.AreEqual(121.5, valueForCurrentSimulationTime);
            });
        }

        private static void InitDatabaseWithValues()
        {
            var influxDb = new InfluxDb("http://" + InfluxDbHost + ":8086", User, Password, InfluxVersion.v096);

            var databaseAsync = influxDb.CreateDatabaseAsync(DatabaseName);
            databaseAsync.Wait();

            var points = new List<Point>();
            var startTime = DateTime.ParseExact("2000-01-01 00:00", "yyyy-MM-dd HH:mm", null);
            var temperature = 19.9;
            for (var i = 0; i < 30; i++)
            {
                var point = new Point
                {
                    Measurement = Measurement,
                    Precision = TimeUnit.Milliseconds,
                    Timestamp = startTime
                };
                point.Fields.Add(Column, temperature);
                points.Add(point);
                startTime = startTime.AddMinutes(10);
                temperature = temperature + 0.1;
            }
            var writeResponse = influxDb.WriteAsync(DatabaseName, points.ToArray());
            writeResponse.Wait();
        }

        private TimeSeriesLayer.TimeSeriesLayer InitTimeSeriesLayer(TimeSeriesLayer.TimeSeriesLayer timeSeriesLayer)
        {
            var hostNameField = timeSeriesLayer.GetType()
                .GetField("HostName", BindingFlags.NonPublic | BindingFlags.Instance);
            hostNameField.SetValue(timeSeriesLayer, InfluxDbHost);

            var configServiceClient = Substitute.For<IConfigServiceClient>();
            configServiceClient.Get("influxdb/user").Returns(User);
            configServiceClient.Get("influxdb/password").Returns(Password);

            var configServiceField = timeSeriesLayer.GetType()
                .GetField("ConfigService", BindingFlags.NonPublic | BindingFlags.Instance);
            configServiceField.SetValue(timeSeriesLayer, configServiceClient);

            var initData = CreateTInitDataWithoutTimeSeriesInitInfo();
            initData.TimeSeriesInitInfo = new TimeSeriesInitConfig(Measurement, Column, "temperature");
            timeSeriesLayer.InitLayer(initData, null, null);
            return timeSeriesLayer;
        }

        private static TInitData CreateTInitDataWithoutTimeSeriesInitInfo()
        {
            return new TInitData(false, TimeSpan.FromMinutes(10),
                DateTime.ParseExact("2000-01-01 01:00", "yyyy-MM-dd HH:mm", null), Guid.NewGuid(), "");
        }
    }
}