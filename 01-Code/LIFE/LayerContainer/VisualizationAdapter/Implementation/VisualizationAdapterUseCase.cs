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
using System.Threading.Tasks;
using VisualizationAdapter.Interface;
using LifeAPI.Layer.Visualization;
using LifeAPI.Layer;

namespace VisualizationAdapter.Implementation {
	internal class VisualizationAdapterUseCase : IVisualizationAdapter {

		/// <summary>
		/// The SimulationID. 
		/// It will be set before the first call to VisualizeTick().
		/// </summary>
		/// <value>The simulation identifier.</value>
		public Guid SimulationId { get; set; }

		private readonly ConcurrentDictionary<ILayer, ConcurrentDictionary<IVisualizableAgent, byte>> _visualizablesPerLayer;

		public VisualizationAdapterUseCase() {
			_visualizablesPerLayer = new ConcurrentDictionary<ILayer, ConcurrentDictionary<IVisualizableAgent, byte>>();
        }

        #region IVisualizationAdapterInternal Members

        public void VisualizeTick(int currentTick) {

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

        #endregion
    }
}