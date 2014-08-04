namespace GenericAgentArchitecture.Interfaces {

  public interface IPerception {
    T GetData<T>() where T : class;
  }
}