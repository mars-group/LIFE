using System;
using System.Diagnostics;
using CommonTypes.DataTypes;
using ESCTestLayer.Entities;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using NUnit.Framework;

namespace ESCTest {

  public class ESCTest {



    private IESC _esc;


    #region Setup / Tear down
    
    [SetUp]
    public void SetUp() {
      _esc = new ESC();
    }

    
    [TearDown]
    public void TearDown() {
    }

    #endregion


    [Test]
    public void TestCorrectPlacement2D() {

      Vector dims = new Vector(1, 1, 0);
      for (int i = 0; i < 4; i ++) _esc.Add(i, dims);

      Vector pos, ret;

      pos = new Vector(1, 1, 0);
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
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      var pos= new Vector(1, 1, 0);

      Assert.True(pos.Equals(_esc.SetPosition(0, pos, Vector.UnitVectorXAxis).Position));
      Assert.False(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
    }



    [Test]
    public void TestFitting3D_floats() {
      var dims = new Vector(1, 1, 1);
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      var pos = new Vector(1, 1, 1);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos,  Vector.UnitVectorXAxis).Position));

      pos = new Vector(1, 1, 0);
      Assert.True(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
    }


    [Test]
    public void TestOverlap3D_floats()
    {
        var dims = new Vector(1, 1, 1);
        _esc.Add(0, dims);
        _esc.Add(1, dims);

        Vector pos, ret;

        pos = new Vector(1, 1, 1);
        Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector(0, 1, 0)).Position));

        pos = new Vector(1, 1, 0);
        Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector(1, 1, 1)).Position));

        pos = new Vector(1, 0, 1);
        Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector(1, 0, 1)).Position));
    }

    [Test]
    public void TestOverlap2DTo3D()
    {
        var dimension1 = new Vector(10, 10, 10);
        var dimension2 = new Vector(1, 1, 0);
        _esc.Add(0, dimension1);
        _esc.Add(1, dimension2);

        Vector pos = Vector.Origin;
        Assert.True(pos.Equals(_esc.SetPosition(0, pos, Vector.UnitVectorXAxis).Position));

        pos = new Vector(5, 5, 5);
        Assert.True(Vector.Null.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
     
        pos = new Vector(11,11,11);
        Assert.True(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));

        pos = new Vector(5, 5, 5);
        Assert.False(pos.Equals(_esc.SetPosition(1, pos, Vector.UnitVectorXAxis).Position));
    }



    [Test]
    public void TestRegainOfOldPosition() {

      Vector dims = new Vector(1, 1, 1);
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      Vector pos, ret;

      pos = new Vector(1, 1, 1);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector(0, 1, 0)).Position));

      pos = new Vector(1, 1, 0);  
      Assert.True(pos.Equals(_esc.SetPosition(1, pos, new Vector(1, -1, 0)).Position));

      Assert.True(pos.Equals(_esc.SetPosition(1, new Vector(1, 1, 1), new Vector(1, 0, 0)).Position));
    }


    [Test]
    public void Test500() {

      Vector dims = new Vector(1, 1, 1);
      Vector dir  = new Vector(1, 0, 0);

      var stopwatch = Stopwatch.StartNew();
      for (int i = 0; i < 500; i++) {
        _esc.Add(i, dims);
        _esc.SetPosition(i, new Vector(i, 0, 0), dir);
      }
      // 36.4 sec für 50k agents.
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

    }
  }
}