using GenericAgentArchitecture.Perception;

namespace GenericAgentArchitecture.Interfaces {
  internal interface IGenericDataSource {
    SensorInput GetData(int dataType);
  }
}