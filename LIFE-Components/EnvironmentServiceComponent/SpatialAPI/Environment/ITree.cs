using System.Collections.Generic;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialAPI.Environment {
  
  /// <summary>
  ///   Describes a tree with its basic operations.
  /// </summary>
  /// <typeparam name="T">are the elements that are stored in the tree</typeparam>
  public interface ITree<T> where T : class, ISpatialObject {
  
    /// <summary>
    ///   Inserts given entity to tree.
    /// </summary>
    /// <param name="entity">that is added</param>
    void Insert(T entity);

    /// <summary>
    ///   Removes given entity from the tree.
    /// </summary>
    /// <param name="entity">that is removed</param>
    void Remove(T entity);

    /// <summary>
    ///   Finds a list of <code>T</code> that are intersecting with given shape.
    /// </summary>
    /// <param name="shape">defines, which entities are returned.</param>
    /// <returns>A list of entities that correspond with shape.</returns>
    List<T> Query(BoundingBox shape);

    /// <summary>
    ///   Query the specified sphere. Optimized query for spheres
    /// </summary>
    /// <param name="sphere">Sphere.</param>
    List<T> Query(Sphere sphere, int maxResults = -1);

    /// <summary>
    ///   Provides a list of all entities that are stored in the tree.
    /// </summary>
    /// <returns>The complete list of entities.</returns>
    List<T> GetAll();
  }
}