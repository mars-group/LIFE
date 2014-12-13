using System;
using LifeAPI.Agent;
using LifeAPI.Layer;

namespace BugReproductionModel
{
	public class SuicideAgent: IAgent, IEquatable<SuicideAgent>
	{

		private int _lifeTime = 0;

		private int _tickNr = 0;

		private int _id;

		private ILayer _layer = null;

		private UnregisterAgent _unregister = null;

		public SuicideAgent (int identifier, ILayer layer, UnregisterAgent unregister) {
            ID = identifier;
			_id = identifier;
			_layer = layer;
			_unregister = unregister;

			var gen = new Random (DateTime.Now.Millisecond);

			_lifeTime = gen.Next (0, 40);
		}

		public void Tick()
		{
			_lifeTime--;
			_tickNr++;

			if (_lifeTime < 0) {
				_unregister.Invoke (_layer, this);

				Console.WriteLine ("Agent with equals & Co. " + _id + " commited suicide at tick " + _tickNr);
			}
		}

		public bool Equals(SuicideAgent other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (_id == other._id);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SuicideAgent)obj);
		}

		public override int GetHashCode() {
			return _id.GetHashCode();
		}

		public static bool operator ==(SuicideAgent left, SuicideAgent right) {
			return Equals(left, right);
		}

		public static bool operator !=(SuicideAgent left, SuicideAgent right) {
			return !Equals(left, right);
		}

	    public long ID { get; set; }
	}
}

