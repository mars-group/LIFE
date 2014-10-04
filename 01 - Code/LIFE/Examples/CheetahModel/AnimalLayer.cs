using System;
using LayerAPI.Interfaces;
using Mono.Addins;
using System.Xml.Linq;
using System.Collections.Generic;
using TwoDimEnvironment;


[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace CheetahModel
{
	[Extension(typeof (ISteppedLayer))]
	public class AnimalLayer :ISteppedLayer
	{
		private Impala _impala;
		private ITwoDimEnvironment<Animal> _env;
		const int numberMales = 2;
		const int NumberFemales = 4;

		public AnimalLayer ()
		{
			_env = new TwoDimEnvironmentUseCase<Animal> ();
			_impala = new Impala ();

		}

		public bool InitLayer<I>(I layerInitData, RegisterAgent registerAgentHandle,
			UnregisterAgent unregisterAgentHandle) {


			for (int i=0; i < numberMales;i++){
				var c = new Cheetah (3 + i, Animal.Gender.Male, 1, i+5, _env);
				registerAgentHandle.Invoke (this, c);
				_env.Add (c);
			}
			for (int i = 0; i < NumberFemales; i++) {
				var c = new Cheetah (1 + i, Animal.Gender.Female, i+5, 1, _env);
				registerAgentHandle.Invoke (this, c);
				_env.Add (c);
			}
			registerAgentHandle.Invoke (this, _impala);

			return true;
		}

		public long GetCurrentTick() {
			throw new NotImplementedException();
		}
			
	}
}