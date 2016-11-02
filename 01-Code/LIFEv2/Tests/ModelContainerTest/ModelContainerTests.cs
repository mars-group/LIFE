using System.Collections.Generic;
using LCConnector.TransportTypes;
using ModelContainer.Implementation.Entities;
using NUnit.Framework;

namespace ModelContainerTest
{
    [TestFixture]
    public class Tests
    {

        /// <summary>
        ///     Construct a graph as pictured in Tests/Visualization/TestInstantiationPositive.jpg.
        ///     Then expect no violation of the order rules.
        /// </summary>
        [Test]
        public void TestInstantiationOrderPositive() {
            // build a relatively complex graph
            ModelStructure structure = new ModelStructure();

            TLayerDescription[] descriptions = {
                new TLayerDescription("0", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("1", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("2", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("3", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("4", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("5", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("6", 0, 1, "no use.dll","0","0"),
                new TLayerDescription("7", 0, 1, "no use.dll","0","0")
            };

            structure.AddLayer(descriptions[0], typeof (Zero));
            structure.AddLayer(descriptions[1], typeof (One));
            structure.AddLayer(descriptions[2], typeof (Two));
            structure.AddLayer(descriptions[3], typeof (Three), typeof (Zero), typeof (One), typeof (Two));
            structure.AddLayer(descriptions[4], typeof (Four), typeof (Three));
            structure.AddLayer(descriptions[5], typeof (Five), typeof (Three), typeof (Six));
            structure.AddLayer(descriptions[6], typeof (Six), typeof (Four));
            structure.AddLayer(descriptions[7], typeof (Seven), typeof (Six));

            // now test against violations

            IList<TLayerDescription> order = structure.CalculateInstantiationOrder();

            // check 0, 1 and 2
            Assert.IsTrue(order.IndexOf(descriptions[0]) < order.IndexOf(descriptions[3]),
                "description[0] too late in the order.");
            Assert.IsTrue(order.IndexOf(descriptions[1]) < order.IndexOf(descriptions[3]),
                "description[1] too late in the order.");
            Assert.IsTrue(order.IndexOf(descriptions[2]) < order.IndexOf(descriptions[3]),
                "description[2] too late in the order.");

            //check 3
            Assert.IsTrue(order.IndexOf(descriptions[3]) < order.IndexOf(descriptions[4]),
                "description[3] too late in the order.");
            Assert.IsTrue(order.IndexOf(descriptions[3]) < order.IndexOf(descriptions[5]),
                "description[3] too late in the order.");

            // check 4
            Assert.IsTrue(order.IndexOf(descriptions[4]) > order.IndexOf(descriptions[3]),
                "description[4] too early in the order.");
            Assert.IsTrue(order.IndexOf(descriptions[4]) < order.IndexOf(descriptions[6]),
                "description[4] too early in the order.");

            //check 5
            Assert.IsTrue(order.IndexOf(descriptions[5]) > order.IndexOf(descriptions[3])
                          && order.IndexOf(descriptions[5]) > order.IndexOf(descriptions[6]),
                "description[5] too early in the order.");

            //check 6
            Assert.IsTrue(order.IndexOf(descriptions[6]) > order.IndexOf(descriptions[4]),
                "description[6] too early in the order.");

            //check 7
            Assert.IsTrue(order.IndexOf(descriptions[7]) > order.IndexOf(descriptions[6]),
                "description[7] too early in the order.");
        }
    }
}


