// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CSharpQuadTree;
using ElephantLayer.TransportTypes;
using LifeAPI.Agent;
using PlantLayer;
using TwoDimEnvironment;
using WaterLayer;

namespace ElephantLayer.Agents {
    public class Elephant : IAgent, IQuadObject {
        public Location Center {
            get { return new Location() {X = _bounds.X + _bounds.Width/2, Y = _bounds.Y - _bounds.Height/2}; }
        }

        private static int totalElephants = 0;

        private readonly int elephantNumber;
        private readonly int visionDistance;
        private readonly double stepLength;
        private readonly PlantLayerImpl plantLayer;
        private readonly WaterLayerImpl waterLayer;
        private readonly ITwoDimEnvironment<Elephant> twoDimEnv;
        private int rank = 1; //This will matter later
        private bool isThirsty;
        private double radius;
        private double direction;
        private int steps;
        private Rect _bounds;
        private double maxTurningRadius = Math.PI/3;
        private double idealSeperation = 20;

        public Elephant
            (float x,
                float y,
                Size size,
                double direction,
                int visionDistance,
                double stepLength,
                PlantLayerImpl plantLayer,
                WaterLayerImpl waterLayer,
                ITwoDimEnvironment<Elephant> twoDimEnv,
                Guid agentID) {
            ID = agentID;
            _bounds.X = x;
            _bounds.Y = y;
            _bounds.Size = size;
            this.direction = direction;
            steps = 0;
            this.visionDistance = visionDistance;
            this.stepLength = stepLength;

            this.plantLayer = plantLayer;
            this.waterLayer = waterLayer;
            this.twoDimEnv = twoDimEnv;
            isThirsty = true;
            elephantNumber = ++totalElephants;
        }

        #region IAgent Members

        public void Tick() {
            Turn();
            Move();
            List<TWaterhole> actList = waterLayer.Probe(_bounds.X, _bounds.Y, 1);
            if (actList.Count > 0)
                isThirsty = false;
            steps++;
        }

        #endregion

        #region IQuadObject Members

        public Rect Bounds { get { return _bounds; } set { _bounds = value; } }
        public event EventHandler BoundsChanged;

        #endregion

        public void Move() {
            double x = _bounds.X + stepLength*Math.Cos(direction);
            double y = _bounds.Y + stepLength*Math.Sin(direction);
            double locx = _bounds.X;
            twoDimEnv.Move(this, x, y);
            plantLayer.Stomp(_bounds, 1);
        }

        public void Turn() {
            double water = GetWaterholeFactor();
            double random = GetRandomTurnFactor();
            double repulsion = GetRepulsionFactor();
            double attraction = GetAttractionFactor();
            double alignment = GetAlignmentFactor();
            double sum = water + random + repulsion + attraction + alignment;
            double avg = sum/5;
            double angle = FixTurnAngle(avg);
            setDirection(getDirection() + angle);
        }

        public override String ToString() {
            return "Elephant #" + elephantNumber + ". Total steps = " + steps;
        }

        private double GetRandomTurnDirection() {
            double angle = new Random().NextDouble()*getMaxTurningRadius()*2 - getMaxTurningRadius();
            return angle;
        }

        private double GetWaterholeDirection(TWaterhole actor) {
            double angle = getTurnAngleTo
                (new Location() {X = actor.Bounds.X + actor.Bounds.Width/2, Y = actor.Bounds.Y - actor.Bounds.Height/2});
            angle = FixTurnAngle(angle);
            return angle;
        }

        private double GetRepulsionDirection(TElephant actor) {
            Location loc = actor.Center;
            double angle = getTurnAngleTo(loc);
            /*
		if(angle>Math.PI/2 || angle < -Math.PI/2)
			return 0;
			*/
            angle = FixTurnAngle(angle + Math.PI);
            return angle;
        }

        private double GetAttractionDirection(TElephant actor) {
            Location loc = actor.Center;
            double angle = getTurnAngleTo(loc);
            angle = FixTurnAngle(angle);
            return angle;
        }

        private double GetAlignmentDirection(TElephant actor) {
            double angle = actor.getDirection() - getDirection();
            angle = FixTurnAngle(angle);
            return angle;
        }

        private double getRandomTurnFactorWeight() {
            return 1;
        }

        private double GetWaterholeFactorWeight(TWaterhole actor) {
            if (!isThirsty)
                return 0;
            double distance = distanceTo
                (new Location() {X = actor.Bounds.X + actor.Bounds.Width/2, Y = actor.Bounds.Y - actor.Bounds.Height/2});
            return 1/Math.Sqrt(distance);
        }

        private double GetRepulsionFactorWeight(TElephant actor) {
            double distance = distanceTo(actor.Center);
            if (distance > idealSeperation)
                return 0;
            else
                return (-distance/idealSeperation) + 1;
        }

