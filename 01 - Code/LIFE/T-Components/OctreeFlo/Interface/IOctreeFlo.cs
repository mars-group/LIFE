using System.Collections.Generic;
using SpatialCommon.Shape;

namespace OctreeFlo.Interface {
    /// <summary>
    /// Implements an unbalanced, 3-dimensonal tree.
    /// </summary>
    /// <remarks>
    /// The tree's nodes are cubic subdivions of the complete space encompassed by the tree.<br/>
    /// The root node owns all the space. It may have up to eight children, each one the same size.<br/>
    /// This repeats recursively down to the leaf nodes. One leaf node contains up to a maximum number of items.<br/>
    /// The described structure allows for all operations to need only computing time O(log(n)) with n being the <br/>
    /// number of items within the tree.
    /// </remarks>
    public interface IOctreeFlo<T> where T : class, IShape 
    {

        void Insert(T shape);
        void Remove(T shape);

        List<T> Query(BoundingBox bounds);

        int GetShapeCount();
    }

}