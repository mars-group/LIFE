using System;
using System.Windows;
using ElephantLayer.Agents;
using PlantLayer;
using TwoDimEnvironment;
using WaterLayer;

namespace ElephantLayer.TransportTypes
{


    public class TElephant
    {
        private static int totalElephants = 0;

        private int rank = 1; //This will matter later
        private readonly int elephantNumber;
        private bool isThirsty;
        private double direction;
        private int visionDistance;
        private double stepLength;
        private int steps;
        private Rect _bounds;
        private PlantLayerImpl plantLayer;
        private WaterLayerImpl waterLayer;
        private ITwoDimEnvironment<Elephant> twoDimEnv;
        private double maxTurningRadius = Math.PI / 3;
        private double idealSeperation = 20;

        public TElephant(Elephant elephant) {
            this.rank = elephant.getRank();
            _bounds = elephant.Get_bounds();
            this.direction = elephant.getDirection();
            this.steps = elephant.GetSteps();
            this.visionDistance = elephant.GetVisionDistance();
            this.stepLength = elephant.GetStepLength();

            this.plantLayer = elephant.GetPlantLayer();
            this.waterLayer = elephant.GetWaterLayer();
            this.twoDimEnv = elephant.GetTwoDimEnv();
            this.isThirsty = elephant.IsThirsty();
            this.elephantNumber = elephant.GetElephantNumber();
            this.maxTurningRadius = elephant.getMaxTurningRadius();
            this.idealSeperation = elephant.getIdealSeperation();
        }

        public Location Center
        {
            get { return new Location() { X = _bounds.X + _bounds.Width / 2, Y = _bounds.Y - _bounds.Height / 2 }; }
        }

        public event EventHandler BoundsChanged;

        public bool IsThirsty()
        {
            return isThirsty;
        }

        public Rect GetBounds()
        {
            return _bounds;
        }

        public int GetSteps()
        {
            return steps;
        }

        public int GetVisionDistance()
        {
            return visionDistance;
        }

        public double GetStepLength()
        {
            return stepLength;
        }

        public PlantLayerImpl GetPlantLayer()
        {
            return plantLayer;
        }

        public WaterLayerImpl GetWaterLayer()
        {
            return waterLayer;
        }

        public ITwoDimEnvironment<Elephant> GetTwoDimEnv()
        {
            return twoDimEnv;
        }

        public int GetElephantNumber()
        {
            return elephantNumber;
        }
        public double getMaxTurningRadius()
        {
            return maxTurningRadius;
        }
        public double getDirection()
        {
            return direction;
        }

        public double getIdealSeperation()
        {
            return idealSeperation;
        }
    }
}
