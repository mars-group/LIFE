using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCConnector.TransportTypes.ModelStructure;
using NUnit.Framework;

namespace SimulationManagerTest
{
    [TestFixture]
    public class TestModelContainer
    {
        [Test]
        public void TestModelContentCopy() {
            if(Directory.Exists("./copiedOrdner")) Directory.Delete("./copiedOrdner");

            ModelContent content = new ModelContent("./testOrdner");

            content.Write("./copiedOrdner");
        }
    }
}
