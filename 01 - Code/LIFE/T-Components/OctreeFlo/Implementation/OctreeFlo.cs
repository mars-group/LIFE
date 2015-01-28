using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OctreeFlo.Interface;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;

namespace OctreeFlo.Implementation {

    public class OctreeFlo<T> : IOctreeFlo<T> where T : class, IShape {
        private readonly int maxObjectsPerLeaf;
        private readonly Vector3 minLeafSize;
        private readonly Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
        private readonly Dictionary<T, Ocnode> objectToNodeLookup = new Dictionary<T, Ocnode>();
        private readonly bool sort;
        private readonly object syncLock = new object();
        private int objectSortId;

        public OctreeFlo(Vector3 minLeafSize, int maxObjectsPerLeaf) {
            Root = null;
            this.minLeafSize = minLeafSize;
            this.maxObjectsPerLeaf = maxObjectsPerLeaf;
        }//TODO Octree muss ISpatialEntity verstehen und nicht IShape

        /// <summary>
        /// </summary>
        /// <param name="minLeafSize">The smallest size a leaf will split into</param>
        /// <param name="maxObjectsPerLeaf">Maximum number of objects per leaf before it forces a split into sub quadrants</param>
        /// <param name="sort">Whether or not queries will return objects in the order in which they were added</param>
        public OctreeFlo(Vector3 minLeafSize, int maxObjectsPerLeaf, bool sort)
            : this(minLeafSize, maxObjectsPerLeaf) {
            this.sort = sort;
        }

        public Ocnode Root { get; private set; }

        public void Insert(T shape) {
            //TODO does not undestand shapes with no dimension (at least as first added shape)
            lock (syncLock) {
                if (sort & !objectSortOrder.ContainsKey(shape)) {
                    objectSortOrder.Add(shape, objectSortId++);
                }
                var bounds = shape.Bounds;
                if (Root == null) {
                    var rootSize = new Vector3
                        (Math.Ceiling(bounds.Dimension.X/minLeafSize.X),
                            Math.Ceiling(bounds.Dimension.Y/minLeafSize.Y),
                            Math.Ceiling(bounds.Dimension.Z/minLeafSize.Z));

                    var multiplier = Math.Max(Math.Max(rootSize.X, rootSize.Y), rootSize.Z);
                    rootSize = rootSize*multiplier;

                    Root =
                        new Ocnode
                            (BoundingBox.GenerateByDimension
                                (bounds.Position,
                                    new Vector3(rootSize.X, rootSize.Y, rootSize.Z)));
                }
                while (!Root.Bounds.Contains(bounds)) {
                    ExpandRoot(bounds);
                }

                InsertNodeObject(Root, shape);
            }
        }

        public List<T> Query(BoundingBox bounds) {
            lock (syncLock) {
                var results = new List<T>();
                if (Root != null) {
                    Query(bounds, Root, results);
                }
                if (sort) {
                    results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
                }
                return results;
            }
        }

        public void Remove(T shape) {
            lock (syncLock) {
                if (sort && objectSortOrder.ContainsKey(shape)) {
                    objectSortOrder.Remove(shape);
                }

                if (!objectToNodeLookup.ContainsKey(shape)) {
                    throw new KeyNotFoundException("QuadObject not found in dictionary for removal");
                }

                var containingNode = objectToNodeLookup[shape];
                RemoveQuadObjectFromNode(shape);

                if (containingNode.Parent != null) {
                    CheckChildNodes(containingNode.Parent);
                }
            }
        }

        public int GetShapeCount() {
            lock (syncLock) {
                if (Root == null) {
                    return 0;
                }
                var count = GetQuadObjectCount(Root);
                return count;
            }
        }

        public int GetSortOrder(T quadObject) {
            lock (objectSortOrder) {
                if (!objectSortOrder.ContainsKey(quadObject)) {
                    return -1;
                }
                return objectSortOrder[quadObject];
            }
        }

        public List<Ocnode> GetAllNodes() {
            lock (syncLock) {
                var results = new List<Ocnode>();
                if (Root != null) {
                    results.Add(Root);
                    GetChildNodes(Root, results);
                }
                return results;
            }
        }

        public int GetQuadNodeCount() {
            lock (syncLock) {
                if (Root == null) {
                    return 0;
                }
                var count = GetQuadNodeCount(Root, 1);
                return count;
            }
        }

        #region Nested type: Ocnode

        public class Ocnode {
            private static int _id;
            private readonly Ocnode[] _nodes = new Ocnode[8];
            public readonly int ID = _id++;
            internal readonly List<T> Shapes = new List<T>();
            public ReadOnlyCollection<Ocnode> Nodes;
            public ReadOnlyCollection<T> Objects;

