using LIFE.API.Environment.GeoCommon;

namespace LIFE.API.Layer.Obstacle
{
    public interface IObstacleLayer : ILayer
    {
        /// <summary>
        ///   Computes the accumulated path rating from start to destination.
        ///   An optional failFastThreshold may be provided to allow for a faster return
        ///   of the method in case that threshold is already hit by the raytracing.
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="destination">Destination position</param>
        /// <param name="failFastThreshold">The fail fast threshold (optional)</param>
        /// <returns>Accumulated path rating</returns>
        double GetAccumulatedPathRating(IGeoCoordinate start, IGeoCoordinate destination,
            double failFastThreshold = double.MaxValue);

        /// <summary>
        ///   Computes the accumulated path rating from the start position to the position
        ///   which results from a virtual movement according to speed and bearing parameters.
        ///   An optional failFastThreshold may be provided to allow for a faster return
        ///   of the method in case that threshold is already hit by the raytracing.
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="speed">The speed to move with</param>
        /// <param name="bearing">The bearing of the movement.</param>
        /// <param name="failFastThreshold">The fail fast threshold (optional)</param>
        /// <returns>Accumulated path rating</returns>
        double GetAccumulatedPathRating(IGeoCoordinate start, double speed, double bearing,
            double failFastThreshold = double.MaxValue);

        /// <summary>
        ///   Adds to a cell's rating value.
        /// </summary>
        /// <param name="position">The gps position used to obtain the cell.</param>
        /// <param name="ratingValue">The value to add to the rating.</param>
        void AddCellRating(IGeoCoordinate position, double ratingValue);

        /// <summary>
        ///   Reduces a cell's rating value.
        /// </summary>
        /// <param name="position">The gps position used to obtain the cell.</param>
        /// <param name="ratingValue">The value by which to reduce the cell rating.</param>
        void ReduceCellRating(IGeoCoordinate position, double ratingValue);

        /// <summary>
        /// Try to reduce a cell rating by a given value
        /// </summary>
        /// <param name="position">gps position to find the cell</param>
        /// <param name="amountToTake">value by which the cell rating should be reduced</param>
        /// <returns>the amount that was took or 0 if there isn't enough left</returns>
        double TryToReduceCellRating(IGeoCoordinate position, double amountToTake);

        /// <summary>
        ///   Set the rating of the cell at the given position.
        ///   Will overwrite any other rating in that cell.
        /// </summary>
        /// <param name="position">The gps position used to obtain the cell.</param>
        /// <param name="cellValue">The new value for the cell</param>
        void SetCellRating(IGeoCoordinate position, double cellValue);

        /// <summary>
        /// Get the cell rating for a geo coordinate
        /// </summary>
        /// <param name="position">the geo coordinate</param>
        /// <returns>the value of the corresponding cell</returns>
        double GetCellRating(IGeoCoordinate position);

        /// <summary>
        /// Get the gps positon of the the adjacent cell with the highest value
        /// </summary>
        /// <param name="positon">gps position where the search starts</param>
        /// <returns>position of the cell with the highest value</returns>
        IGeoCoordinate GetNeighbourCellWithMaxValue(IGeoCoordinate positon);
    }
}