        private double GetAttractionFactorWeight(TElephant actor) {
            double distance = distanceTo(actor.Center);
            if (distance < idealSeperation)
                return 0;
            else
                return ((distance - idealSeperation)/idealSeperation);
        }

        private double GetAlignmentFactorWeight(TElephant actor) {
            return 1;
//		return 1/3 + 1/rank;
        }

        private double GetWaterholeFactor() {
            List<TWaterhole> actList = waterLayer.Probe(_bounds.X, _bounds.Y, visionDistance);
            ;
            if (actList.Count() == 0)
                return 0;
            double sum = 0;
            foreach (TWaterhole actor in actList) {
                sum += GetWaterholeDirection(actor)*GetWaterholeFactorWeight(actor);
            }
            return sum;
        }

        private double GetRandomTurnFactor() {
            return GetRandomTurnDirection()*getRandomTurnFactorWeight();
        }

        private double GetRepulsionFactor() {
            List<Elephant> actList1 = twoDimEnv.Find(this, visionDistance);
            List<TElephant> actList2 = new List<TElephant>();
            foreach (Elephant actor in actList1) {
                actList2.Add(new TElephant(actor));
            }
            if (actList2.Count() == 0)
                return 0;
            double sum = 0;
            int num = 0;
            foreach (TElephant actor in actList2) {
                sum += GetRepulsionDirection(actor)*GetRepulsionFactorWeight(actor);
                num++;
            }
            if (num == 0)
                return 0;
            return sum/num;
        }

        private double GetAttractionFactor() {
            List<Elephant> actList1 = twoDimEnv.Find(this, visionDistance);
            List<TElephant> actList2 = new List<TElephant>();
            foreach (Elephant actor in actList1) {
                actList2.Add(new TElephant(actor));
            }
            if (actList2.Count() == 0)
                return 0;
            double sum = 0;
            int num = 0;
            foreach (TElephant actor in actList2) {
                sum += GetAttractionDirection(actor)*GetAttractionFactorWeight(actor);
                num++;
            }
            if (num == 0)
                return 0;
            return sum/num;
        }

        private double GetAlignmentFactor() {
            List<Elephant> actList1 = twoDimEnv.Find(this, visionDistance);
            List<TElephant> actList2 = new List<TElephant>();
            foreach (Elephant actor in actList1) {
                actList2.Add(new TElephant(actor));
            }
            if (actList2.Count() == 0)
                return 0;
            double sum = 0;
            int num = 0;
            foreach (TElephant actor in actList2) {
                sum += GetAlignmentDirection(actor)*GetAlignmentFactorWeight(actor);
                num++;
            }
            if (num == 0)
                return 0;
            return sum/num;
        }

        private double FixTurnAngle(double angle) {
            angle %= Math.PI*2;
            if (angle > Math.PI)
                angle -= Math.PI*2;
            if (angle < -Math.PI)
                angle += Math.PI*2;
            if (Math.PI == angle || -Math.PI == angle)
                return 0;
            if (angle > getMaxTurningRadius())
                return getMaxTurningRadius();
            if (angle < -getMaxTurningRadius())
                return -getMaxTurningRadius();
            return angle;
        }

        public double getMaxTurningRadius() {
            return maxTurningRadius;
        }

        public double getDirection() {
            return direction;
        }

        public void setDirection(double direction) {
            this.direction = direction;
        }

        public double getIdealSeperation() {
            return idealSeperation;
        }

        public void setIdealSeperation(double idealSeperation) {
            this.idealSeperation = idealSeperation;
        }

        private double getTurnAngleTo(Location loc) {
            double angle = getAngleTo(loc);
            angle -= direction;
            if (angle < -Math.PI)
                angle += 2*Math.PI;
            return angle;
        }

        public double getAngleTo(Location otherLoc) {
            double yDist = otherLoc.Y - Center.Y;
            double xDist = otherLoc.X - Center.X;
            double angle;
            if (xDist == 0) {
                if (yDist > 0)
                    angle = Math.PI/2;
                else
                    angle = -Math.PI/2;
            }
            else
                angle = Math.Atan(yDist/xDist);

            if (Center.X > otherLoc.X) {
                if (angle < 0)
                    angle += Math.PI;
                else
                    angle -= Math.PI;
            }
            return angle;
        }

        public double distanceTo(Location loc) {
            double xDist = (Center.X - loc.X);
            double yDist = (Center.Y - loc.Y);
            double distance = Math.Sqrt(xDist*xDist + yDist*yDist);
            return distance;
        }

        public bool IsThirsty() {
            return isThirsty;
        }

        public Rect Get_bounds() {
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

        public int getRank() {
            return rank;
        }

        public Guid ID { get; set; }
    }
}