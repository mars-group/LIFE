using System;
using System.Threading;
using Environment = GenericAgentArchitecture.Environments.Environment;

namespace GenericAgentArchitecture.Auxiliary {
  
  /// <summary>
  ///   This class periodicly triggers the environment and thereby all agents.
  ///   It only exists for very simple testing reasons ans should not be used otherwise!
  /// </summary>
  public class Executor {
    
    private readonly Environment _environment; // The agent container.
    private readonly ConsoleView _view;        // The console view module.


    /// <summary>
    ///   Instantiate a runtime.
    ///   <param name="environment">The environment to execute.</param>
    ///   <param name="view">The console view module.</param>
    /// </summary>
    public Executor(Environment environment, ConsoleView view) {
      _environment = environment;
      _view = view;
    }


    /// <summary>
    ///   Execution routine. Sends a tick to the environment container.
    /// </summary>
    /// <param name="delay">Thread delay (in ms), 0 for manual execution.</param>
    public void Run(int delay) {
      while (true) {
        _environment.Tick();
        if (_view != null) _view.Print();

        // Manual or automatic execution.
        if (delay == 0) Console.ReadLine();
        else {
          Console.WriteLine();
          Thread.Sleep(delay);
        }
      }
      // ReSharper disable once FunctionNeverReturns
    }
  }
}
