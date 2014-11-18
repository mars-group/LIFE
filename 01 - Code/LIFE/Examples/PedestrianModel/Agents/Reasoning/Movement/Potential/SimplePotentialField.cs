using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Movement;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
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
                //return Collections.unmodifiableSet(potentialEmitters);
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