using System;
using System.Reflection;
using ConfigService;
using InfluxDB.Net;
using InfluxDB.Net.Enums;
using LIFE.API.Layer.Initialization;
using NSubstitute;
using NUnit.Framework;

namespace LIFE.Components.Layers
{
    [TestFixture]
    public class TimeSeriesIntegrationTest
    {

        [Test]
        public void integrationTest()
        {
            // given
            const string user = "mars";
            const string password = "sram2015";
            const string influxDbHost = "artifactory.mars.haw-hamburg.de-influxdb";

            var timeSeriesLayer = new ConcreteTimeSeriesLayer();

            var hostNameField = timeSeriesLayer.GetType().GetField("HostName", BindingFlags.NonPublic | BindingFlags.Instance);
            hostNameField.SetValue(timeSeriesLayer, influxDbHost);

            var configServiceClient = Substitute.For<IConfigServiceClient>();
            configServiceClient.Get("influxdb/user").Returns(user);
            configServiceClient.Get("influxdb/password").Returns(password);

            var configServiceField = timeSeriesLayer.GetType().GetField("ConfigService", BindingFlags.NonPublic | BindingFlags.Instance);
            configServiceField.SetValue(timeSeriesLayer, configServiceClient);

            TInitData initData = createTInitDataWithoutTimeSeriesInitInfo();
            initData.TimeSeriesInitInfo = new TimeSeriesInitConfig("t1sdfghjkl", "c_1", "temperature");
            timeSeriesLayer.InitLayer(initData, null, null);

            var influxDb = new InfluxDb("http://" + influxDbHost + ":8086", user, password, InfluxVersion.v096);
            influxDb.CreateDatabaseAsync("timeseries").Wait();

            // when
            var valueForCurrentSimulationTime = timeSeriesLayer.GetValueForCurrentSimulationTime();

            // then
            Assert.AreEqual(valueForCurrentSimulationTime, 1337);
        }


        private TInitData createTInitDataWithoutTimeSeriesInitInfo()
        {
            return new TInitData(false, TimeSpan.FromDays(1), DateTime.ParseExact("2000-04-22 00:00", "yyyy-MM-dd HH:mm", null), Guid.NewGuid(), "");
        }
    }
}