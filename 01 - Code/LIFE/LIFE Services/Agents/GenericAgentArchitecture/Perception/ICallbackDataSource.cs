namespace GenericAgentArchitecture.Perception {
  public interface ICallbackDataSource {
    void SetCallbackMode(bool enabled, SensorInput inputStorage);
  }
}