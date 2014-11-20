using System;
using LayerAPI.Interfaces;
using Mono.Addins;
using ElephantLayer.Agents;
using System.Windows;
using WaterLayer;
using PlantLayer;
using TwoDimEnvironment;
using System.Collections.Generic;
using ElephantLayer.TransportTypes;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace ElephantLayer
{
	[Extension(typeof (ISteppedLayer))]
    public class ElephantLayerImpl : ISteppedLayer
    {
		private const int _ELEPHANT_COUNT = 10;

		private PlantLayerImpl _plantLayer;
		private WaterLayerImpl _waterLayer;
		private ITwoDimEnvironment<Elephant> _environment;
	    private long _currentTick;

	    public ElephantLayerImpl(PlantLayerImpl plantLayer, WaterLayerImpl waterLayer){
			_plantLayer = plantLayer;
			_waterLayer = waterLayer;
			_environment = new TwoDimEnvironmentUseCase<Elephant> ();
		}

        public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {

			for(int i = 0; i < _ELEPHANT_COUNT; i++){
				var e = new Elephant(i*10,0, new Size(){ Width = 1.0, Height = 1.0}, -90.0, 60, 0.5, _plantLayer, _waterLayer, _environment);
				registerAgentHandle.Invoke (this, e);
				_environment.Add(e);
			}

			return true;
        }
        public long GetCurrentTick()
        {
            return _currentTick;
        }
        public void SetCurrentTick(long currentTick)
        {
            this._currentTick = currentTick;
        }

		public List<TElephant> GetAllElephants(){
			var allElephants = _environment.GetAll();
			var result = new List<TElephant> ();
			allElephants.ForEach (e => result.Add(new TElephant(e)));
			return result;
		}
    }
}
