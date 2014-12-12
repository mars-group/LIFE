using System;
using LayerAPI.Agent;
using LayerAPI.Layer;

namespace BugReproductionModel
{
	/// <summary>
	/// A breed agent breeds NUM_OF_CHILDS each TICK_PERIOD ticks
	/// </summary>
	public class BreedAgent: IAgent
	{

		private const int NUM_OF_CHILDS = 1000;

		private const int TICK_PERIOD = 10;

		private Guid AgentID;

		private int _tickCount = 0;

		private RegisterAgent _registerHandle = null;

		private UnregisterAgent _unregisterHandle = null;

		private ILayer _layer = null;

		public BreedAgent (ILayer layer, RegisterAgent registerHandle, UnregisterAgent unregister)
		{
			AgentID = Guid.NewGuid ();

			_tickCount = 0;

			_registerHandle = registerHandle;
			_unregisterHandle = unregister;
			_layer = layer;
		}

		public void Tick() 
		{
			if (_tickCount % TICK_PERIOD == 0) {
				for (int i = 0; i < NUM_OF_CHILDS; i++) {
					_registerHandle.Invoke (_layer, new SuicideAgent(2000 + 
						(_tickCount / TICK_PERIOD) * NUM_OF_CHILDS + i, _layer, _unregisterHandle));
				}

				Console.WriteLine("Agent " + AgentID + " breed " + NUM_OF_CHILDS + " suicide agents");
			}

			_tickCount++;
		}


		public bool Equals(BreedAgent other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (other.AgentID == AgentID);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BreedAgent)obj);
		}

		public override int GetHashCode() {
			return AgentID.GetHashCode();
		}

		public static bool operator ==(BreedAgent left, BreedAgent right) {
			return Equals(left, right);
		}

		public static bool operator !=(BreedAgent left, BreedAgent right) {
			return !Equals(left, right);
		}
	}
}

