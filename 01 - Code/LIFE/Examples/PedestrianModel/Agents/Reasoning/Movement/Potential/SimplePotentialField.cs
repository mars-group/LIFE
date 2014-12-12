using System.Collections.Generic;
using System.Linq;
using LifeAPI.Spatial;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential {

    public class SimplePotentialField : IPotentialField {
        /// <summary>
        ///     The collection of potential emitters.
        /// </summary>
        private readonly HashSet<IPotentialEmitter> _potentialEmitters = new HashSet<IPotentialEmitter>();

        #region IPotentialField Members

        public void AddEmitter(IPotentialEmitter emitter) {
            _potentialEmitters.Add(emitter);
        }

        public ICollection<IPotentialEmitter> Emitters {
            get {
                return _potentialEmitters.ToList().AsReadOnly();
            }
        }

        public void ClearAll() {
            _potentialEmitters.Clear();
        }

        public double CalculatePotential(Vector position) {
            double result = 0.0;

            foreach (IPotentialEmitter emitter in _potentialEmitters) {
                result += emitter.GetPotential(position);
            }

            return result;
        }

        #endregion
    }

}