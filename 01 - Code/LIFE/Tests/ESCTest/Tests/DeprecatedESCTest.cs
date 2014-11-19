using System;
using System.Diagnostics;
using ESCTest.Entities;
using ESCTestLayer.Implementation;
using NUnit.Framework;

namespace ESCTest.Tests {
    using ESCTestLayer.Interface;
    using GenericAgentArchitectureCommon.TransportTypes;

    public class DeprecatedESCTest {
        private IDeprecatedESC _esc;
        const int InformationType = 1;

        #region Setup / Tear down

        [SetUp]
        public void SetUp() {
            _esc = new ESC();
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        [Test]
        public void TestAgent2D() {
            var agent0 = new TestDeprecatedAgent2D(0, _esc);
            var agent1 = new TestDeprecatedAgent2D(1, _esc);

            Assert.True(agent0.SetPosition(new TVector(1, 1)));
            Assert.False(agent1.SetPosition(new TVector(1, 1)));
            Assert.True(agent1.SetPosition(new TVector(2.1f, 2.1f)));
            Assert.False(agent1.SetPosition(new TVector(1, 1)));
        }

        [Test]
        public void TestCorrectPlacement2D() {
            var dims = new TVector(1, 1, 0);
            for (int i = 0; i < 4; i ++) {
                _esc.Add(i, InformationType, true, dims);
            }

            var pos = new TVector(1, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, new TVector(0, 1, 0)).Position));

            pos = new TVector(2, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, new TVector(-1, 0, 0)).Position));

            pos = new TVector(2, 0, 0);
            Assert.True(pos.Equals(_esc.SetPosition(2, pos, new TVector(0, -1, 0)).Position));

            pos = new TVector(0, 2, 0);
            Assert.True(pos.Equals(_esc.SetPosition(3, pos, new TVector(1, 0, 0)).Position));
        }


        [Test]
        public void TestOverlap2D() {
            var dims = new TVector(1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new TVector(1, 1, 0);

            Assert.True(pos.Equals(_esc.SetPosition(0, pos, TVector.UnitVectorXAxis).Position));
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, TVector.UnitVectorXAxis).Position));
        }


        [Test]
        public void TestFitting3D_floats() {
            var dims = new TVector(1, 1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new TVector(1, 1, 1);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, TVector.UnitVectorXAxis).Position));

            pos = new TVector(1, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, TVector.UnitVectorXAxis).Position));
        }


        [Test]
        public void TestOverlap3D_floats() {
            var dims = new TVector(1, 1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new TVector(1, 1, 1);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, new TVector(0, 1, 0)).Position));

            pos = new TVector(1, 1, 0);
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, new TVector(1, 1, 1)).Position));

            pos = new TVector(1, 0, 1);
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, new TVector(1, 0, 1)).Position));
        }

        [Test]
        public void TestOverlap2DTo3D() {
            var dimension1 = new TVector(10, 10, 10);
            var dimension2 = new TVector(1, 1, 0);
            _esc.Add(0, InformationType, true, dimension1);
            _esc.Add(1, InformationType, true, dimension2);

            TVector pos = TVector.Origin;
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, TVector.UnitVectorXAxis).Position));

            pos = new TVector(5, 5, 5);
            Assert.True(TVector.Null.Equals(_esc.SetPosition(1, pos, TVector.UnitVectorXAxis).Position));

            pos = new TVector(11, 11, 11);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, TVector.UnitVectorXAxis).Position));

            pos = new TVector(5, 5, 5);
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, TVector.UnitVectorXAxis).Position));
        }


        [Test]
        public void TestRegainOfOldPosition() {
            var dims = new TVector(1, 1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new TVector(1, 1, 1);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, new TVector(0, 1, 0)).Position));

            pos = new TVector(1, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, new TVector(1, -1, 0)).Position));

            Assert.True(pos.Equals(_esc.SetPosition(1, new TVector(1, 1, 1), new TVector(1, 0, 0)).Position));
        }


        [Test]
        public void TestAdd500Elements() {
            var dimension = new TVector(1, 1, 1);

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++) {
                _esc.Add(i, InformationType, true, dimension);
                _esc.SetPosition(i, new TVector(i, 0, 0), TVector.UnitVectorXAxis);
            }
            // 36.4 sec für 50k agents.
            Console.WriteLine(stopwatch.ElapsedMilliseconds+ " ms");
        }
    }
}