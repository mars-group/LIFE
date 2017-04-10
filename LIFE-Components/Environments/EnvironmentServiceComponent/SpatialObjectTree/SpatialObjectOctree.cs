using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.SpatialObjectTree
{
    /// <summary>
    ///   Implements an unbalanced, 3-dimensonal tree.
    /// </summary>
    /// <remarks>
    ///   The tree's nodes are cubic subdivions of the complete space encompassed by the tree.<br />
    ///   The root node owns all the space. It may have up to eight children, each one the same size.<br />
    ///   This repeats recursively down to the leaf nodes. One leaf node contains up to a maximum number of items.<br />
    ///   The described structure allows for all operations to need only computing time O(log(n)) with n being the <br />
    ///   number of items within the tree.
    /// </remarks>
    public class SpatialObjectOctree<T> : ITree<T> where T : class, ISpatialObject
    {
        public enum Direction
        {
            Nwb = 0, //NORTH-WEST-BOTTOM
            Neb = 1,
            Swb = 2,
            Seb = 3,
            Nwt = 4,
            Net = 5,
            Swt = 6,
            Set = 7 //SOUTH-EAST-TOP
        }

        private readonly int _maxObjectsPerLeaf;
        private readonly Vector3 _minLeafSize;
        private readonly ConcurrentDictionary<T, Ocnode> _objectToNodeLookup = new ConcurrentDictionary<T, Ocnode>();
        private readonly object _syncLock = new object();


        /// <summary>
        /// </summary>
        /// <param name="minLeafSize">The smallest size a leaf will split into</param>
        /// <param name="maxObjectsPerLeaf">Maximum number of objects per leaf before it forces a split into sub quadrants</param>
        public SpatialObjectOctree(Vector3 minLeafSize, int maxObjectsPerLeaf)
        {
            Root = null;
            _minLeafSize = minLeafSize;
            _maxObjectsPerLeaf = maxObjectsPerLeaf;
        }


        private Ocnode Root { get; set; }

        #region NestedType Ocnode

        public class Ocnode
        {
            private readonly Mutex _mutex;
            private readonly Ocnode[] _nodes = new Ocnode[8];
            public readonly ReadOnlyCollection<Ocnode> Nodes;
            public readonly ReadOnlyCollection<T> Objects;
            internal readonly List<T> Shapes = new List<T>();


            public Ocnode(BoundingBox bounds)
            {
                _mutex = new Mutex();
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<Ocnode>(_nodes);
                Objects = new ReadOnlyCollection<T>(Shapes);
            }


            public Ocnode(double x, double y, double z, Vector3 dimension)
                : this(
                    BoundingBox.GenerateByDimension
                    (
                        new Vector3(x + dimension.X / 2, y + dimension.Y / 2, z + dimension.Z / 2),
                        new Vector3(dimension.X, dimension.Y, dimension.Z)))
            {
            }

            public Ocnode Parent { get; internal set; }

            public Ocnode this[Direction direction]
            {
                get
                {
                    switch (direction)
                    {
                        case Direction.Nwt:
                            return _nodes[0];
                        case Direction.Net:
                            return _nodes[1];
                        case Direction.Swt:
                            return _nodes[2];
                        case Direction.Set:
                            return _nodes[3];
                        case Direction.Nwb:
                            return _nodes[4];
                        case Direction.Neb:
                            return _nodes[5];
                        case Direction.Swb:
                            return _nodes[6];
                        case Direction.Seb:
                            return _nodes[7];
                        default:
                            return null;
                    }
                }
                set
                {
                    switch (direction)
                    {
                        case Direction.Nwt:
                            _nodes[0] = value;
                            break;
                        case Direction.Net:
                            _nodes[1] = value;
                            break;
                        case Direction.Swt:
                            _nodes[2] = value;
                            break;
                        case Direction.Set:
                            _nodes[3] = value;
                            break;
                        case Direction.Nwb:
                            _nodes[4] = value;
                            break;
                        case Direction.Neb:
                            _nodes[5] = value;
                            break;
                        case Direction.Swb:
                            _nodes[6] = value;
                            break;
                        case Direction.Seb:
                            _nodes[7] = value;
                            break;
                    }
                    if (value != null)
                    {
                        value.Parent = this;
                    }
                }
            }

            public BoundingBox Bounds { get; private set; }

            public void GetAccess()
            {
                _mutex.WaitOne();
            }

            public void ReleaseAccess()
            {
                _mutex.ReleaseMutex();
            }

            public bool HasChildNodes()
            {
                return _nodes[0] != null;
            }
        }

        #endregion

        #region ITree<T> Members

        public void Insert(T spatialObject)
        {
            var bounds = spatialObject.Shape.Bounds;

            // still make use of lock here, since that's the only point where really only one thread may access
            lock (_syncLock)
            {
                // check if this is the first item to be added to the tree
                if (Root == null)
                {
                    Root =
                        new Ocnode
                        (BoundingBox.GenerateByDimension
                        (bounds.Position,
                            _minLeafSize));
                }
            }

            // expand Octree if new entity is out of current bounds
            while (!Root.Bounds.Contains(bounds))
            {
                ExpandRoot(bounds);
            }

            InsertNodeObject(Root, spatialObject);
        }

        public List<T> Query(BoundingBox shape)
        {
            var results = new List<T>();
            if (Root != null)
            {
                Query(shape, Root, results);
            }
            return results;
        }

        public void Remove(T spatialObject)
        {
            if (!_objectToNodeLookup.ContainsKey(spatialObject))
            {
                // object not found, so return. Why is that an error condition?
                return;
            }

            var containingNode = _objectToNodeLookup[spatialObject];

            containingNode.GetAccess();
            RemoveSpatialObjectFromNode(spatialObject);

            containingNode.ReleaseAccess();
        }

        public List<T> GetAll()
        {
            return _objectToNodeLookup.Keys.ToList();
        }

        #endregion

        #region Private Methods

        private void Query(BoundingBox shape, Ocnode node, List<T> results)
        {
            if (node == null)
            {
                return;
            }
            bool alreadyReleased = false;
            node.GetAccess();

            if (shape.IntersectsWith((IShape) node.Bounds))
            {
                // copy objects to intermediate array because node's objects might be altered in the meantime
                var objects = new T[node.Objects.Count];
                node.Objects.CopyTo(objects, 0);


                foreach (var spatialObject in objects)
                {
                    if (shape.IntersectsWith((IShape) spatialObject.Shape.Bounds))
                    {
                        results.Add(spatialObject);
                    }
                }

                var nodes = node.Nodes;
                node.ReleaseAccess();
                alreadyReleased = true;
                foreach (var childNode in nodes)
                {
                    Query(shape, childNode, results);
                }
            }

            if (!alreadyReleased)
            {
                node.ReleaseAccess();
            }
        }


        private void ExpandRoot(BoundingBox newChildBounds)
        {
            lock (_syncLock)
            {
                bool isWest = Root.Bounds.LeftBottomFront.X < newChildBounds.LeftBottomFront.X;
                bool isNorth = Root.Bounds.LeftBottomFront.Y < newChildBounds.LeftBottomFront.Y;
                bool isBottom = Root.Bounds.LeftBottomFront.Z > newChildBounds.LeftBottomFront.Z;

                Direction rootDirection;
                if (isBottom)
                {
                    if (isNorth)
                    {
                        rootDirection = isWest ? Direction.Nwb : Direction.Neb;
                    }
                    else
                    {
                        rootDirection = isWest ? Direction.Swb : Direction.Seb;
                    }
                }
                else
                {
                    if (isNorth)
                    {
                        rootDirection = isWest ? Direction.Nwt : Direction.Net;
                    }
                    else
                    {
                        rootDirection = isWest ? Direction.Swt : Direction.Set;
                    }
                }

                double newX = (rootDirection == Direction.Nwb || rootDirection == Direction.Swb ||
                               rootDirection == Direction.Nwt || rootDirection == Direction.Swt)
                    ? Root.Bounds.LeftBottomFront.X
                    : Root.Bounds.LeftBottomFront.X - Root.Bounds.Width;
                double newY = (rootDirection == Direction.Nwb || rootDirection == Direction.Neb ||
                               rootDirection == Direction.Nwt || rootDirection == Direction.Net)
                    ? Root.Bounds.LeftBottomFront.Y
                    : Root.Bounds.LeftBottomFront.Y - Root.Bounds.Height;
                double newZ = (rootDirection == Direction.Nwt || rootDirection == Direction.Net ||
                               rootDirection == Direction.Swt || rootDirection == Direction.Set)
                    ? Root.Bounds.LeftBottomFront.Z
                    : Root.Bounds.LeftBottomFront.Z - Root.Bounds.Length;

                BoundingBox newRootBounds = BoundingBox.GenerateByDimension
                (
                    new Vector3(newX + Root.Bounds.Width, newY + Root.Bounds.Height, newZ + Root.Bounds.Length),
                    new Vector3(Root.Bounds.Width * 2, Root.Bounds.Height * 2, Root.Bounds.Length * 2));
                Ocnode newRoot = new Ocnode(newRootBounds);
                SetupChildNodes(newRoot);
                newRoot[rootDirection] = Root;

                Root = newRoot;
            }
        }

        private void InsertNodeObject(Ocnode node, T spatialObject)
        {
            if (!node.Bounds.Contains(spatialObject.Shape.Bounds))
            {
                throw new Exception("This should not happen, child does not fit within node bounds");
            }


            // get exclusive Access to 'node'
            node.GetAccess();
            // Rearrange node's children if need be
            if (!node.HasChildNodes() && node.Objects.Count + 1 > _maxObjectsPerLeaf)
            {
                SetupChildNodes(node);

                List<T> childObjects = new List<T>(node.Objects);
                List<T> childrenToRelocate = new List<T>();

                foreach (T childObject in childObjects)
                {
                    foreach (Ocnode childNode in node.Nodes)
                    {
                        if (childNode == null)
                        {
                            continue;
                        }

                        // fetch all childObjects which belong into sub-nodes
                        if (childNode.Bounds.Contains(childObject.Shape.Bounds))
                        {
                            childrenToRelocate.Add(childObject);
                        }
                    }
                }

                // remove all childobjects which need to be relocated and re-add them
                foreach (var childObject in childrenToRelocate)
                {
                    RemoveSpatialObjectFromNode(childObject);
                    InsertNodeObject(node, childObject);
                }
            }


            // check for all childNodes whether spatialObject belongs into them and insert if so
            foreach (var childNode in node.Nodes)
            {
                if (childNode == null)
                {
                    continue;
                }
                if (!childNode.Bounds.Contains(spatialObject.Shape.Bounds))
                {
                    continue;
                }

                // release exclusive Access to 'node', since work will continue in a sub-tree
                node.ReleaseAccess();
                InsertNodeObject(childNode, spatialObject);
                return;
            }

            // object belongs into this node, so add
            AddSpatialObjectToNode(node, spatialObject);

            // release exclusive Access to 'node'
            node.ReleaseAccess();
        }

        private void RemoveSpatialObjectFromNode(T quadObject)
        {
            Ocnode node = _objectToNodeLookup[quadObject];
            node.Shapes.Remove(quadObject);
            Ocnode junk;
            _objectToNodeLookup.TryRemove(quadObject, out junk);
        }

        private void AddSpatialObjectToNode(Ocnode node, T quadObject)
        {
            node.Shapes.Add(quadObject);
            _objectToNodeLookup.TryAdd(quadObject, node);
        }


        private void SetupChildNodes(Ocnode node)
        {
            if (_minLeafSize.X <= node.Bounds.Width / 2 && _minLeafSize.Y <= node.Bounds.Height / 2 &&
                _minLeafSize.Z <= node.Bounds.Length / 2)
            {
                Vector3 dimension = new Vector3
                (node.Bounds.Width / 2,
                    node.Bounds.Height / 2,
                    node.Bounds.Length / 2);

                node[Direction.Nwt] = new Ocnode
                (node.Bounds.LeftBottomFront.X,
                    node.Bounds.LeftBottomFront.Y,
                    node.Bounds.LeftBottomFront.Z + node.Bounds.Length / 2,
                    dimension);
                node[Direction.Net] = new Ocnode
                (node.Bounds.LeftBottomFront.X + node.Bounds.Width / 2,
                    node.Bounds.LeftBottomFront.Y,
                    node.Bounds.LeftBottomFront.Z + node.Bounds.Length / 2,
                    dimension);
                node[Direction.Swt] = new Ocnode
                (node.Bounds.LeftBottomFront.X,
                    node.Bounds.LeftBottomFront.Y + node.Bounds.Height / 2,
                    node.Bounds.LeftBottomFront.Z + node.Bounds.Length / 2,
                    dimension);
                node[Direction.Set] = new Ocnode
                (node.Bounds.LeftBottomFront.X + node.Bounds.Width / 2,
                    node.Bounds.LeftBottomFront.Y + node.Bounds.Height / 2,
                    node.Bounds.LeftBottomFront.Z + node.Bounds.Length / 2,
                    dimension);
                node[Direction.Nwb] = new Ocnode
                (node.Bounds.LeftBottomFront.X,
                    node.Bounds.LeftBottomFront.Y,
                    node.Bounds.LeftBottomFront.Z,
                    dimension);
                node[Direction.Neb] = new Ocnode
                (node.Bounds.LeftBottomFront.X + node.Bounds.Width / 2,
                    node.Bounds.LeftBottomFront.Y,
                    node.Bounds.LeftBottomFront.Z,
                    dimension);
                node[Direction.Swb] = new Ocnode
                (node.Bounds.LeftBottomFront.X,
                    node.Bounds.LeftBottomFront.Y + node.Bounds.Height / 2,
                    node.Bounds.LeftBottomFront.Z,
                    dimension);
                node[Direction.Seb] = new Ocnode
                (node.Bounds.LeftBottomFront.X + node.Bounds.Width / 2,
                    node.Bounds.LeftBottomFront.Y + node.Bounds.Height / 2,
                    node.Bounds.LeftBottomFront.Z,
                    dimension);
            }
        }

        public List<T> Query(Sphere sphere, int maxResults = -1)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}