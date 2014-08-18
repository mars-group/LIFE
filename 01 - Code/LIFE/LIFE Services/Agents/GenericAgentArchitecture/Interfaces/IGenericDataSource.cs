using GenericAgentArchitecture.Perception;

namespace GenericAgentArchitecture.Interfaces {
  public interface IGenericDataSource {
    SensorInput GetData(int dataType);
  }
}