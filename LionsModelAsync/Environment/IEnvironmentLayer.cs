using System.Collections.Generic;
using LIFE.API.Layer;
using WolvesModel.Agents;

namespace WolvesModel.Environment {
  
  /// <summary>
  ///   Interface for the environment layer.
  /// </summary>
  public interface IEnvironmentLayer : ISteppedActiveLayer {
    

    /// <summary>
    ///   Returns a random free grid cell.
    /// </summary>
    /// <returns>the [x,y] coordinates or 'null', if none could be found.</returns>
    int[] GetFreeCell();


    /// <summary>
    ///   The environment's extent in X-direction.
    /// </summary>
    int DimensionX { get; }


    /// <summary>
    ///   Y-direction environment extent.
    /// </summary>
    int DimensionY { get; }


    /// <summary>
    ///   Find grass agents in the environment.
    /// </summary>
    /// <param name="posX">Explore base position (X).</param>
    /// <param name="posY">Explore base position (Y).</param>
    /// <param name="viewRange">Exploration range.</param>
    /// <returns>A list of grass agents found.</returns>
    IList<Grass> FindGrass(int posX, int posY, int viewRange);


    /// <summary>
    ///   Find sheep in the environment.
    /// </summary>
    /// <param name="posX">Explore base position (X).</param>
    /// <param name="posY">Explore base position (Y).</param>
    /// <param name="viewRange">Exploration range.</param>
    /// <returns>A list of sheep agents found.</returns>
    IList<Sheep> FindSheep(int posX, int posY, int viewRange);


    /// <summary>
    ///   Find wolves in the environment.
    /// </summary>
    /// <param name="posX">Explore base position (X).</param>
    /// <param name="posY">Explore base position (Y).</param>
    /// <param name="viewRange">Exploration range.</param>
    /// <returns>A list of wolves found.</returns>
    IList<Wolf> FindWolves(int posX, int posY, int viewRange);
  }
}