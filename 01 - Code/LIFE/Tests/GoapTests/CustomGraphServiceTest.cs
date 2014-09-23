﻿using System;
using System.Collections.Generic;
using GoapCommon.Interfaces;
using GoapGraphConnector.CustomGraph;
using GoapModelTest.Worldstates;
using NUnit.Framework;

namespace GoapTests {
    [TestFixture]
    internal class CustomGraphServiceTest {

        #region Setup/Teardown

        [SetUp]
        protected void SetUp() {
            _graph = new Graph(new List<IGoapVertex> {V5}, new List<IGoapEdge>());
            _star = new AStarSteppable(V5, V1, _graph);
            _graphService = new GoapCustomGraphService();
         }

        #endregion

        private static readonly IGoapWorldstate HappyTrue = new Happy(true);
        private static readonly IGoapWorldstate HappyFalse1 = new Happy(false);
        private static readonly IGoapWorldstate HappyFalse2 = new Happy(false);
        private static readonly IGoapWorldstate ToyTrue = new HasToy(true);
        private static readonly IGoapWorldstate ToyFalse = new HasToy(false);

        private Graph _graph;
        private AStarSteppable _star;
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

        [Test]
        public void GetShortestPathTest() {
            throw new NotImplementedException();
        }

        [Test]
        public void GetActualDepthFromRootTest(){
            throw new NotImplementedException();
        }

        [Test]
        public void GetEdgeFromAbstractGoapActionTest(){
            throw new NotImplementedException();
        } 
        
        [Test]
        public void GetEdgeFromPreconditionsTest(){
            throw new NotImplementedException();
        }
        
        [Test]
        public void ImplicitExpandCurrentVertexTest(){
            throw new NotImplementedException();
        }

        [Test]
        public void ImplicitAStarStepTest(){
            throw new NotImplementedException();
        }
        
    }
}