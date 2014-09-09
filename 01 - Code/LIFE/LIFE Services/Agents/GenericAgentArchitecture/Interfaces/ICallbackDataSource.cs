using GenericAgentArchitecture.Perception;

namespace GenericAgentArchitecture.Interfaces {
  public interface ICallbackDataSource {
    void SetCallbackMode(bool enabled, SensorInput inputStorage);
  }
}