using System;

namespace Primitive_Architecture.Perception {
  
  /// <summary>
  /// This is the base class for any input types (sensory information, communication).
  /// </summary>
  internal class Input {

    public long Timestamp { get; private set; }  // Input creation time.

    /// <summary>
    /// Base constructor for any new input object. Provides the input timestamp.
    /// </summary>
    public Input() {
      Timestamp = DateTime.Now.Ticks;
    }
  }
}