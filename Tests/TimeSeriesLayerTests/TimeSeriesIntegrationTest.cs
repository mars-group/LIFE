using System;
using System.Collections.Generic;
using System.Reflection;
using ConfigService;
using InfluxDB.Net;
using InfluxDB.Net.Enums;
using InfluxDB.Net.Infrastructure.Influx;
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
        private const string DatabaseName = "timeseries";
        private const string Measurement = "my_measurement";

        [Test]
        public void IntegrationTest()
        {
            // given
            InitDatabaseValues();

            var timeSeriesLayer = InitTimeSeriesLayer();

            // when
            var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();

            // then
            Assert.AreEqual(valueForCurrentSimulationTime, 123);
        }

        private static void InitDatabaseValues()
        {
            Console.WriteLine("-------------");
            var influxDb = new InfluxDb("http://" + InfluxDbHost + ":8086", User, Password, InfluxVersion.v096);
            Console.WriteLine("Lets create the Database");

            var databaseAsync = influxDb.CreateDatabaseAsync(DatabaseName);
            databaseAsync.Wait();

            var point = new Point()
            {
                Measurement = Measurement,
                Precision = TimeUnit.Milliseconds,
                Timestamp = DateTime.ParseExact("2000-01-01 00:00", "yyyy-MM-dd HH:mm", null),
            };
            point.Fields.Add("temperature", 123);
            var writeResponse = influxDb.WriteAsync(DatabaseName, point);

            var taskStatus = databaseAsync.Status;
            Console.WriteLine("Status for creating database: " + taskStatus);

            var writeStatus = writeResponse.Status;
            Console.WriteLine("Status for value write: " + writeStatus);
        }

        private ConcreteTimeSeriesLayer InitTimeSeriesLayer()
        {
            var timeSeriesLayer = new ConcreteTimeSeriesLayer();

            var hostNameField = timeSeriesLayer.GetType().GetField("HostName", BindingFlags.NonPublic | BindingFlags.Instance);
            hostNameField.SetValue(timeSeriesLayer, InfluxDbHost);

            var configServiceClient = Substitute.For<IConfigServiceClient>();
            configServiceClient.Get("influxdb/user").Returns(User);
            configServiceClient.Get("influxdb/password").Returns(Password);

            var configServiceField = timeSeriesLayer.GetType()
                .GetField("ConfigService", BindingFlags.NonPublic | BindingFlags.Instance);
            configServiceField.SetValue(timeSeriesLayer, configServiceClient);

            TInitData initData = createTInitDataWithoutTimeSeriesInitInfo();
            initData.TimeSeriesInitInfo = new TimeSeriesInitConfig("t1sdfghjkl", "c_1", "temperature");
            timeSeriesLayer.InitLayer(initData, null, null);
            return timeSeriesLayer;
        }

        private TInitData createTInitDataWithoutTimeSeriesInitInfo()
        {
            return new TInitData(false, TimeSpan.FromDays(1), DateTime.ParseExact("2000-01-01 00:00", "yyyy-MM-dd HH:mm", null), Guid.NewGuid(), "");
        }
    }
}