            public Ocnode(BoundingBox bounds) {
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<Ocnode>(_nodes);
                Objects = new ReadOnlyCollection<T>(Shapes);
            }

            public Ocnode(double x, double y, double z, Vector3 dimension)
                : this(
                    BoundingBox.GenerateByDimension
                        (
                            new Vector3(x + dimension.X/2, y + dimension.Y/2, z + dimension.Z/2),
                            new Vector3(dimension.X, dimension.Y, dimension.Z))) {}

            public Ocnode Parent { get; internal set; }

            public Ocnode this[Direction direction] {
                get {
                    switch (direction) {
                        case Direction.NWT:
                            return _nodes[0];
                        case Direction.NET:
                            return _nodes[1];
                        case Direction.SWT:
                            return _nodes[2];
                        case Direction.SET:
                            return _nodes[3];
                        case Direction.NWB:
                            return _nodes[4];
                        case Direction.NEB:
                            return _nodes[5];
                        case Direction.SWB:
                            return _nodes[6];
                        case Direction.SEB:
                            return _nodes[7];
                        default:
                            return null;
                    }
                }
                set {
                    switch (direction) {
                        case Direction.NWT:
                            _nodes[0] = value;
                            break;
                        case Direction.NET:
                            _nodes[1] = value;
                            break;
                        case Direction.SWT:
                            _nodes[2] = value;
                            break;
                        case Direction.SET:
                            _nodes[3] = value;
                            break;
                        case Direction.NWB:
                            _nodes[4] = value;
                            break;
                        case Direction.NEB:
                            _nodes[5] = value;
                            break;
                        case Direction.SWB:
                            _nodes[6] = value;
                            break;
                        case Direction.SEB:
                            _nodes[7] = value;
                            break;
                    }
                    if (value != null) {
                        value.Parent = this;
                    }
                }
            }

            public BoundingBox Bounds { get; private set; }

            public bool HasChildNodes() {
                return _nodes[0] != null;
            }
        }

        #endregion

        #region Private Methods

        private void Query(BoundingBox bounds, Ocnode node, List<T> results) {
            lock (syncLock) {
                if (node == null) {
                    return;
                }
                if (bounds.IntersectsWith(node.Bounds)) {
                    foreach (var quadObject in node.Objects) {
                        if (bounds.IntersectsWith(quadObject.Bounds)) {
                            results.Add(quadObject);
                        }
                    }

                    foreach (var childNode in node.Nodes) {
                        Query(bounds, childNode, results);
                    }
                }
            }
        }


        private void ExpandRoot(BoundingBox newChildBounds) {
            lock (syncLock) {
                var isNorth = Root.Bounds.LeftBottomFront.Y < newChildBounds.LeftBottomFront.Y;
                var isWest = Root.Bounds.LeftBottomFront.X < newChildBounds.LeftBottomFront.X;
                var isBottom = Root.Bounds.LeftBottomFront.Z < newChildBounds.LeftBottomFront.Z;

                Direction rootDirection;
                if (isBottom) {
                    if (isNorth) {
                        rootDirection = isWest ? Direction.NWB : Direction.NEB;
                    }
                    else {
                        rootDirection = isWest ? Direction.SWB : Direction.SEB;
                    }
                }
                else {
                    if (isNorth) {
                        rootDirection = isWest ? Direction.NWT : Direction.NET;
                    }
                    else {
                        rootDirection = isWest ? Direction.SWT : Direction.SET;
                    }
                }

                var newX = (rootDirection == Direction.NWB || rootDirection == Direction.SWB ||
                            rootDirection == Direction.NWT || rootDirection == Direction.SWT)
                    ? Root.Bounds.LeftBottomFront.X
                    : Root.Bounds.LeftBottomFront.X - Root.Bounds.Width;
                var newY = (rootDirection == Direction.NWB || rootDirection == Direction.NEB ||
                            rootDirection == Direction.NWT || rootDirection == Direction.NET)
                    ? Root.Bounds.LeftBottomFront.Y
                    : Root.Bounds.LeftBottomFront.Y - Root.Bounds.Height;
                var newZ = (rootDirection == Direction.NWT || rootDirection == Direction.NET ||
                            rootDirection == Direction.SWT || rootDirection == Direction.SET)
                    ? Root.Bounds.LeftBottomFront.Z
                    : Root.Bounds.LeftBottomFront.Z - Root.Bounds.Width;

                var newRootBounds = BoundingBox.GenerateByDimension
                    (
                        new Vector3(newX + Root.Bounds.Width, newY + Root.Bounds.Height, newZ + Root.Bounds.Length),
                        new Vector3(Root.Bounds.Width*2, Root.Bounds.Height*2, Root.Bounds.Length*2));
                var newRoot = new Ocnode(newRootBounds);
                SetupChildNodes(newRoot);
                newRoot[rootDirection] = Root;
                Root = newRoot;
            }
        }

