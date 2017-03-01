using System;
using System.Reflection;
using LIFE.API.Layer.Initialization;
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
            var timeSeriesLayer = new ConcreteTimeSeriesLayer();

            var hostNameField = timeSeriesLayer.GetType().GetField("hostName", BindingFlags.NonPublic | BindingFlags.Instance);
            hostNameField.SetValue(timeSeriesLayer, "artifactory.mars.haw-hamburg.de-influxdb");

            TInitData initData = createTInitDataWithoutTimeSeriesInitInfo();
            initData.TimeSeriesInitInfo = new TimeSeriesInitConfig("t1sdfghjkl", "c_1", "temperature");
            timeSeriesLayer.InitLayer(initData, null, null);

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