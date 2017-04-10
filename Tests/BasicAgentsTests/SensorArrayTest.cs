using LIFE.Components.Agents.BasicAgents.Perception;
using NUnit.Framework;

namespace BasicAgentsTests
{
    internal class Sensor1 : ISensor
    {
        public object Sense()
        {
            return 42;
        }
    }

    internal class Sensor2 : ISensor
    {
        public object Sense()
        {
            return "42";
        }
    }


    [TestFixture]
    public class SensorArrayTest
    {
        [Test] // Legacy sensor creation and access.
        public void SensorTest_01()
        {
            var sa = new SensorArray();
            sa.AddSensor(new Sensor1());
            sa.SenseAll();
            var result = sa.Get<Sensor1, int>();
            Assert.True(result == 42);
        }


        [Test] // Create and use a named sensor.
        public void SensorTest_02()
        {
            var sa = new SensorArray();
            sa.AddSensor(new Sensor1(), "s1");
            sa.SenseAll();
            var result = sa.Get<int>("s1");
            Assert.True(result == 42);
        }


        [Test] // Delete a sensor.
        public void SensorTest_03()
        {
            var sa = new SensorArray();
            sa.AddSensor(new Sensor1(), "s1");
            sa.DeleteSensor("s1");
            sa.SenseAll();
            var ex = Assert.Throws<SensorNotFoundException>(delegate { sa.Get<int>("s1"); });
            Assert.That(ex.Message, Is.EqualTo("[SensorArray] No value for sensor 's1' listed!"));
        }
    }
}