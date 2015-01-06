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
using CSharpQuadTree;
using LifeAPI.Agent;

namespace WaterLayer {
    public class Waterhole : IAgent, IQuadObject {
        public double Capacity { get { return _capacity; } }
        private double _capacity;

        public Waterhole(double x, double y, Size size) {
            _capacity = 100.0;
            Bounds = new Rect(x, y, size.Width, size.Height);
        }

        public double TakeWater(double amount) {
            double res = _capacity - amount;
            if (res >= 0) {
                _capacity = res;
                return amount;
            }
            else {
                _capacity = 0;
                return amount + res;
            }
        }

        #region ITickClient implementation

        public void Tick() {
            // just reset for now, infinite waterhole
            _capacity = 100;
        }

        #endregion

        #region IQuadObject implementation

        public event EventHandler BoundsChanged;

        public Rect Bounds { get; set; }

        #endregion

        public Guid ID { get; set; }
    }
}