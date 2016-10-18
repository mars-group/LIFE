namespace LifeAPI.Layer.PotentialField
{
    public interface IGridPotentialFieldLayer : ILayer
    {
        /// <summary>
        /// Searches for the closest cell with full potential (with breadth first search).
        /// This method should be used if the current cell has no potential and you want to search with endless sight.
        /// Normally #ExploreClosestFullPotentialField should be used, because the potential propagation represents
        /// an agents sight range.
        /// </summary>
        /// <param name="cell">current cell</param>
        /// <returns>Cell of the closest cell with full potential</returns>
        int ExploreClosestWithEndlessSight(int cell);

        /// <summary>
        /// Searches for the closest cell with full potential.
        /// </summary>
        /// <param name="cell">current cell</param>
        /// <returns>Cell of the closest field with full potential or null if the current field has no potential</returns>
        int ExploreClosestFullPotentialField(int cell);

        /// <summary>
        /// Returns if the current cell has full potential.
        /// </summary>
        /// <param name="cell">current cell</param>
        /// <returns>If the current cell has full potential</returns>
        bool HasFullPotential(int cell);
    }
}