﻿// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.10.2014
//  *******************************************************/

using System;
using System.ComponentModel;
using ASC.Communication.ScsServices.Service;
using KNPElevationLayer;

namespace TreeLayer.Agents {
    public class Tree : AscService, ITree {

        private readonly IKnpElevationLayer _elevationLayer;
        private readonly KNPTreeLayer.TreeLayer _treeLayer;
        private readonly bool _sendingNote;

        private double _biomass;

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
                    OnPropertyChanged("Biomass");
                }
            }
        }

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
            (double height, double diameter, double crownDiameter, double age, double biomass, double lat,
            double lon, Guid id, 
            KNPTreeLayer.TreeLayer treeLayer, bool sendingNote = false) : base(id.ToByteArray()) 
        {
            //_elevationLayer = elevationLayer; 
            _treeLayer = treeLayer;
            _sendingNote = sendingNote;
            ID = id;
            Height = height;
            Diameter = diameter;
            CrownDiameter = crownDiameter;
            Age = age;
            Biomass = biomass;
            Lat = lat;
            Lon = lon;
        }

        #region IAgent Members

        public void Tick()
        {
            if (_sendingNote) { 
                var otherTrees = _treeLayer.GetAllOtherTreesThanMe(this);
                //Console.WriteLine("Local tree reportin in, found " + otherTrees.Count + " other trees: ");
                foreach (var tree in otherTrees) {
                    tree.GetIdentifiaction();
                    var tage = tree.Biomass;
                    
                    //Console.WriteLine("OtherTree with ID: "+tree.GetIdentifiaction()+" has biomass: " + tree.Biomass);
                }
            }

            //Age++;
            // grow diameter
            Diameter = Diameter + GParK*(GmaxD - Diameter);
            // grow height
            Height = Height + GParK*(GmaxH - Height);
            // grow biomass
            Biomass = Math.Pow(Math.E, -3.00682 + 1.56775*Math.Log(Diameter*Height));
        }

        #endregion

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