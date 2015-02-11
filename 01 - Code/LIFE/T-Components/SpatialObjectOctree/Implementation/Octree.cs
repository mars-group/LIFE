using System;
using System.Collections.Generic;
using System.Linq;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;
using SpatialObjectOctree.Interface;

namespace SpatialObjectOctree.Implementation {

    /// <summary>
    ///     Yet another quad tree.
    /// </summary>
    public class Octree<T> : IOctree<T> where T : class, ISpatialObject {
        private const int MaxObjects = 4; // Maximum object count before node is split.
        private readonly Octree<T>[] _childNodes; // Sub nodes (if quadtree is segmented).  
    /*  ┌─────┬─────┐  Default
     *  │ II. │  I. │  quadrant
     *  ├─────┼─────┤  enumeration
     *  | III.│ IV. │  (counter clockwise).
     *  └─────┴─────┘  Array: 0-3.
     */

        private readonly int _level; // Nested depth of quadtree node.
        private readonly object _lock = new object();
        private readonly List<T> _objects; // List of contained objects.
        private readonly Vector3 _position; // Beginning position of this node.
        private readonly Vector3 _span; // The extent (width and height).

        /// <summary>
        ///     Create a new quadtree.
        /// </summary>
        /// <param name="level">Nested depth of quadtree node.</param>
        /// <param name="pos">Beginning position of this node.</param>
        /// <param name="span">The extent (width and height).</param>
        public Octree(int level, Vector3 pos, Vector3 span) {
            _level = level;
            _position = pos;
            _span = span;
            _objects = new List<T>();
            _childNodes = new Octree<T>[4];
        }

        /// <summary>
        ///     Clears the quadtree. Deletes all stored objects
        ///     and clears recursively all child nodes.
        /// </summary>
        public void Clear() {
            _objects.Clear();
            for (int i = 0; i < 4; i++) {
                if (HasChildNodes()) {
                    _childNodes[i].Clear();
                    _childNodes[i] = null;
                }
            }
        }

        /// <summary>
        ///     Splits this node into four sub nodes.
        /// </summary>
        private void Split() {
            Vector3 newSize = new Vector3(_span.X/2, _span.Y/2);
            double x = _position.X;
            double y = _position.Y;
            _childNodes[0] = new Octree<T>(_level + 1, new Vector3(x + newSize.X, y + newSize.Y), newSize);
            _childNodes[1] = new Octree<T>(_level + 1, new Vector3(x, y + newSize.Y), newSize);
            _childNodes[2] = new Octree<T>(_level + 1, new Vector3(x, y), newSize);
            _childNodes[3] = new Octree<T>(_level + 1, new Vector3(x + newSize.X, y), newSize);
        }

        /// <summary>
        ///     Determine to which node a new object belongs to.
        /// </summary>
        /// <param name="pos">The position of the object to insert.</param>
        /// <param name="span">The object's width and height.</param>
        /// <returns>The subtree index or -1, if the object stays in the parent node.</returns>
        private int GetIndex(Vector3 pos, Vector3 span) {
            int index = -1;

            // Check, if object completely fits in top/bottom row. Abort, if not! 
            Vector3 center = new Vector3(_position.X + _span.X/2, _position.Y + _span.Y/2);
            bool fitsBottom = (pos.Y + span.Y <= center.Y);
            bool fitsTop = (pos.Y >= center.Y);
            if (!fitsTop && !fitsBottom) {
                return -1;
            }

            // Object can completely fit within the left quadrants.
            if (pos.X + span.X <= center.X) {
                index = fitsTop ? 1 : 2;
            }

            // Object can completely fit within the right quadrants.
            else if (pos.X >= center.X) {
                index = fitsTop ? 0 : 3;
            }

            return index;
        }

