namespace SimulationManagerTest {
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using LCConnector.TransportTypes;
    using LCConnector.TransportTypes.ModelStructure;

    using ModelContainer.Implementation.Entities;

    using NUnit.Framework;

    using SimulationManagerTest.ModelContainerTestClasses;

    [TestFixture]
    public class ModelContainerTest {
        [Test]
        public void TestModelContentCopy() {
            if (Directory.Exists("./copiedOrdner")) Directory.Delete("./copiedOrdner", true);
            if (!Directory.Exists("./testOrdner")) Directory.CreateDirectory("./testOrdner");

            ModelContent content = new ModelContent("./testOrdner");

            BinaryFormatter formatter = new BinaryFormatter();

            using (var stream = new MemoryStream()) {
                formatter.Serialize(stream, content);
                stream.Position = 0;
                ModelContent deserialized = (ModelContent) formatter.Deserialize(stream);
                deserialized.Write("./copiedOrdner");
            }
            //TODO assertion is not active, who ever did this fix it!
            //Assert.AreEqual(Directory.GetFileSystemEntries("./testOrdner"), Directory.GetFileSystemEntries("./copiedOrdner"));
        }

        /// <summary>
        ///     Construct a graph as pictured in Tests/Visualization/TestInstantiationPositive.jpg.
        ///     Then expect no violation of the order rules.
        /// </summary>
        [Test]
        public void TestInstantiationOrderPositive() {
            // build a relatively complex graph
            ModelStructure structure = new ModelStructure();

            TLayerDescription[] descriptions = {
                new TLayerDescription("0", 0, 1, "no use.dll"),
                new TLayerDescription("1", 0, 1, "no use.dll"),
                new TLayerDescription("2", 0, 1, "no use.dll"),
                new TLayerDescription("3", 0, 1, "no use.dll"),
                new TLayerDescription("4", 0, 1, "no use.dll"),
                new TLayerDescription("5", 0, 1, "no use.dll"),
                new TLayerDescription("6", 0, 1, "no use.dll"),
                new TLayerDescription("7", 0, 1, "no use.dll")
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

        [Test]
        public void ModelListChangeTest()
        {
            
        }


        [Test]
        public void LoadModelFromUrlTest()
        {
            //TODO write method

        }

    }
}