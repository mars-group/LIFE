using LayerAPI.Spatial;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential {

    public interface IPotentialEmitter {
        /// <summary>
        ///     Calculates the potential if this emitter at the given position <code>position</code> ant returns it.
        /// </summary>
        /// <param name="reference"> the position to calculate the potential for. </param>
        /// <returns> the emitted potential of this emitter at the given position. </returns>
        double GetPotential(Vector reference);
    }

}