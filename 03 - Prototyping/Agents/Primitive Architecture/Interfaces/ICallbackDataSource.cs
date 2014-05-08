using Primitive_Architecture.Perception;

namespace Primitive_Architecture.Interfaces {
  internal interface ICallbackDataSource {
    void SetCallbackMode(bool enabled, SensorInput inputStorage);
  }
}