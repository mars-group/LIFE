// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using MessageWrappers;

namespace VisualizationAdapter.Interface {
    public interface IVisualizationAdapterPublic {
        event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        void StartVisualization(int? nrOfTicksToVisualize = null);

        void StopVisualization();

        void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight);
    }
}