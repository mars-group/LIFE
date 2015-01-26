using System;
using System.Threading;
using DalskiAgent.Auxiliary.Environment;
using DalskiAgent.Auxiliary.OpenGL;

namespace DalskiAgent.Auxiliary {

  public class TestStart {
    private readonly OpenGLEngine _engine;

    public TestStart() {
      _engine = new OpenGLEngine(640, 480);
      var env = new Env25(30, 20);
      _engine.Objects.Add(env);
    }


    /// <summary>
    ///   Execution routine. Sends a tick to the environment container.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    private void Run(int delay) {
      while (_engine.Run()) {
        if (delay == 0) Console.ReadLine();
        else Thread.Sleep(delay);
      }
    }



    public static void Main() {
      new TestStart().Run(50);
      Console.WriteLine("OpenGL engine has ended. Press return to exit.");
      Console.ReadLine();
    }
  }
}
