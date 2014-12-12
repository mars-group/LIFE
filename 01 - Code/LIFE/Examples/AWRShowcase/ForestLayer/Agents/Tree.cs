// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.10.2014
//  *******************************************************/

using System;
using LayerAPI.Agent;

namespace ForestLayer.Agents {
    public class Tree : IAgent {
        public Guid TreeId { get; private set; }
        public double Height { get; set; }
        public double Diameter { get; set; }
        public double CrownDiameter { get; set; }
        public double Age { get; set; }
        public double Biomass { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        private const double GParK = 0.18;
        private const double GmaxH = 600;
        private const double GmaxD = 325/3.14;


        public Tree
            (double height, double diameter, double crownDiameter, double age, double biomass, double lat, double lon) {
            TreeId = Guid.NewGuid();
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
            Lat = lat;
            Lon = lon;
        }

        #region IAgent Members

        public void Tick() {
            // grow diameter
            Diameter = Diameter + GParK*(GmaxD - Diameter);
            // grow height
            Height = Height + GParK*(GmaxH - Height);
            // grow biomass
            Biomass = Math.Pow(Math.E, -3.00682 + 1.56775*Math.Log(Diameter*Height));
        }

        #endregion
    }
}