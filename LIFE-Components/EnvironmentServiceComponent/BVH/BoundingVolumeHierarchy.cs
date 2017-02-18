using System;
using System.Collections.Generic;
using System.Linq;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace LIFE.Components.ESC.BVH {

  public enum Axis {
    X,
    Y,
    Z
  }

  public class BoundingVolumeHierarchy<T> : ITree<T> where T : class, ISpatialObject {

    private static int LEAF_OBJ_MAX = 10;
    private readonly NodeAdapter _nodeAdapter = new NodeAdapter();
    private readonly Node _root;
    private readonly object _syncLock = new object();

    /// <summary>
    ///   Constructs a <code>BoundingVolumeHierarchy</code> and adds given entities to it.
    /// </summary>
    /// <param name="entities">that can be added to the tree.</param>
    public BoundingVolumeHierarchy(List<T> entities = null, int leafObjectMax = 10) {
      LEAF_OBJ_MAX = leafObjectMax;
      _root = new Node(_nodeAdapter, entities ?? new List<T>());
    }

    public void Insert(T spatialObject) {
      lock (_syncLock) {
        var box = VolatileBoundingBox.FromBoundingBox(spatialObject.Shape.Bounds);
        var sah = _root.CalculateSAH(ref box);
        _root.AddObject(_nodeAdapter, spatialObject, ref box, sah);
      }
    }

    public void Remove(T spatialObject) {
      lock (_syncLock) {
        var leaf = _nodeAdapter.GetLeaf(spatialObject);
        leaf.RemoveObject(_nodeAdapter, spatialObject);
      }
    }

    public List<T> Query(Sphere sphere, int maxResults = -1) {
      var result = new List<T>();
      lock (_syncLock) {
        if (sphere != null)
          foreach (var node in Traverse(sphere.Bounds))
            foreach (var spatialEntity in node.Entities)
              if (spatialEntity.Shape.IntersectsWith(sphere)) {
                result.Add(spatialEntity);
                if ((maxResults > 0) && (result.Count >= maxResults)) return result;
              }
      }
      return result;
    }

    public List<T> Query(BoundingBox shape) {
      var result = new List<T>();
      lock (_syncLock) {
        if (shape != null)
          foreach (var node in Traverse(shape))
            foreach (var spatialEntity in node.Entities)
              if (spatialEntity.Shape.IntersectsWith(shape)) result.Add(spatialEntity);
      }
      return result;
    }

    public List<T> GetAll() {
      var result = new List<T>();
      foreach (var boundsSphere in TraverseAll())
        if (boundsSphere != null) foreach (var spatialEntity in boundsSphere.Entities) result.Add(spatialEntity);
      return result;
    }

    private List<Node> TraverseAll() {
      var hits = new List<Node>();
      Traverse(_root, box => true, hits);
      return hits;
    }

    private List<Node> Traverse(BoundingBox volume) {
      var hits = new List<Node>();
      Traverse(_root, box => box.IntersectsWith(volume), hits);
      return hits;
    }

    private void Traverse(Node node, IntersectionTestDelegate testIntersection, List<Node> hitlist) {
      if ((node != null) && testIntersection(node.Box.ToBoundingBox())) {
        hitlist.Add(node);
        Traverse(node.Left, testIntersection, hitlist);
        Traverse(node.Right, testIntersection, hitlist);
      }
    }

    private delegate bool IntersectionTestDelegate(BoundingBox box);

    #region NestedType Node

    private class Node {
      public readonly List<T> Entities;
      private int _depth;
      private bool _leaf;

      private Node _parent;
      public VolatileBoundingBox Box;
      public Node Right, Left;

      /// <summary>
      ///   Creates a root node and maps given entities to it.
      /// </summary>
      /// <param name="nodeAdapter">supervises all entity to node dependencies</param>
      /// <param name="entities">that should be saved in this node</param>
      public Node(NodeAdapter nodeAdapter, List<T> entities) : this(nodeAdapter, null, entities, 0) {}

      private Node(NodeAdapter nodeAdapter, Node parent, List<T> entities, int depth) {
        _parent = parent;
        Entities = entities;
        _depth = depth;
        _leaf = Entities.Count <= LEAF_OBJ_MAX;

        if (Entities.Any()) {
          ComputeVolumeByEntities();
          SplitNodeIfNecessary(nodeAdapter);

          if (_leaf) Entities.ForEach(o => nodeAdapter.MapEntityToLeaf(o, this));
          else ComputeVolumeByChildNodes(false);
        }
      }

      private bool Root {
        get { return _parent == null; }
      }

      private void ComputeVolumeByEntities() {
        var boundingBox = Entities[0].Shape.Bounds;
        Box.Min = boundingBox.Min;
        Box.Max = boundingBox.Max;
        for (var i = 1; i < Entities.Count; i++)
          ExpandVolumeTo(VolatileBoundingBox.FromBoundingBox(Entities[i].Shape.Bounds));
      }

      private void ExpandVolumeTo(VolatileBoundingBox newBox) {
        var expanded = false;
        var newMin = newBox.Min;
        var newMax = newBox.Max;
        if (newMin.X < Box.Min.X) {
          expanded = true;
          Box.Min.X = newMin.X;
        }
        if (newMin.Y < Box.Min.Y) {
          expanded = true;
          Box.Min.Y = newMin.Y;
        }
        if (newMin.Z < Box.Min.Z) {
          expanded = true;
          Box.Min.Z = newMin.Z;
        }
        if (newMax.X > Box.Max.X) {
          expanded = true;
          Box.Max.X = newMax.X;
        }
        if (newMax.Y > Box.Max.Y) {
          expanded = true;
          Box.Max.Y = newMax.Y;
        }
        if (newMax.Z > Box.Max.Z) {
          expanded = true;
          Box.Max.Z = newMax.Z;
        }
        if (expanded && !Root) _parent.ExpandVolumeTo(Box);
      }

      private void ComputeVolumeRecursiverly(T entity) {
        var oldbox = Box;

        ExpandVolumeTo(VolatileBoundingBox.FromBoundingBox(entity.Shape.Bounds));
        if (!Box.Equals(oldbox)) if (!Root) _parent.ComputeVolumeByChildNodes();
      }

      private void ComputeVolumeRecursiverly() {
        //TODO wo benutzt
        var oldbox = Box;

        ComputeVolumeByEntities();
        if (!Box.Equals(oldbox)) if (!Root) _parent.ComputeVolumeByChildNodes();
      }

      private double CalculateSAH(VolatileBoundingBox box) {
        var xSize = box.Max.X - box.Min.X;
        var ySize = box.Max.Y - box.Min.Y;
        var zSize = box.Max.Z - box.Min.Z;

        return 2.0f*(xSize*ySize + xSize*zSize + ySize*zSize);
      }

      internal double CalculateSAH(ref VolatileBoundingBox box) {
        var xSize = box.Max.X - box.Min.X;
        var ySize = box.Max.Y - box.Min.Y;
        var zSize = box.Max.Z - box.Min.Z;

        return 2.0f*(xSize*ySize + xSize*zSize + ySize*zSize);
      }

      private double CalculateSAH(Node node) {
        var xSize = node.Box.Max.X - node.Box.Min.X;
        var ySize = node.Box.Max.Y - node.Box.Min.Y;
        var zSize = node.Box.Max.Z - node.Box.Min.Z;

        return 2.0f*(xSize*ySize + xSize*zSize + ySize*zSize);
      }

      private void SplitNodeIfNecessary(NodeAdapter nodeAdapter) {
        if (Entities.Count > LEAF_OBJ_MAX) {
          _leaf = false;
          // second, decide which axis to split on, and sort..
          var splitlist = Entities;

          splitlist.ForEach(o => nodeAdapter.UnmapEntity(o));

          switch (PickSplitAxis()) {
            // sort along the appropriate axis
            case Axis.X:
              splitlist.Sort((e1, e2) => e1.Shape.Position.X.CompareTo(e2.Shape.Position.X));
              break;
            case Axis.Y:
              splitlist.Sort((e1, e2) => e1.Shape.Position.Y.CompareTo(e2.Shape.Position.Y));
              break;
            case Axis.Z:
              splitlist.Sort((e1, e2) => e1.Shape.Position.Z.CompareTo(e2.Shape.Position.Z));
              break;
            default:
              throw new NotImplementedException();
          }
          var center = splitlist.Count/2; // Find the center object in our current sub-list

          // create the new Left and Right nodes...
          Left = new Node(nodeAdapter, this, splitlist.GetRange(0, center), _depth + 1);
          // Split the Hierarchy to the Left
          Right = new Node
            (nodeAdapter, this, splitlist.GetRange(center, splitlist.Count - center), _depth + 1);
          // Split the Hierarchy to the Right     
          Entities.Clear();
        }
      }

      /// <summary>
      ///   Determine the biggest axis
      /// </summary>
      /// <returns>The axis with the highest range.</returns>
      private Axis PickSplitAxis() {
        var x = Box.Max.X - Box.Min.X;
        var y = Box.Max.Y - Box.Min.Y;
        var z = Box.Max.Z - Box.Min.Z;

        if (x > y) {
          if (x > z) return Axis.X;
          return Axis.Z;
        }
        if (y > z) return Axis.Y;
        return Axis.Z;
      }

      internal void AddObject
        (NodeAdapter nodeAdapter, T entity, ref VolatileBoundingBox entitiyBox, double sah) {
        if (_leaf) {
          Entities.Add(entity);
          nodeAdapter.MapEntityToLeaf(entity, this);
          ComputeVolumeRecursiverly(entity);
          SplitNodeIfNecessary(nodeAdapter);
        }
        else {
          var leftSAH = CalculateSAH(Left);
          var rightSAH = CalculateSAH(Right);
          var sendLeftSAH = rightSAH + CalculateSAH(Left.Box.CreateExpanded(entitiyBox)); // (L+N,R)
          var sendRightSAH = leftSAH + CalculateSAH(Right.Box.CreateExpanded(entitiyBox)); // (L,R+N)

          if (sendLeftSAH < sendRightSAH) Left.AddObject(nodeAdapter, entity, ref entitiyBox, sah);
          else Right.AddObject(nodeAdapter, entity, ref entitiyBox, sah);
        }
      }

      internal void RemoveObject(NodeAdapter nodeAdapter, T entity) {
        if (!_leaf) throw new Exception("RemoveObject() called on nonLeaf!");

        nodeAdapter.UnmapEntity(entity);
        Entities.Remove(entity);
        if (Entities.Any()) {
          ComputeVolumeRecursiverly();
        }
        else if (!Root) {
          // our leaf is empty, so collapse it if we are not the root...
          _parent.RemoveLeaf(nodeAdapter, this);
          _parent = null;
        }
      }


      private void RemoveLeaf(NodeAdapter nodeAdapter, Node removeLeaf) {
        if ((Left == null) || (Right == null)) throw new Exception("bad intermediate node");
        Node keepLeaf;
        if (removeLeaf == Left) keepLeaf = Right;
        else if (removeLeaf == Right) keepLeaf = Left;
        else throw new Exception("RemoveLeaf doesn't match any leaf!");

        // "become" the leaf we are keeping.
        Box = keepLeaf.Box;
        Left = keepLeaf.Left;
        Right = keepLeaf.Right;
        Entities.Clear();
        Entities.AddRange(keepLeaf.Entities);
        _leaf = keepLeaf._leaf;

        if (!Entities.Any()) {
          Left._parent = this;
          Right._parent = this; // reassign child parents..
          PropagateDepth(_depth); // this reassigns _depth for our children
        }
        else {
          // map the objects we adopted to us...                                                
          Entities.ForEach(o => { nodeAdapter.MapEntityToLeaf(o, this); });
        }

        // propagate our new volume..
        if (_parent != null) _parent.ComputeVolumeByChildNodes();
      }

      /// <summary>
      ///   Sets the _depth to this <code>Node</code> and recursive to it's child-nodes.
      /// </summary>
      /// <param name="depth">the value that will be propagated</param>
      private void PropagateDepth(int depth) {
        _depth = depth;
        if (!_leaf) {
          Left.PropagateDepth(depth + 1);
          Right.PropagateDepth(depth + 1);
        }
      }

      private void ComputeVolumeByChildNodes(bool recurse = true) {
        if (!_leaf) {
          Box.Min.X = Left.Box.Min.X;
          Box.Max.X = Left.Box.Max.X;
          Box.Min.Y = Left.Box.Min.Y;
          Box.Max.Y = Left.Box.Max.Y;
          Box.Min.Z = Left.Box.Min.Z;
          Box.Max.Z = Left.Box.Max.Z;

          if (Right.Box.Min.X < Box.Min.X) Box.Min.X = Right.Box.Min.X;
          if (Right.Box.Min.Y < Box.Min.Y) Box.Min.Y = Right.Box.Min.Y;
          if (Right.Box.Min.Z < Box.Min.Z) Box.Min.Z = Right.Box.Min.Z;

          if (Right.Box.Max.X > Box.Max.X) Box.Max.X = Right.Box.Max.X;
          if (Right.Box.Max.Y > Box.Max.Y) Box.Max.Y = Right.Box.Max.Y;
          if (Right.Box.Max.Z > Box.Max.Z) Box.Max.Z = Right.Box.Max.Z;

          if (recurse && !Root) _parent.ComputeVolumeByChildNodes();
        }
      }
    }

    #endregion

    #region NodeAdapter

    private class NodeAdapter {
      private readonly Dictionary<T, Node> _entityToNode = new Dictionary<T, Node>();

      public void MapEntityToLeaf(T entity, Node leaf) {
        _entityToNode[entity] = leaf;
      }

      public void UnmapEntity(T entity) {
        _entityToNode.Remove(entity);
      }

      public Node GetLeaf(T entity) {
        return _entityToNode[entity];
      }
    }

    #endregion
  }
}