using System;
using System.Diagnostics;
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

      Vector3f dims = new Vector3f(1, 1, 0);
      for (int i = 0; i < 4; i ++) _esc.Add(i, dims);

      Vector3f pos, ret;

      pos = new Vector3f(1, 1, 0);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector3f(0, 1, 0))));

      pos = new Vector3f(2, 1, 0);
      Assert.True(pos.Equals(_esc.SetPosition(1, pos, new Vector3f(-1, 0, 0))));

      pos = new Vector3f(2, 0, 0);
      Assert.True(pos.Equals(_esc.SetPosition(2, pos, new Vector3f(0, -1, 0))));

      pos = new Vector3f(0, 2, 0);
      Assert.True(pos.Equals(_esc.SetPosition(3, pos, new Vector3f(1, 0, 0))));
    }


    [Test]
    public void TestOverlap2D() {

      Vector3f dims = new Vector3f(1, 1, 0);
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      Vector3f pos;

      pos = new Vector3f(1, 1, 0);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector3f(0, 1, 0))));

      pos = new Vector3f(1, 1, 0);
      Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector3f(-1, 0, 0))));
    }



    [Test]
    public void TestApposite3D_floats() {

      Vector3f dims = new Vector3f(1, 1, 1);
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      Vector3f pos, ret;

      pos = new Vector3f(1, 1, 1);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector3f(0, 1, 0))));

      pos = new Vector3f(1, 1, 0);  
       Assert.True(pos.Equals(_esc.SetPosition(1, pos, new Vector3f(-1, 1, 0))));
    }



    [Test]
    public void TestOverlap3D_floats() {

      Vector3f dims = new Vector3f(1, 1, 1);
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      Vector3f pos, ret;

      pos = new Vector3f(1, 1, 1);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector3f(0, 1, 0))));

      pos = new Vector3f(1, 1, 0);  
      Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector3f(1, 1, 1))));

      pos = new Vector3f(1, 0, 1);  
      Assert.False(pos.Equals(_esc.SetPosition(1, pos, new Vector3f(1, 0, 1))));
    }



    [Test]
    public void TestRegainOfOldPosition() {

      Vector3f dims = new Vector3f(1, 1, 1);
     _esc.Add(0, dims);
     _esc.Add(1, dims);

      Vector3f pos, ret;

      pos = new Vector3f(1, 1, 1);
      Assert.True(pos.Equals(_esc.SetPosition(0, pos, new Vector3f(0, 1, 0))));

      pos = new Vector3f(1, 1, 0);  
      Assert.True(pos.Equals(_esc.SetPosition(1, pos, new Vector3f(1, -1, 0))));

      Assert.True(pos.Equals(_esc.SetPosition(1, new Vector3f(1, 1, 1), new Vector3f(1, 0, 0))));
    }


    [Test]
    public void Test50000() {

      Vector3f dims = new Vector3f(1, 1, 1);
      Vector3f dir  = new Vector3f(1, 0, 0);

      var stopwatch = Stopwatch.StartNew();
      for (int i = 0; i < 50000; i++) {
        _esc.Add(i, dims);
        _esc.SetPosition(i, new Vector3f(i, 0, 0), dir);
      }
      // 36.4 sec für 50k agents.
      Console.WriteLine(stopwatch.ElapsedMilliseconds);

    }
  }
}