        private void InsertNodeObject(Ocnode node, T quadObject) {
            lock (syncLock) {
                if (!node.Bounds.Contains(quadObject.Bounds)) {
                    throw new Exception("This should not happen, child does not fit within node bounds");
                }

                if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf) {
                    SetupChildNodes(node);

                    var childObjects = new List<T>(node.Objects);
                    var childrenToRelocate = new List<T>();

                    foreach (var childObject in childObjects) {
                        foreach (var childNode in node.Nodes) {
                            if (childNode == null) {
                                continue;
                            }

                            if (childNode.Bounds.Contains(childObject.Bounds)) {
                                childrenToRelocate.Add(childObject);
                            }
                        }
                    }

                    foreach (var childObject in childrenToRelocate) {
                        RemoveQuadObjectFromNode(childObject);
                        InsertNodeObject(node, childObject);
                    }
                }

                foreach (var childNode in node.Nodes) {
                    if (childNode != null) {
                        if (childNode.Bounds.Contains(quadObject.Bounds)) {
                            InsertNodeObject(childNode, quadObject);
                            return;
                        }
                    }
                }

                AddQuadObjectToNode(node, quadObject);
            }
        }

        private void ClearQuadObjectsFromNode(Ocnode node) {
            lock (syncLock) {
                var quadObjects = new List<T>(node.Objects);
                foreach (var quadObject in quadObjects) {
                    RemoveQuadObjectFromNode(quadObject);
                }
            }
        }

        private void RemoveQuadObjectFromNode(T quadObject) {
            lock (syncLock) {
                var node = objectToNodeLookup[quadObject];
                node.Shapes.Remove(quadObject);
                objectToNodeLookup.Remove(quadObject);
                quadObject.BoundsChanged -= quadObject_BoundsChanged;
            }
        }

        private void AddQuadObjectToNode(Ocnode node, T quadObject) {
            lock (syncLock) {
                node.Shapes.Add(quadObject);
                objectToNodeLookup.Add(quadObject, node);
                quadObject.BoundsChanged += quadObject_BoundsChanged;
            }
        }

        private void quadObject_BoundsChanged(object sender, EventArgs e) {
            lock (syncLock) {
                var quadObject = sender as T;
                if (quadObject != null) {
                    var node = objectToNodeLookup[quadObject];
                    if (!node.Bounds.Contains(quadObject.Bounds) || node.HasChildNodes()) {
                        RemoveQuadObjectFromNode(quadObject);
                        Insert(quadObject);
                        if (node.Parent != null) {
                            CheckChildNodes(node.Parent);
                        }
                    }
                }
            }
        }

        private void SetupChildNodes(Ocnode node) {
            lock (syncLock) {
                if (minLeafSize.X <= node.Bounds.Width/2 && minLeafSize.Y <= node.Bounds.Height/2 &&
                    minLeafSize.Z <= node.Bounds.Length/2) {
                    var dimension = new Vector3
                        (node.Bounds.Width/2,
                            node.Bounds.Height/2,
                            node.Bounds.Length/2);

                    node[Direction.NWT] = new Ocnode
                        (node.Bounds.LeftBottomFront.X,
                            node.Bounds.LeftBottomFront.Y,
                            node.Bounds.LeftBottomFront.Z + node.Bounds.Length/2,
                            dimension);
                    node[Direction.NET] = new Ocnode
                        (node.Bounds.LeftBottomFront.X + node.Bounds.Width/2,
                            node.Bounds.LeftBottomFront.Y,
                            node.Bounds.LeftBottomFront.Z + node.Bounds.Length/2,
                            dimension);
                    node[Direction.SWT] = new Ocnode
                        (node.Bounds.LeftBottomFront.X,
                            node.Bounds.LeftBottomFront.Y + node.Bounds.Height/2,
                            node.Bounds.LeftBottomFront.Z + node.Bounds.Length/2,
                            dimension);
                    node[Direction.SET] = new Ocnode
                        (node.Bounds.LeftBottomFront.X + node.Bounds.Width/2,
                            node.Bounds.LeftBottomFront.Y + node.Bounds.Height/2,
                            node.Bounds.LeftBottomFront.Z + node.Bounds.Length/2,
                            dimension);
                    node[Direction.NWB] = new Ocnode
                        (node.Bounds.LeftBottomFront.X,
                            node.Bounds.LeftBottomFront.Y,
                            node.Bounds.LeftBottomFront.Z,
                            dimension);
                    node[Direction.NEB] = new Ocnode
                        (node.Bounds.LeftBottomFront.X + node.Bounds.Width/2,
                            node.Bounds.LeftBottomFront.Y,
                            node.Bounds.LeftBottomFront.Z,
                            dimension);
                    node[Direction.SWB] = new Ocnode
                        (node.Bounds.LeftBottomFront.X,
                            node.Bounds.LeftBottomFront.Y + node.Bounds.Height/2,
                            node.Bounds.LeftBottomFront.Z,
                            dimension);
                    node[Direction.SEB] = new Ocnode
                        (node.Bounds.LeftBottomFront.X + node.Bounds.Width/2,
                            node.Bounds.LeftBottomFront.Y + node.Bounds.Height/2,
                            node.Bounds.LeftBottomFront.Z,
                            dimension);
                }
            }
        }


