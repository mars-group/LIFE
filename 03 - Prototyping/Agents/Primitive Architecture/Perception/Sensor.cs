using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Perception {
  
  /// <summary>
  ///   This sensor class provides an abstract base for custom implementations.
  /// </summary>
  internal abstract class Sensor {
 
    public bool Enabled { set; get; }    // Shows if sensor is operational or not.
    private bool _active;                // This boolean controls polling or callback mode.
    private SensorInput _lastInput;      // Container for the last retrieved sensor input.
    protected IGenericDataSource Source; // The data source (environment) to sense. 
    public int InformationType { get; private set; }  // Linkage to the data type to acquire.
    //TODO: A sensor needs a halo and a position reference to transmit to the source.
    //TODO: Maybe the halo contains the position reference ... (← in via constr.)


    /// <summary>
    ///   Create an abstract sensor, serving as a base for either a type-specific
    ///   sensor or a generic sensor that acquires a given type of information.
    /// </summary>
    protected Sensor(int informationType) {
      InformationType = informationType;
      Enabled = true;
      _active = true;
      _lastInput = null;
    }


    /// <summary>
    ///   Retrieve the sensor data.
    /// </summary>
    /// <returns>The latest sensor information.</returns>
    public SensorInput Sense() {
      if (!Enabled) return null;
      if (_active) _lastInput = RetrieveData();
      return _lastInput;
    }


    /// <summary>
    ///   This method shall provide an implementation for active information retrieval.
    /// </summary>
    /// <returns>An information object, acquired via some polling function.</returns>
    protected abstract SensorInput RetrieveData();


    /// <summary>
    ///   Switch this sensor between active (polling) and reactive (callback) mode.
    /// </summary>
    /// <param name="active">Set this sensor active (true) or passive (false).</param>
    /// <returns>Return value tells, whether this operation succeeded or not.</returns>
    public bool SetActive(bool active) {
      if (!(Source is ICallbackDataSource)) return false;
      if (_active != active) {
        (Source as ICallbackDataSource).SetCallbackMode(!active, _lastInput);
        _active = active;
      }
      return true;
    }
  }
}