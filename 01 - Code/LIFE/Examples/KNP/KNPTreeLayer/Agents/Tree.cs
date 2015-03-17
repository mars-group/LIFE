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
using GeoAPI.Geometries;
using KNPElevationLayer;
using NetTopologySuite.Geometries;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;

namespace TreeLayer.Agents {
    public class Tree : AscService, ITree {
        private double _biomass;
        private Coordinate _imageCoordinates;
        public Guid ID { get; set; }
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
                }
            }
        }

        public double HeightAboveNN { get; set; }

        public double Lat { get; set; }
        public double Lon { get; set; }

        public ISpatialEntity SpatialEntity { get; set; }

        public string GetIdentifiaction()
        {
            return ID.ToString();
        }

        private const double GParK = 0.18;
        private const double GmaxH = 600;
        private const double GmaxD = 325/3.14;


        public Tree
            (double height, double diameter, double crownDiameter, double age, double biomass, double lat, double lon, Guid id, KNPTreeLayer.TreeLayer treeLayer, IKnpElevationLayer elevationLayer) : base(id.ToByteArray()) {
            _imageCoordinates = elevationLayer.TransformToImage(lat, lon);
            SpatialEntity = new SpatialTreeEntity(_imageCoordinates.X,_imageCoordinates.Y,id);
            ID = id;
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

        #region IAgent Members

        public void Tick() {
            // grow diameter
            Diameter = Diameter + GParK*(GmaxD - Diameter);
            // grow height
            Height = Height + GParK*(GmaxH - Height)*(HeightAboveNN/0.001);
            // grow biomass
            Biomass = Math.Pow(Math.E, -3.00682 + 1.56775*Math.Log(Diameter*Height));
        }

        #endregion

        private class SpatialTreeEntity : ISpatialEntity {

            public SpatialTreeEntity(double x, double y, Guid id) {
                Shape = new Cuboid(new Vector3(1, 2, 1), new Vector3(x, y));
                AgentGuid = id;
            }

            public IShape Shape { get; set; }

            public Enum CollisionType {
                get { return SpatialAPI.Entities.Movement.CollisionType.MassiveAgent; }
            }

            public Guid AgentGuid { get; set; }
        }

    }


}