        private void CheckChildNodes(Ocnode node) {
            lock (syncLock) {
                if (GetQuadObjectCount(node) <= maxObjectsPerLeaf) {
                    // Move child objects into this node, and delete sub nodes
                    var subChildObjects = GetChildObjects(node);
                    foreach (var childObject in subChildObjects) {
                        if (!node.Objects.Contains(childObject)) {
                            RemoveQuadObjectFromNode(childObject);
                            AddQuadObjectToNode(node, childObject);
                        }
                    }
                    if (node[Direction.NWT] != null) {
                        node[Direction.NWT].Parent = null;
                        node[Direction.NWT] = null;
                    }
                    if (node[Direction.NET] != null) {
                        node[Direction.NET].Parent = null;
                        node[Direction.NET] = null;
                    }
                    if (node[Direction.SWT] != null) {
                        node[Direction.SWT].Parent = null;
                        node[Direction.SWT] = null;
                    }
                    if (node[Direction.SET] != null) {
                        node[Direction.SET].Parent = null;
                        node[Direction.SET] = null;
                    }
                    if (node[Direction.NWB] != null) {
                        node[Direction.NWB].Parent = null;
                        node[Direction.NWB] = null;
                    }
                    if (node[Direction.NEB] != null) {
                        node[Direction.NEB].Parent = null;
                        node[Direction.NEB] = null;
                    }
                    if (node[Direction.SWB] != null) {
                        node[Direction.SWB].Parent = null;
                        node[Direction.SWB] = null;
                    }
                    if (node[Direction.SEB] != null) {
                        node[Direction.SEB].Parent = null;
                        node[Direction.SEB] = null;
                    }

                    if (node.Parent != null) {
                        CheckChildNodes(node.Parent);
                    }
                    else {
                        // Its the root node, see if we're down to one quadrant, with none in local storage - if so, ditch the other three
                        var numQuadrantsWithObjects = 0;
                        Ocnode nodeWithObjects = null;
                        foreach (var childNode in node.Nodes) {
                            if (childNode != null && GetQuadObjectCount(childNode) > 0) {
                                numQuadrantsWithObjects++;
                                nodeWithObjects = childNode;
                                if (numQuadrantsWithObjects > 1) {
                                    break;
                                }
                            }
                        }
                        if (numQuadrantsWithObjects == 1) {
                            foreach (var childNode in node.Nodes) {
                                if (childNode != nodeWithObjects) {
                                    childNode.Parent = null;
                                }
                            }
                            Root = nodeWithObjects;
                        }
                    }
                }
            }
        }


        private List<T> GetChildObjects(Ocnode node) {
            lock (syncLock) {
                var results = new List<T>();
                results.AddRange(node.Shapes);
                foreach (var childNode in node.Nodes) {
                    if (childNode != null) {
                        results.AddRange(GetChildObjects(childNode));
                    }
                }
                return results;
            }
        }


        private int GetQuadObjectCount(Ocnode node) {
            lock (syncLock) {
                var count = node.Objects.Count;
                foreach (var childNode in node.Nodes) {
                    if (childNode != null) {
                        count += GetQuadObjectCount(childNode);
                    }
                }
                return count;
            }
        }


        private int GetQuadNodeCount(Ocnode node, int count) {
            lock (syncLock) {
                if (node == null) {
                    return count;
                }

                foreach (var childNode in node.Nodes) {
                    if (childNode != null) {
                        count++;
                    }
                }
                return count;
            }
        }


        private void GetChildNodes(Ocnode node, ICollection<Ocnode> results) {
            lock (syncLock) {
                foreach (var childNode in node.Nodes) {
                    if (childNode != null) {
                        results.Add(childNode);
                        GetChildNodes(childNode, results);
                    }
                }
            }
        }

        #endregion
    }

    public enum Direction {
        NWB = 0, //NORTH-WEST-BOTTOM
        NEB = 1,
        SWB = 2,
        SEB = 3,
        NWT = 4,
        NET = 5,
        SWT = 6,
        SET = 7 //SOUTH-EAST-TOP
    }

}