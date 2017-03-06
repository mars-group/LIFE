using System;
using LIFE.API.Layer.Initialization;
using LIFE.Components.TimeSeriesLayer.Exceptions;
using NUnit.Framework;

namespace LIFE.Components.Layers
{
    [TestFixture]
    public class TimeSeriesLayerTest
    {
        [Test]
        public void ShouldRejectNullAsInit()
        {
            var timeSeriesLayer = new ConcreteTimeSeriesLayer();
            TInitData invalidTInitDataWithoutTimeSeriesInit = createTInitDataWithoutTimeSeriesInitInfo();

            Assert.Throws
            (typeof(InvalidTimeSeriesLayerInitConfigException),
                delegate { timeSeriesLayer.InitLayer(invalidTInitDataWithoutTimeSeriesInit, null, null); });
        }

        [Test]
        public void ShouldRejectInitWithoutTableName()
        {
            var timeSeriesLayer = new ConcreteTimeSeriesLayer();
            TInitData invalidTInitData = createTInitDataWithoutTimeSeriesInitInfo();
            invalidTInitData.TimeSeriesInitInfo = new TimeSeriesInitConfig(null, "c_1", "temperature");

            Assert.Throws
            (typeof(InvalidTimeSeriesLayerInitConfigException),
                delegate { timeSeriesLayer.InitLayer(invalidTInitData, null, null); });
        }

        [Test]
        public void ShouldRejectInitWithoutDatabaseColumnName()
        {
            var timeSeriesLayer = new ConcreteTimeSeriesLayer();
            TInitData invalidTInitData = createTInitDataWithoutTimeSeriesInitInfo();
            invalidTInitData.TimeSeriesInitInfo = new TimeSeriesInitConfig("my_table", null, "temperature");

            Assert.Throws
            (typeof(InvalidTimeSeriesLayerInitConfigException),
                delegate { timeSeriesLayer.InitLayer(invalidTInitData, null, null); });
        }

        [Test]
        public void ShouldRejectInitWithoutClearColumnName()
        {
            var timeSeriesLayer = new ConcreteTimeSeriesLayer();
            TInitData invalidTInitData = createTInitDataWithoutTimeSeriesInitInfo();
            invalidTInitData.TimeSeriesInitInfo = new TimeSeriesInitConfig("my_table", "c_1", null);

            Assert.Throws
            (typeof(InvalidTimeSeriesLayerInitConfigException),
                delegate { timeSeriesLayer.InitLayer(invalidTInitData, null, null); });
        }

        private TInitData createTInitDataWithoutTimeSeriesInitInfo()
        {
            return new TInitData(false, TimeSpan.FromDays(1),
                DateTime.ParseExact("2000-04-22 00:00", "yyyy-MM-dd HH:mm", null), Guid.NewGuid(), "");
        }
    }
}