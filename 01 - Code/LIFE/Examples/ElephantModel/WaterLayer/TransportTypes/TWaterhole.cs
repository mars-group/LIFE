// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System.Windows;

namespace WaterLayer {
    public class TWaterhole {
        public double Capacity { get; private set; }
        public Rect Bounds { get; private set; }
        private double _capacity;
        private Rect _bounds;

        public TWaterhole(Waterhole w) {
            _capacity = w.Capacity;
            _bounds = w.Bounds;
        }
    }
}