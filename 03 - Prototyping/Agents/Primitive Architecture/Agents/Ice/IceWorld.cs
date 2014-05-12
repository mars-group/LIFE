using System;
using Environment = Primitive_Architecture.Dummies.Environment;

namespace Primitive_Architecture.Agents.Ice {
  
  /// <summary>
  ///   A simple testing environment. Contains only two agents and a sunshine state. 
  /// </summary>
  internal class IceWorld : Environment {

    private bool _sunshine;          // A flag describing whether the sun shines or not.
    private readonly Random _random; // Random number generator.


    /// <summary>
    ///   Initialize the iceman environment.
    /// </summary>
    public IceWorld() : base(null) {
      //TODO Interaction loader missing.  
      _sunshine = false;
      _random = new Random();
    }


    /// <summary>
    ///   Toggle the sunshine boolean randomly.
    /// </summary>
    protected override void AdvanceEnvironment() {
      var rnd = _random.Next(100);
      if (rnd > 70) {
        _sunshine = !_sunshine;
        Console.WriteLine("Sun state changed to "+_sunshine);
      }
    }


    /// <summary>
    ///   Return the sunshine flag. Used by sensor method.
    /// </summary>
    /// <returns>The sunshine boolean.</returns>
    public bool GetSunshine() {
      return _sunshine;
    }


    /// <summary>
    ///   Calculate the distance between two agents. Not used here!
    /// </summary>
    /// <param name="x">The first agent.</param>
    /// <param name="y">The second agent.</param>
    /// <returns>A value describing the distance between these two agents.</returns>
    public override double GetDistance(Agent x, Agent y) {
      return 0;
    }
  }
}