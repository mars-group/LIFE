using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using GoapModelTest.Worldstates;
using NUnit.Framework;


namespace GoapTests {
    [TestFixture]
    internal class CustomGraphGraphTest {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp() {
            _graph = new Graph(new List<IGoapNode> {V5}, new List<IGoapEdge>());
            _emptyGraph = new Graph(new List<IGoapNode>(), new List<IGoapEdge>());
        }

        #endregion

        private static readonly IGoapWorldstate HappyTrue = new Happy(true);
        private static readonly IGoapWorldstate HappyFalse1 = new Happy(false);
        private static readonly IGoapWorldstate HappyFalse2 = new Happy(false);
        private static readonly IGoapWorldstate ToyTrue = new HasToy(true);
        private static readonly IGoapWorldstate ToyFalse = new HasToy(false);

        private Graph _graph;
        private Graph _emptyGraph;

        private static readonly Vertex V1 = new Vertex(new List<IGoapWorldstate> { HappyTrue }, 1, "v1_happy_true");
        private static readonly Vertex V2 = new Vertex(new List<IGoapWorldstate> { HappyFalse1 }, 1, "v2_happy_false");
        private static readonly Vertex V3 = new Vertex(new List<IGoapWorldstate> { HappyFalse2 }, 1, "v3_happy_false");
        private static readonly Vertex V4 = new Vertex(new List<IGoapWorldstate> { ToyTrue }, 1, "v4_toy_true");
        private static readonly Vertex V5 = new Vertex(new List<IGoapWorldstate> { ToyFalse }, 1, "v5_toy_false");

        private static readonly Vertex V6 = new Vertex(new List<IGoapWorldstate> { ToyFalse }, 1, "v5_toy_false");

        [Test]
        public void AddVertexTest() {
            Assert.False(_graph.GetVertices().Count == 0);
            int startCount = _graph.GetVertices().Count;
            _graph.AddVertex(V4);
            Assert.True(_graph.GetVertices().Count == (startCount + 1));
            _graph.AddVertex(V3);
            Assert.True(_graph.GetVertices().Count == (startCount + 2));
            _graph.AddVertex(V1);
            Assert.True(_graph.GetVertices().Count == (startCount + 3));
            Assert.False(_graph.GetVertices().Count == 0);
        }

        [Test]
        public void AddEdgeTest() {
            _graph.AddVertex(V4);
            _graph.AddVertex(V2);
            _graph.AddVertex(V1);

            int startCount = _graph.GetEdges().Count;
            Assert.False(startCount > 0);
            _graph.AddEdge(new Edge(1, V5, V2));
            Assert.True(_graph.GetEdges().Count == (startCount + 1));
            _graph.AddEdge(new Edge(1, V4, V1));
            Assert.True(_graph.GetEdges().Count == (startCount + 2));
            _graph.AddEdge(new Edge(1, V1, V5));
            Assert.True(_graph.GetEdges().Count == (startCount + 3));
        }

        [Test]
        public void IsEmptyTest() {
            Assert.True(_emptyGraph.IsEmpty());
            Assert.False(_graph.IsEmpty());
        }

        [Test]
        public void ContainsVertex() {
            Assert.True(_graph.ContainsVertex(V5));
            Assert.True(_graph.ContainsVertex(V6));
            Assert.False(_graph.ContainsVertex(V1));
        }

        [Test]
        public void ConsistenceTest() {
            _emptyGraph.AddEdge(new Edge(1, V5, V2));
            _emptyGraph.AddEdge(new Edge(1, V4, V1));
            Assert.True(_emptyGraph.ContainsVertex(V1));
            Assert.True(_emptyGraph.ContainsVertex(V2));
            Assert.True(_emptyGraph.ContainsVertex(V4));
            Assert.True(_emptyGraph.ContainsVertex(V5));
            Assert.True(_emptyGraph.GetEdges().Count == 2);
            Assert.True(_emptyGraph.GetVertices().Count == 4);
        }

        [Test]
        public void GetEdgesBySourceAndTargetTest() {
            IGoapEdge e1 = new Edge(1, V5, V2);
            IGoapEdge e2 = new Edge(1, V4, V1);
            IGoapEdge e3 = new Edge(1, V1, V4);
            IGoapEdge e4 = new Edge(2, V1, V4);
            IGoapEdge e5 = new Edge(1, V1, V2);

            _emptyGraph.AddEdge(e1);
            _emptyGraph.AddEdge(e2);
            _emptyGraph.AddEdge(e3);
            _emptyGraph.AddEdge(e4);
            _emptyGraph.AddEdge(e5);

            List<IGoapEdge> edgeList = _emptyGraph.GetEdgesBySourceAndTarget(V1, V4);
            Assert.IsNotEmpty(edgeList);
            Assert.True(edgeList.Count == 2);
            Assert.Contains(e3, edgeList);
            Assert.Contains(e4, edgeList);

            edgeList = _emptyGraph.GetEdgesBySourceAndTarget(V5, V4);
            Assert.IsEmpty(edgeList);


            Assert.AreEqual(e3, _emptyGraph.GetCheapestEdgeBySourceAndTarget(V1, V4));
        }

        [Test]
        public void GetcheapestWayCostTest() {
            IGoapEdge e3 = new Edge(1, V1, V4);
            IGoapEdge e4 = new Edge(2, V1, V4);

            _emptyGraph.AddEdge(e3);
            _emptyGraph.AddEdge(e4);

            Assert.True(_emptyGraph.GetcheapestWayCost(V1, V4) == 1);
        }

        [Test]
        public void GetReachableAdjcentVerticesTest() {
            IGoapEdge e2 = new Edge(1, V2, V1);
            IGoapEdge e3 = new Edge(1, V1, V4);
            IGoapEdge e4 = new Edge(2, V1, V5);

            _emptyGraph.AddEdge(e2);
            _emptyGraph.AddEdge(e3);
            _emptyGraph.AddEdge(e4);

            List<IGoapNode> vertices = _emptyGraph.GetReachableAdjcentVertices(V1);
            Assert.Contains(V4, vertices);
            Assert.Contains(V5, vertices);
            Assert.False(vertices.Contains(V2));
            Assert.False(vertices.Contains(null));
        }

        [Test]
        public void GetEdgesBySourceVertexTest() {
            IGoapEdge e2 = new Edge(1, V2, V1);
            IGoapEdge e3 = new Edge(1, V1, V4);
            IGoapEdge e4 = new Edge(2, V1, V5);

            _emptyGraph.AddEdge(e2);
            _emptyGraph.AddEdge(e3);
            _emptyGraph.AddEdge(e4);

            List<IGoapEdge> edges = _emptyGraph.GetEdgesBySourceVertex(V1);

            Assert.Contains(e3, edges);
            Assert.Contains(e4, edges);
            Assert.False(edges.Contains(e2));
            Assert.False(edges.Contains(null));
        }
    }
}