//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisualizationAdapter.Interface;
using LifeAPI.Layer.Visualization;
using LifeAPI.Layer;

namespace VisualizationAdapter.Implementation {
	internal class VisualizationAdapterUseCase : IVisualizationAdapterInternal {
		private readonly ConcurrentDictionary<ILayer, ConcurrentDictionary<IVisualizableAgent, byte>> _visualizablesPerLayer;
        private bool _isRunning;
        private int? _tickVisualizationIntervall;

        public VisualizationAdapterUseCase() {
			_visualizablesPerLayer = new ConcurrentDictionary<ILayer, ConcurrentDictionary<IVisualizableAgent, byte>>();
            _isRunning = false;
        }

        #region IVisualizationAdapterInternal Members

        public void VisualizeTick(int currentTick) {
            if (!_isRunning) return;
            if (_tickVisualizationIntervall.HasValue && currentTick%_tickVisualizationIntervall != 0) return;


            Parallel.ForEach
                (_visualizablesPerLayer,
                    vis => {
					// store layer name
					var layerName = vis.Key.GetType().Name;
					Parallel.ForEach(vis.Value.Keys, _visAgent => {
						var jsonStrig =_visAgent.GetVisualizationJson();
						// Send via Queue if possible
						// TODO : Send to Queue
						// DoSend(buildPackage(layer,jsonString));
					});

                });
        }

		public void RegisterVisualizable (ILayer layer, IVisualizableAgent visAgent)
		{
			ConcurrentDictionary<IVisualizableAgent, byte> agentList;
			if (_visualizablesPerLayer.TryGetValue (layer, out agentList)) {
				agentList.TryAdd(visAgent, new byte());
			} else {
				agentList = new ConcurrentDictionary<IVisualizableAgent, byte>();
				agentList.TryAdd(visAgent, new byte());
				_visualizablesPerLayer.TryAdd (layer, agentList);
			}
		}

		public void DeRegisterVisualizable (ILayer layer, IVisualizableAgent visAgent)
		{
			ConcurrentDictionary<IVisualizableAgent, byte> agentBag;
			if (_visualizablesPerLayer.TryGetValue (layer, out agentBag)) {
				byte bla;
				agentBag.TryRemove(visAgent, out bla);
			}
		}

        public void StartVisualization(int? nrOfTicksToVisualize = null) {
            _tickVisualizationIntervall = nrOfTicksToVisualize;
            _isRunning = true;
        }

        public void StopVisualization() {
            _isRunning = false;
        }

        public void ChangeVisualizationView(double topLeft, double topRight, double bottomLeft, double bottomRight) {
            throw new NotImplementedException();
        }

        #endregion
    }
}