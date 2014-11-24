using System.Collections.Generic;
using GenericAgentArchitectureCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential {

    public interface IPotentialField {
        /// <summary>
        ///     Returns all potential emitters of this potential field.
        /// </summary>
        /// <returns> a collection of all potential emitters of this potential field </returns>
        ICollection<IPotentialEmitter> Emitters { get; }

        /// <summary>
        ///     Adds an emitter to the field.
        /// </summary>
        /// <param name="emitter"> the emitter to add </param>
        void AddEmitter(IPotentialEmitter emitter);

        /// <summary>
        ///     Clears the whole potential field.
        /// </summary>
        void ClearAll();

        /// <summary>
        ///     Calculates the potential of this potential field at the position <code>position</code> by adding the
        ///     potentials of all <seealso cref="IPotentialEmitter" />s at this position.
        /// </summary>
        /// <param name="position"> the position of the potential </param>
        /// <returns> the potential at this position </returns>
        double CalculatePotential(Vector position);
    }

}