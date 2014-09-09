using System;
using GenericAgentArchitecture.Perception;

namespace GenericAgentArchitecture.Interfaces {
  public interface IGenericDataSource {
    //SensorInput GetData(int dataType);

    SensorInput GetData(int infType);

/*
    Überladungen für Position, Sichtbereich, ...
    Rückgabe: JSON
    Sensoren nutzen Implementation dieser Schnittstelle für Datenabruf.
    Generischer Sensor hat Datentyp und nimmt gemäß verfügbarer Daten Abfrage auf DataSource-Objekt vor.
    Implementation setzt den Datentyp und erledigt die Umwandlung in das SensorInput-Objekt. 
*/
  }

  // So würde eine konkrete Datenquelle aussehen. Alle T's werden automatisch durch float ersetzt.
  //public interface ITemperatureDataSource : IGenericDataSource<float> {}

 

/*
  public class GenericSensor {
    private InformationType _infType;
    private IGenericDataSource _dataSource;

    public void Sense() {

      var returnValue = _dataSource.GetData(_infType);
      
      var casted = (T) returnValue;
    }
   // protected T
  }


  public class InformationType {
    public Type returnType;
    //additional information.
  }


  public interface GenericInformationObject {
    void SetString(string str);
    void SetFloat(float f);
    void SetInteger(int i);
    Object GetObject(); 
  }
*/
}