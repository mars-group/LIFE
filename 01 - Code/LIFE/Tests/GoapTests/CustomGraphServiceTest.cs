using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using GoapModelTest.Actions;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    internal class CustomGraphServiceTest {

        #region Setup/Teardown

        [SetUp]
        protected void SetUp() {
            _graph = new Graph(new List<IGoapVertex> {V5}, new List<IGoapEdge>());
            _graphService = new GoapCustomGraphService();
         }

        #endregion

        private static readonly IGoapWorldstate HappyTrue = new Happy(true);
        private static readonly IGoapWorldstate HappyFalse1 = new Happy(false);
        private static readonly IGoapWorldstate HappyFalse2 = new Happy(false);
        private static readonly IGoapWorldstate ToyTrue = new HasToy(true);
        private static readonly IGoapWorldstate ToyFalse = new HasToy(false);

        private Graph _graph;
        private IGoapGraphService _graphService;

        private static readonly Vertex V1 = new Vertex(new List<IGoapWorldstate> {HappyTrue}, 1, "v1_happy_true");
        private static readonly Vertex V2 = new Vertex(new List<IGoapWorldstate> {HappyFalse1}, 1, "v2_happy_false");
        private static readonly Vertex V3 = new Vertex(new List<IGoapWorldstate> {HappyFalse2}, 1, "v3_happy_false");
        private static readonly Vertex V4 = new Vertex(new List<IGoapWorldstate> {ToyTrue}, 1, "v4_toy_true");
        private static readonly Vertex V5 = new Vertex(new List<IGoapWorldstate> {ToyFalse}, 1, "v5_toy_false");

     
       [Test]
        public void ParallelEdgesTest() {
            Edge e1 = new Edge(1, V5, V2);
            Edge e2 = new Edge(1, V5, V2);

            _graph.AddEdge(e1);
            _graph.AddEdge(e2);

            List<IGoapEdge> edges = _graph.GetEdges();
            
            Assert.AreNotSame(edges[0], edges[1]);
            Assert.True(edges[0].GetSource().Equals(edges[1].GetSource()));
            Assert.True(edges[0].GetTarget().Equals(edges[1].GetTarget()));
        }

        [Test]
        public void AntiparallelEdgesTest() {
            Edge e1 = new Edge(1, V5, V2);
            Edge e2 = new Edge(1, V2, V5);

            _graph.AddEdge(e1);
            _graph.AddEdge(e2);

            List<IGoapEdge> edges = _graph.GetEdges();
            
            Assert.AreNotSame(edges[0], edges[1]);
            Assert.True(edges[0].GetSource().Equals(edges[1].GetTarget()));
            Assert.True(edges[0].GetTarget().Equals(edges[1].GetSource()));
        }
        
        [Test]
        public void RealizeVertexEqualityTest() {
            Edge e1 = new Edge(1, V5, V2);
            Edge e2 = new Edge(1, V5, V3);

            _graph.AddEdge(e1);
            _graph.AddEdge(e2);

            List<IGoapVertex> nodes = _graph.GetVertices();
            Assert.True(V2.Equals(V3));
            Assert.True(nodes.Count == 2);
        }

        [Test]
        public void IsGraphEmptyTest() {
            _graphService.InitializeGoapGraph(new List<IGoapWorldstate>(), new List<IGoapWorldstate>() );
            Assert.False(_graphService.IsGraphEmpty());
        }

        [Test]
        public void GetNextVertexFromOpenListTest() {
            _graphService.InitializeGoapGraph(new List<IGoapWorldstate>(), new List<IGoapWorldstate>());
            Assert.AreEqual(new Vertex(new List<IGoapWorldstate>()), _graphService.GetNextVertexFromOpenList());

            _graphService.InitializeGoapGraph(new List<IGoapWorldstate>{HappyFalse1}, new List<IGoapWorldstate>());
            Assert.AreEqual(new Vertex(new List<IGoapWorldstate> { HappyFalse1 }), _graphService.GetNextVertexFromOpenList());
        }

        [Test]
        public void HasNextVertexOnOpenListTest() {
            _graphService.InitializeGoapGraph(new List<IGoapWorldstate>(), new List<IGoapWorldstate>());
            Assert.True(_graphService.HasNextVertexOnOpenList());
        }

        [Test]
        public void IsCurrentVertexTargetTest() {
            _graphService.InitializeGoapGraph(new List<IGoapWorldstate>(), new List<IGoapWorldstate>());
            Assert.True(_graphService.IsCurrentVertexTarget());

            _graphService.InitializeGoapGraph(new List<IGoapWorldstate> { HappyFalse1 }, new List<IGoapWorldstate> { HappyFalse1 });
            Assert.True(_graphService.IsCurrentVertexTarget());

            _graphService.InitializeGoapGraph(new List<IGoapWorldstate> { HappyFalse2 }, new List<IGoapWorldstate> { HappyFalse1 });
            Assert.True(_graphService.IsCurrentVertexTarget());

            _graphService.InitializeGoapGraph(new List<IGoapWorldstate> { HappyFalse2, ToyTrue }, new List<IGoapWorldstate> { HappyFalse2 });
            Assert.True(_graphService.IsCurrentVertexTarget());
        }
        /*
        [Test]
        public void GetShortestPathTest() {
            // V5 is root; V1 is target;
            _graphService.InitializeGoapGraph(new List<IGoapWorldstate> {HappyTrue}, new List<IGoapWorldstate> {ToyFalse});
            IGoapVertex curr = _graphService.GetNextVertexFromOpenList();


            throw new NotImplementedException();
        }

        [Test]
        public void GetActualDepthFromRootTest(){
            throw new NotImplementedException();
        }*/

        [Test]
        public void GetEdgeFromAbstractGoapActionTest(){
            /*
            IGoapEdge e1 = _graphService.GetEdgeFromAbstractGoapAction(new ActionPlay(), V1.Worldstate());
            Assert.AreEqual(e1.GetSource(),V1);
            Assert.AreNotSame(e1.GetSource(),V1);

            IGoapEdge e2 = _graphService.GetEdgeFromAbstractGoapAction(new ActionClean(), V2.Worldstate());
            Assert.AreEqual(e2.GetSource(), V2);
            Assert.AreNotSame(e2.GetSource(), V2);

            IGoapEdge e3 = _graphService.GetEdgeFromAbstractGoapAction(new ActionGetToy(), V3.Worldstate());
            Assert.AreEqual(e3.GetSource(), V3);
            Assert.AreNotSame(e3.GetSource(), V3);
            */
        } 
        
        [Test]
        public void GetEdgeFromPreconditionsTest() {
           /*
            AbstractGoapAction a1 = new ActionPlay();
            AbstractGoapAction a2 = new ActionGetToy();
            AbstractGoapAction a3 = new ActionClean();

            IGoapEdge e1 = GetEdgeFromPreconditions(a1,);

            IGoapEdge GetEdgeFromPreconditions(AbstractGoapAction action, List<IGoapWorldstate> currentState) {
            var start = new Vertex(currentState);
            var target = new Vertex(action.PreConditions);
            return new Edge(action.GetExecutionCosts(), start, target);
            */
        }
        
        
    }
}