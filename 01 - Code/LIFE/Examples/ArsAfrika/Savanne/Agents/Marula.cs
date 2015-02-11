using System;
using GeoAPI.Geometries;
using GisCoordinatesLayer;
using LifeAPI.Agent;
using SpatialAPI.Environment;

namespace Savanne.Agents {

    /// <summary>
    ///     the marula attributes are disigned by ulfias first diagramms
    /// </summary>
    internal class Marula : IAgent {
        #region Sex enum

        public enum Sex {
            Male,
            Female
        }

        #endregion

        private readonly IGisCoordinatesLayer _gisCoordinatesLayer;

        private int _heigthInCm;
        private int _age;
        private int _damageType;
        private int _stemDiameterInMm;
        private Sex _sex;
        public double PosX;
        public double PosY;
        public double visualisationPosX;
        public double visualisationPosY;



        public Marula
            (IEnvironment esc,
                int heightInCm,
                int age,
                int damageType,
                int stemDiameterInMm,
                Sex sex,
                double posX,
                double posY,
                IGisCoordinatesLayer gisCoordinatesLayer) {
            _heigthInCm = heightInCm;
            _age = age;
            _damageType = damageType;
            _stemDiameterInMm = stemDiameterInMm;
            _sex = sex;
            PosX = posX;
            PosY = posY;
            _gisCoordinatesLayer = gisCoordinatesLayer;
            ID = Guid.NewGuid();

            Coordinate transformedFromGpsToVisualisationData = _gisCoordinatesLayer.TransformToImage(PosX, PosY);
            visualisationPosX = transformedFromGpsToVisualisationData.X;
            visualisationPosY = transformedFromGpsToVisualisationData.Y;

            Console.WriteLine("My visualisation coords are: " + visualisationPosX + ", " + visualisationPosY);
            
            }

        #region IAgent Members

        public Guid ID { get; set; }


        public void Tick() {
            //Coordinate transformedFromGpsToVisualisationData = _gisCoordinatesLayer.TransformToImage(PosX, PosY);
            //Console.WriteLine(transformedFromGpsToVisualisationData);
            
        }

        #endregion
    }

}