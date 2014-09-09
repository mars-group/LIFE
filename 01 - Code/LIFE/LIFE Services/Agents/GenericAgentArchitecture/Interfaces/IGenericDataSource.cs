using System;
using GenericAgentArchitecture.Perception;

namespace GenericAgentArchitecture.Interfaces {
  public interface IGenericDataSource {
    Object GetData(int informationType, Halo halo);
  }
}