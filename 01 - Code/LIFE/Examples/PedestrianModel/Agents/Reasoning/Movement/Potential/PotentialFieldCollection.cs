using System;
using System.Collections.Generic;
using GenericAgentArchitectureCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential {

    public class PotentialFieldCollection : HashSet<IPotentialField>, IPotentialField {
        /// <summary>
        ///     Constructs a new empty potential field collection.
        /// </summary>
        public PotentialFieldCollection() {}

        /// <summary>
        ///     Constructs a new potential field collection using the given collection as initial elements.
        /// </summary>
        /// <param name="collection"> the collection whose elements are to be placed into this set </param>
        public PotentialFieldCollection(ICollection<IPotentialField> collection) : base(collection) {}

        #region IPotentialField Members

        /// <summary>
        ///     Calculates the potential for all fields for the given position and returns the sum of it.
        /// </summary>
        /// <param name="position"> the position to calculate the potential for </param>
        /// <returns> the sum of all potentials </returns>
        public double CalculatePotential(Vector position) {
            double potentialSum = 0.0;

            foreach (IPotentialField field in this) {
                potentialSum += field.CalculatePotential(position);
            }

            return potentialSum;
        }

        public void AddEmitter(IPotentialEmitter emitter) {
            throw new NotImplementedException();
        }

        public ICollection<IPotentialEmitter> Emitters {
            get {
                throw new NotImplementedException();
            }
        }

        public void ClearAll() {
            Clear();
        }

        #endregion
    }

}