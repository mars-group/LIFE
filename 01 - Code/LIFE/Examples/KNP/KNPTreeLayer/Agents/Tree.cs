// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.10.2014
//  *******************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using DalskiAgent.Agents;
using DalskiAgent.Reasoning;
using KNPElevationLayer;
using LifeAPI.Layer;
using NetTopologySuite.Geometries;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Environment;
using SpatialAPI.Shape;

namespace TreeLayer.Agents {
    public class Tree : SpatialAgent, ITree {

        private double _biomass;
        private readonly KNPTreeLayer.TreeLayer _treeLayer;
        private IEnvironment _environment;

        public double Height { get; set; }
        public double Diameter { get; set; }
        public double CrownDiameter { get; set; }
        public double Age { get; set;}

        public double Biomass
        {
            get
            {
                return _biomass;
            }
            set
            {
                if (_biomass != value)
                {
                    _biomass = value;
                    OnPropertyChanged("Biomass");
                }
            }
        }

        public double HeightAboveNN { get; set; }

        public double Lat { get; set; }
        public double Lon { get; set; }
        public string GetIdentifiaction()
        {
            return ID.ToString();
        }

        private const double GParK = 0.18;
        private const double GmaxH = 600;
        private const double GmaxD = 325/3.14;


        public Tree
            (double height, double diameter, double crownDiameter, double age, double biomass,
            double lat, double lon, Guid id, 
            KNPTreeLayer.TreeLayer treeLayer, IKnpElevationLayer elevationLayer, RegisterAgent registerAgent, UnregisterAgent unregisterAgent, IEnvironment env)
            : base(treeLayer, registerAgent, unregisterAgent, env,
            new Cuboid(new Vector3(1,1,1),new Vector3(lat, lon, 0))) 
        {
            _treeLayer = treeLayer;
            _environment = env;

            // AscService ID
            ServiceID = id;
            // DalskiAgent ID
            ID = ServiceID;


            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
            Lat = lat;
            Lon = lon;
            var result = elevationLayer.GetDataByGeometry(new Point(Lat, Lon));
            HeightAboveNN = Double.Parse(result.ResultEntries.First().Value.ToString());
        }

        protected IInteraction Reason() {
            var result = _environment.ExploreAll();
            // grow diameter
            Diameter = Diameter + GParK * (GmaxD - Diameter);
            // grow height
            Height = Height + GParK * (GmaxH - Height) * (HeightAboveNN / 0.001);
            // grow biomass
            Biomass = Math.Pow(Math.E, -3.00682 + 1.56775 * Math.Log(Diameter * Height));

            ITree otherTree = _treeLayer.GetTreeById(result.First().AgentGuid);

            return new ConsumeTreeInteraction(this, otherTree);
        }


        #region PropertyChanged Handling
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}