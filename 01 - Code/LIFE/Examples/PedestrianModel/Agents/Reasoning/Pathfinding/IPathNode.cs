namespace PedestrianModel.Agents.Reasoning.Pathfinding {

    public interface IPathNode<TE> {
        /// <summary>
        ///     Returns the adapted object of this node.
        /// </summary>
        /// <returns> the adapted object </returns>
        TE AdaptedObject { get; }

        /// <summary>
        ///     returns the assigned predecessor node of this node. The predecessor node is used to create the entire
        ///     path.
        /// </summary>
        /// <returns> the predecessor of this node </returns>
        IPathNode<TE> Predecessor { get; set; }


        /// <summary>
        ///     Returns true if <code>o</code> is a Node object with equal adapted objects.
        /// </summary>
        /// <param name="o"> the object to compare </param>
        /// <returns> true if the object o is equal to this node </returns>
        bool Equals(object o);

        int GetHashCode();
    }

}