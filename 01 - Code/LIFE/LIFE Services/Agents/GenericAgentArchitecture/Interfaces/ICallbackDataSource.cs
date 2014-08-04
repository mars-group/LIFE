using GenericAgentArchitecture.Perception;

namespace GenericAgentArchitecture.Interfaces {
  internal interface ICallbackDataSource {
    void SetCallbackMode(bool enabled, SensorInput inputStorage);
  }
}