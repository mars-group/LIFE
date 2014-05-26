namespace Common.Interfaces {

  public interface IPerception {
    T GetData<T>() where T : class;
  }
}