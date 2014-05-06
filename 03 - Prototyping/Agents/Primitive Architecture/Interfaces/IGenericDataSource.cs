using Primitive_Architecture.Perception;

namespace Primitive_Architecture.Interfaces {
  internal interface IGenericDataSource {
    SensorInput GetData(int dataType);
  }
}