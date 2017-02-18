namespace LIFE.Components.GridPotentialFieldLayer {

  public interface IFieldLoader<out TPotentialFieldType> where TPotentialFieldType : PotentialField {

    TPotentialFieldType LoadPotentialField(string filePath);
  }
}