        /// <summary>
        ///     Inserts an object into the quadtree.
        /// </summary>
        /// <param name="obj"></param>
        public void Insert(T obj) {
            lock (_lock)
            {
                var shape = obj.Shape;
                Vector3 pos = shape.Position;
                Vector3 span = shape.Bounds.Dimension;

                if (HasChildNodes()) { // If this node has subnodes ...
                    int index = GetIndex(pos, span); // find the right node ...
                    if (index != -1) { // and if found ...
                        _childNodes[index].Insert(obj); // insert it there!
                        return;
                    }
                }

                // The object has to be inserted here.
                _objects.Add(obj);
                if (_objects.Count > MaxObjects) {
                    if (!HasChildNodes()) {
                        Split();
                    }

                    int i = 0;
                    while (i < _objects.Count) {
                        var shape2 = _objects[i].Shape;
                        int index = GetIndex(shape2.Position, shape2.Bounds.Dimension);
                        if (index != -1) {
                            T tmp = _objects[i];
                            _objects.Remove(tmp);
                            _childNodes[index].Insert(tmp);
                        }
                        else {
                            i ++; // We can't move this object to a sub node.
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Removes an object from to quadtree.
        /// </summary>
        /// <param name="obj">The object to remove.</param>
        /// <returns>'True' on successful deletion, 'false', if object was not found.</returns>
        public void Remove(T obj) {
            lock (_lock)
            {
                var shape = obj.Shape;
                Vector3 pos = shape.Position;
                Vector3 span = shape.Bounds.Dimension;

                int index = GetIndex(pos, span); // Determine quadrant.
                if (index == -1 || !HasChildNodes()) {
                    _objects.Remove(obj); // Object is on this level.
                }
                 _childNodes[index].Remove(obj); // Else go down recursively.
            }
        }

        public List<T> Query(BoundingBox bounds) {
            return Retrieve(new List<T>(), bounds.Position, bounds.Dimension);
        }

        public List<T> GetAll() {
            return new List<T>(_objects); 
        }

        /// <summary>
        ///     Helper function to determine if this node has subnodes or not.
        /// </summary>
        /// <returns>'True', if node is further splitted, 'false', if it is a leaf.</returns>
        private bool HasChildNodes() {
            return (_childNodes[0] != null);
        }

        /// <summary>
        ///     Returns all objects that could collide with the given object.
        /// </summary>
        /// <param name="retList">Return list (for recursive additions).</param>
        /// <param name="pos">Position of the given object.</param>
        /// <param name="span">Its width and height.</param>
        /// <returns></returns>
        public List<T> Retrieve(List<T> retList, Vector3 pos, Vector3 span) {
            // Intersect query area with current scope [collision detection]. Skip, if it's outside!
            if (!CDF.IntersectRects(_position, _span, pos, span)) {
                return retList;
            }
            int index = GetIndex(pos, span);

            lock (_lock) {
                // If this node is responsible or has no child nodes, get all contained objects.
                if (index == -1 || !HasChildNodes()) {
                    for (int i = 0; i < _objects.Count; i ++) {
                        if (CDF.IntersectRects
                            (
                                new Vector3
                                    ((double) _objects[i].Shape.Position.X, (double) _objects[i].Shape.Position.Y),
                                new Vector3
                                    ((double) _objects[i].Shape.Bounds.Width, (double) _objects[i].Shape.Bounds.Height),
                                pos,
                                span)) {
                            retList.Add(_objects[i]);
                        }
                    }
                }

                // If query area is [also] managed by a child node, redirect call.
                if (HasChildNodes()) {
                    for (int i = 0; i < 4; i ++) {
                        _childNodes[i].Retrieve(retList, pos, span);
                    }
                }

                return retList;
            }
        }

        /// <summary>
        ///     Utility class with various collision detection functions.
        /// </summary>
        public static class CDF {
            /// <summary>
            ///     Function to check the intersection of two rectangles.
            /// </summary>
            /// <param name="pos1">Reference point (bottom,left) of rectangle 1.</param>
            /// <param name="span1">Dimension (extent) of the first rectangle.</param>
            /// <param name="pos2">Reference point (bottom,left) of rectangle 2.</param>
            /// <param name="span2">Dimension (extent) of the second rectangle.</param>
            /// <returns>'True', if the rectangles intersect or one contains the other.</returns>
            public static bool IntersectRects(Vector3 pos1, Vector3 span1, Vector3 pos2, Vector3 span2) {
                return IntervalsCollide(new Vector3(pos1.X, pos1.X + span1.X), new Vector3(pos2.X, pos2.X + span2.X)) &&
                       IntervalsCollide(new Vector3(pos1.Y, pos1.Y + span1.Y), new Vector3(pos2.Y, pos2.Y + span2.Y));
            }

            /// <summary>
            ///     Checks, if two intervals collide.
            /// </summary>
            /// <param name="intv1">First interval.</param>
            /// <param name="intv2">Second interval.</param>
            /// <returns>'True', if the intervals overlap, touch (!) or one contains the other.</returns>
            public static bool IntervalsCollide(Vector3 intv1, Vector3 intv2) {
                return !(intv2.X >= intv1.Y || intv1.X >= intv2.Y);
            }
        };
    }

}