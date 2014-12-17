using System;
using System.Collections.Generic;
using DalskiAgent.Environments;
using EnvironmentServiceComponent.Implementation;
using LifeAPI.Spatial;
using LifeAPI.Perception;
using ISpatialObject = DalskiAgent.Environments.ISpatialObject;

namespace WolvesModel.Environment {
  
  /// <summary>
  ///   This grassland is home to sheeps and wolves ... and yes, 'grass'.
  ///   It serves two purposes here: Environment representation and data source.
  /// </summary>
  class Grassland : IEnvironmentOld, IDataSource {

    private readonly IEnvironmentOld _env;   // Environment implementation.
    private readonly Vector _boundaries;  // Positive extent of the environment.
    private readonly Random _random;      // Random number generator.

    /// <summary>
    ///   Create a new grassland.
    /// </summary>
    /// <param name="boundaries">Positive extent of the environment.</param>
    /// <param name="isGrid">Shall a grid be used?</param>
    public Grassland(Vector boundaries, bool isGrid) {
      _random = new Random();
      _boundaries = boundaries;
      _env = new ESCAdapter(new GeometryESC(), boundaries, isGrid);  
    }


    /// <summary>
    ///   Retrieve information from a data source.
    ///   Overrides GetData to provide additional "Grass" agent queries.
    /// </summary>
    /// <param name="spec">Information object describing which data to query.</param>
    /// <returns>An object representing the percepted information.</returns>
    public object GetData(ISpecification spec) {
      return ((ESCAdapter) _env).GetData(spec);
    }


    /// <summary>
    ///   Returns a random, free position (for random agent movement).
    ///   ATTENTION: This function is only supported for the Env2D version!
    /// </summary>
    /// <returns>A vector representing a free position.</returns>
    public Vector GetRandomPosition() {
      var x = _random.Next((int)_boundaries.X);
      var y = _random.Next((int)_boundaries.Y);     
      return new Vector(x, y);
    }



    /* Redirection of environment functions to concrete implementation. */
    /********************************************************************/

    public void AddObject(ISpatialObject obj, CollisionType collisionType, Vector pos, out DataAccessor acc, Vector dim, Direction dir) {
      _env.AddObject(obj,collisionType, pos, out acc, dim, dir);
    }

    public void RemoveObject(ISpatialObject obj) {
      _env.RemoveObject(obj);
    }

    public MovementResult MoveObject(ISpatialObject obj, Vector movement, Direction dir = null) {
      return _env.MoveObject(obj, movement, dir);
    }

    public List<ISpatialObject> GetAllObjects() {
      return _env.GetAllObjects();
    }

    public void AdvanceEnvironment() {
      _env.AdvanceEnvironment();
    }
  }
}
