// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Windows;
using CSharpQuadTree;
using LayerAPI.Interfaces;

namespace PlantLayer.Agents {
    public class Plant : IAgent, IQuadObject {
        private double _health;

        private Rect _bounds;

        public Plant(float x, float y, Size size) {
            _bounds.X = x;
            _bounds.Y = y;
            _bounds.Size = size;
            _health = 100;
        }

        #region IAgent Members

        public void Tick() {
            if (_health > 0.0) {
                // regenerate a tiny bit
                //_health += _health * 0.0001;
            }
        }

        #endregion

        #region IQuadObject Members

        public Rect Bounds { get { return _bounds; } set { _bounds = value; } }

        public event EventHandler BoundsChanged;

        #endregion

        public double GetHealth() {
            return _health;
        }

        public void SubHealth(double x) {
            _health -= x;
        }
    }
}