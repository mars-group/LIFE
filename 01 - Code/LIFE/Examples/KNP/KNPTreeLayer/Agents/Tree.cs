// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.10.2014
//  *******************************************************/

using System;
using System.Linq;
using ASC.Communication.ScsServices.Service;
using KNPElevationLayer;
using NetTopologySuite.Geometries;

namespace TreeLayer.Agents {
    public class Tree : AscService, ITree {
        private readonly IKnpTreeLayer _treeLayer;
        private readonly Guid[] _clusterGroup;
        public Guid ID { get; set; }
        public double Height { get; set; }
        public double Diameter { get; set; }
        public double CrownDiameter { get; set; }
        public double Age { get; set;}
        public double Biomass { get; set; }
        public double HeightAboveNN { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        private const double GParK = 0.18;
        private const double GmaxH = 600;
        private const double GmaxD = 325/3.14;


        public Tree
            (double height, double diameter, double crownDiameter, double age, double biomass, double lat, double lon,
            Guid id, TreeLayer treeLayer, IKnpElevationLayer elevationLayer, Guid[] clusterGroup) 
            : base(id.ToByteArray()) {
            _treeLayer = treeLayer;
            _clusterGroup = clusterGroup;
            ID = id;
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
            Lat = lat;
            Lon = lon;
            // fetch and parse HeightAboveNN data from ElevationLayer
            var result = elevationLayer.GetDataByGeometry(new Point(Lat, Lon));
            HeightAboveNN = Double.Parse(result.ResultEntries.First().Value.ToString());
        }

        #region IAgent Members

        public void Tick() {
            foreach (var agentId in _clusterGroup)
            {
                ITree otherTree;
                if (_treeLayer.GetTreeById(agentId, out otherTree))
                {
                    // obtain other tree's height - symbolic handshake operation
                    var otherTreesHeight = otherTree.Height;
                }
            }

            // grow diameter
            Diameter = Diameter + GParK*(GmaxD - Diameter);
            // grow height
            Height = Height + GParK*(GmaxH - Height)*(HeightAboveNN/0.001);
            // grow biomass
            Biomass = Math.Pow(Math.E, -3.00682 + 1.56775*Math.Log(Diameter*Height));
        }

        #endregion
    }


}