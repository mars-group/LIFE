﻿// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System.Windows;
using PlantLayer.Agents;

namespace PlantLayer {
    public class TPlant {
        private readonly double _health;
        private readonly Rect _bounds;

        public TPlant(Plant p) {
            _health = p.GetHealth();
            _bounds = p.Bounds;
        }

        public double GetHealth() {
            return _health;
        }

        public Rect GetBounds() {
            return _bounds;
        }
    }
}