using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using NUnit.Framework;


namespace GoapCustomGraphTests
{
  
    [TestFixture]
    public class VertexTest {

        private Vertex _v1;
        private Vertex _v2;
        private Vertex _v3;
        private Vertex _v4;
        

        [SetUp]
        protected void SetUp() {
            _v1 = new Vertex(new List<IGoapWorldstate>(), 2, "V1");
            _v2 = new Vertex(new List<IGoapWorldstate>(), 2, "V2");
            _v3 = new Vertex(new List<IGoapWorldstate>());
            _v4 = new Vertex(new List<IGoapWorldstate>());
            // TODO factory zum initialisieren von nicht leeren worldstatelisten

        }


        [Test]
        public void ConstructorThreeParamTest() {
            Assert.NotNull(_v1);
        }

        [Test]
        public void ConstructorOneParamTest()
        {
            Assert.NotNull(_v3);
        }
     
        /// <summary>
        /// Equality is depending on type of wordstates and their bool value
        /// </summary>
        [Test]
        public void EqualityTest() {
            Assert.True(_v1.Equals(_v2));
            Assert.True(_v3.Equals(_v4));
            Assert.True(_v1.Equals(_v4));
        }


        [Test]
        public void EqualityOperatorTest() {
            Assert.True(_v1 == _v2);
            Assert.True(_v3 == _v4);
            Assert.True(_v1 == _v4);
           
        }


    }
}

