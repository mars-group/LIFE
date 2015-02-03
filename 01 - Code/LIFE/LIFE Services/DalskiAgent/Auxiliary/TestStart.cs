using System;
using System.Threading;
using System.Threading.Tasks;
using DalskiAgent.Auxiliary.Environment;
using DalskiAgent.Auxiliary.OpenGL;

namespace DalskiAgent.Auxiliary {

  public class TestStart {
    private readonly OpenGLEngine _engine;

    private TestStart() {     
      const int envWidth  = 600;
      const int envHeight = 600;
      const int nrAgents  = 3200;

      // Create environment and desired number of spatial entities (unpositioned, 1x1 dimension).
      Env25 env = new Env25(envWidth, envHeight);
      Parallel.For(0, nrAgents, i1 => env.AddWithRandomPosition(new Obj(new Float2(0, 0), new Float2(1, 1))));      
      
      _engine = new OpenGLEngine(640, 480);
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


    /// <summary>
    ///   Main function for sequential execution.
    ///   Yep, it's the ultimate evil ...
    /// </summary>
    public static void Main() {
      new TestStart().Run(1);
    }
  }
}
