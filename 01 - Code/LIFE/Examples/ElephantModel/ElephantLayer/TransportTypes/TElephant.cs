// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System;
using System.Windows;
using ElephantLayer.Agents;
using PlantLayer;
using TwoDimEnvironment;
using WaterLayer;

namespace ElephantLayer.TransportTypes {
    public class TElephant {
        public Location Center {
            get { return new Location() {X = _bounds.X + _bounds.Width/2, Y = _bounds.Y - _bounds.Height/2}; }
        }

        private static int totalElephants = 0;

        private readonly int elephantNumber;
        private readonly bool isThirsty;
        private readonly double direction;
        private readonly int visionDistance;
        private readonly double stepLength;
        private readonly int steps;
        private readonly PlantLayerImpl plantLayer;
        private readonly WaterLayerImpl waterLayer;
        private readonly ITwoDimEnvironment<Elephant> twoDimEnv;
        private readonly double maxTurningRadius = Math.PI/3;
        private readonly double idealSeperation = 20;
        private int rank = 1; //This will matter later
        private Rect _bounds;

        public TElephant(Elephant elephant) {
            rank = elephant.getRank();
            _bounds = elephant.Get_bounds();
            direction = elephant.getDirection();
            steps = elephant.GetSteps();
            visionDistance = elephant.GetVisionDistance();
            stepLength = elephant.GetStepLength();

            plantLayer = elephant.GetPlantLayer();
            waterLayer = elephant.GetWaterLayer();
            twoDimEnv = elephant.GetTwoDimEnv();
            isThirsty = elephant.IsThirsty();
            elephantNumber = elephant.GetElephantNumber();
            maxTurningRadius = elephant.getMaxTurningRadius();
            idealSeperation = elephant.getIdealSeperation();
        }

        public event EventHandler BoundsChanged;

        public bool IsThirsty() {
            return isThirsty;
        }

        public Rect GetBounds() {
            return _bounds;
        }

        public int GetSteps() {
            return steps;
        }

        public int GetVisionDistance() {
            return visionDistance;
        }

        public double GetStepLength() {
            return stepLength;
        }

        public PlantLayerImpl GetPlantLayer() {
            return plantLayer;
        }

        public WaterLayerImpl GetWaterLayer() {
            return waterLayer;
        }

        public ITwoDimEnvironment<Elephant> GetTwoDimEnv() {
            return twoDimEnv;
        }

        public int GetElephantNumber() {
            return elephantNumber;
        }

        public double getMaxTurningRadius() {
            return maxTurningRadius;
        }

        public double getDirection() {
            return direction;
        }

        public double getIdealSeperation() {
            return idealSeperation;
        }
    }
}