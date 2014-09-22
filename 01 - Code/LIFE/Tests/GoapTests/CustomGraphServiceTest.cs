using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests
{
    [TestFixture]
    class CustomGraphServiceTest
    {

         #region Setup/Teardown

        private static readonly IGoapWorldstate HappyTrue = new Happy(true);
        private static readonly IGoapWorldstate HappyFalse1 = new Happy(false);
        private static readonly IGoapWorldstate HappyFalse2 = new Happy(false);
        private static readonly IGoapWorldstate ToyTrue = new HasToy(true);
        private static readonly IGoapWorldstate ToyFalse = new HasToy(false);

        private Graph _graph1;
        private Graph _graph2;

        private AStarSteppable _star1;
        private AStarSteppable _star2;

        private IGoapVertex _root1;
        private IGoapVertex _root2;

        private IGoapVertex _target1;
        private IGoapVertex _target2;

        private Vertex _v1;
        private Vertex _v2;
        private Vertex _v3;
        private Vertex _v4;
        private Vertex _v5;

        [SetUp]
        protected void SetUp() {

            _v1 = new Vertex(new List<IGoapWorldstate>{HappyTrue},1,"v1_happy_true");
            _v2 = new Vertex(new List<IGoapWorldstate>{HappyFalse1},1,"v2_happy_false");
            _v3 = new Vertex(new List<IGoapWorldstate>{HappyFalse2},1,"v3_happy_false");
            _v4 = new Vertex(new List<IGoapWorldstate>{ToyTrue},1,"v4_toy_true");
            _v5 = new Vertex(new List<IGoapWorldstate>{ToyFalse}, 1, "v5_toy_false");

            _root1 = _v1;
            _target1 = _v5;

            _graph1 = new Graph(new List<IGoapVertex> { _root1 }, new List<IGoapEdge>());
            _star1 = new AStarSteppable(_root1, _target1, _graph1);

        }

        #endregion

        [Test]
        public void ParallelEdgesTest() {
            var e1 = new Edge(1, _root1, _v2);
            var e2 = new Edge(1, _root1, _v2);

            _graph1.AddVertex(_v2);
            _graph1.AddEdge(e1);
            _graph1.AddVertex(_v3);
            _graph1.AddEdge(e2);
            
            var edges = _graph1.GetEdges();
            var nodes = _graph1.GetVertices();
            Assert.True(edges.Count == 2);
            Assert.True(nodes.Count == 2);

            Assert.AreNotSame(edges[0],edges[1]);
            Assert.True(edges[0].GetSource().Equals(edges[1].GetSource()));
            Assert.True(edges[0].GetTarget().Equals(edges[1].GetTarget()));
        }


        [Test]
        public void TestForAbilityMultigraph() {
            var e1 = new Edge(1, _root1, _v2);
            var e2 = new Edge(1, _root1, _v3);

            _graph1.AddVertex(_v2);
            _graph1.AddEdge(e1);
            
            _graph1.AddVertex(_v3);
            _graph1.AddEdge(e2);

            // check graph is not empty
            Assert.False(_graph1.IsEmpty());
            // check there are both edges in the graph
            Assert.True(_graph1.GetEdges().Count == 2);
            // check the target vertex of both edges is identified as equals
            Assert.True(_graph1.GetVertices().Count == 2);
            // double check the are no three vertices saved in graph
            Assert.False(_graph1.GetVertices().Count == 3); 
        }

         
    }
}
