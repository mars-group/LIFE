using System;
using System.Diagnostics;
using CommonTypes.DataTypes;
using ESCTest.Entities;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using NUnit.Framework;

namespace ESCTest.Tests {
    public class ESCTest {
        private IESC _esc;
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
            var agent0 = new TestAgent2D(0, _esc);
            var agent1 = new TestAgent2D(1, _esc);

            Assert.True(agent0.SetPosition(new Vector(1, 1)));
            Assert.False(agent1.SetPosition(new Vector(1, 1)));
            Assert.True(agent1.SetPosition(new Vector(2.1f, 2.1f)));
            Assert.False(agent1.SetPosition(new Vector(1, 1)));
        }

        [Test]
        public void TestCorrectPlacement2D() {
            var dims = new Vector(1, 1, 0);
            for (int i = 0; i < 4; i ++) {
                _esc.Add(i, InformationType, true, dims);
            }

            var pos = new Vector(1, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector(0, 1, 0)).Position));

            pos = new Vector(2, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, new Vector(-1, 0, 0)).Position));

            pos = new Vector(2, 0, 0);
            Assert.True(pos.Equals(_esc.SetPosition(2, pos, new Vector(0, -1, 0)).Position));

            pos = new Vector(0, 2, 0);
            Assert.True(pos.Equals(_esc.SetPosition(3, pos, new Vector(1, 0, 0)).Position));
        }


        [Test]
        public void TestOverlap2D() {
            var dims = new Vector(1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new Vector(1, 1, 0);

            Assert.True(pos.Equals(_esc.SetPosition(0, pos, Vector.UnitVectorXAxis).Position));
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
        }


        [Test]
        public void TestFitting3D_floats() {
            var dims = new Vector(1, 1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new Vector(1, 1, 1);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, Vector.UnitVectorXAxis).Position));

            pos = new Vector(1, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
        }


        [Test]
        public void TestOverlap3D_floats() {
            var dims = new Vector(1, 1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new Vector(1, 1, 1);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector(0, 1, 0)).Position));

            pos = new Vector(1, 1, 0);
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector(1, 1, 1)).Position));

            pos = new Vector(1, 0, 1);
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector(1, 0, 1)).Position));
        }

        [Test]
        public void TestOverlap2DTo3D() {
            var dimension1 = new Vector(10, 10, 10);
            var dimension2 = new Vector(1, 1, 0);
            _esc.Add(0, InformationType, true, dimension1);
            _esc.Add(1, InformationType, true, dimension2);

            Vector pos = Vector.Origin;
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, Vector.UnitVectorXAxis).Position));

            pos = new Vector(5, 5, 5);
            Assert.True(Vector.Null.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));

            pos = new Vector(11, 11, 11);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));

            pos = new Vector(5, 5, 5);
            Assert.False(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
        }


        [Test]
        public void TestRegainOfOldPosition() {
            var dims = new Vector(1, 1, 1);
            _esc.Add(0, InformationType, true, dims);
            _esc.Add(1, InformationType, true, dims);

            var pos = new Vector(1, 1, 1);
            Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector(0, 1, 0)).Position));

            pos = new Vector(1, 1, 0);
            Assert.True(pos.Equals(_esc.SetPosition(1, pos, new Vector(1, -1, 0)).Position));

            Assert.True(pos.Equals(_esc.SetPosition(1, new Vector(1, 1, 1), new Vector(1, 0, 0)).Position));
        }


        [Test]
        public void TestAdd500Elements() {
            var dimension = new Vector(1, 1, 1);

            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 500; i++) {
                _esc.Add(i, InformationType, true, dimension);
                _esc.SetPosition(i, new Vector(i, 0, 0), Vector.UnitVectorXAxis);
            }
            // 36.4 sec für 50k agents.
            Console.WriteLine(stopwatch.ElapsedMilliseconds+ " ms");
        }
    }
}