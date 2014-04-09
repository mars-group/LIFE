using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using LCConnector.TransportTypes.ModelStructure;
using NUnit.Framework;

namespace SimulationManagerTest
{
    [TestFixture]
    public class TestModelContainer {
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

            //Assert.AreEqual(Directory.GetFileSystemEntries("./testOrdner"), Directory.GetFileSystemEntries("./copiedOrdner"));
        }
    }
}
