using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace SimulationPlanParserTest
{
    [TestFixture]
    public class Tests
    {
        private JObject scenarioConfig;

        [SetUp]
        public void Setup()
        {
            var json = File.ReadAllText(Directory.GetCurrentDirectory() + "/smsExample.json");
            scenarioConfig = JObject.Parse(json);
        }

        [Test]
        public void Test1()
        {
            var globalParams = scenarioConfig["ParameterizationDescription"]["Global"];
            var startDate = DateTime.Parse(globalParams["SimulationStartDateTime"].ToString());
            var endDate = DateTime.Parse(globalParams["SimulationEndDateTime"].ToString());
            var deltaT = int.Parse(globalParams["DeltaT"].ToString());
            var tickCount = (endDate - startDate).TotalMilliseconds / deltaT;

            Console.WriteLine($"deltaT: {deltaT}, startDate: {startDate}, endDate: {endDate}, end-start in ms: {(endDate - startDate).TotalMilliseconds}");
            Console.WriteLine(tickCount);

        }
    }
}


