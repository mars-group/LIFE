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
using LifeAPI.Layer;

namespace VisualizationAdapter.Implementation {
	public class VisualizationAdapterComponent : IVisualizationAdapter {
        private readonly IVisualizationAdapter _visualizationAdapterInternalUseCase;

		public Guid SimulationId {
			get {
				return _visualizationAdapterInternalUseCase.SimulationId;
			}
			set {
				_visualizationAdapterInternalUseCase.SimulationId = value;
			}
		}

        public VisualizationAdapterComponent() {
            _visualizationAdapterInternalUseCase = new VisualizationAdapterUseCase();
        }

        #region IVisualizationAdapterInternal Members

        public event EventHandler<List<BasicVisualizationMessage>> VisualizationUpdated;

		public void RegisterVisualizable(ILayer layer, IVisualizableAgent visAgent) {
			_visualizationAdapterInternalUseCase.RegisterVisualizable(layer, visAgent);
        }

        public void VisualizeTick(int currentTick) {
            _visualizationAdapterInternalUseCase.VisualizeTick(currentTick);
        }

		public void DeRegisterVisualizable(ILayer layer, IVisualizableAgent visAgent)
        {
			_visualizationAdapterInternalUseCase.DeRegisterVisualizable(layer,visAgent);
        }

        #endregion

    }
}