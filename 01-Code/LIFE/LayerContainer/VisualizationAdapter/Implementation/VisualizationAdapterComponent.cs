//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using LifeAPI.Layer.Visualization;
using LIFEViewProtocol.Basics;
using VisualizationAdapter.Interface;

namespace VisualizationAdapter.Implementation {
    public class VisualizationAdapterComponent : IVisualizationAdapterInternal {
        private readonly IVisualizationAdapterInternal _visualizationAdapterInternalUseCase;

        public VisualizationAdapterComponent() {
            _visualizationAdapterInternalUseCase = new VisualizationAdapterUseCase();
            _visualizationAdapterInternalUseCase.VisualizationUpdated +=
                _visualizationAdapterInternalUseCase_VisualizationUpdated;
        }

        #region IVisualizationAdapterInternal Members

        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

        public void StartVisualization(int? nrOfTicksToVisualize = null) {
            _visualizationAdapterInternalUseCase.StartVisualization(nrOfTicksToVisualize);
        }

        public void StopVisualization() {
            _visualizationAdapterInternalUseCase.StopVisualization();
        }

        public void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight) {
            _visualizationAdapterInternalUseCase.ChangeVisualizationView(topLeft, topRight, bottomLeft, bottomRight);
        }

        public void RegisterVisualizable(IVisualizable visualizable) {
            _visualizationAdapterInternalUseCase.RegisterVisualizable(visualizable);
        }

        public void VisualizeTick(int currentTick) {
            _visualizationAdapterInternalUseCase.VisualizeTick(currentTick);
        }

        #endregion

        private void _visualizationAdapterInternalUseCase_VisualizationUpdated
            (object sender, List<BasicVisualizationMessage> e) {
            VisualizationUpdated(sender, e);
        }
    }
}