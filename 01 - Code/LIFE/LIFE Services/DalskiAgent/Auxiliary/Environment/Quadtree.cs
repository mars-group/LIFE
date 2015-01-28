﻿using System;
using System.Collections.Generic;
using LifeAPI.Spatial;

namespace DalskiAgent.Auxiliary.Environment {
  
  /// <summary>
  ///   Two-dimensional float structure.
  /// </summary>
  public struct Float2 {
    public float X, Y;
    public Float2(float x, float y) {
      X = x;
      Y = y;
    }
  };


  /// <summary>
  ///   Yet another quad tree.
  /// </summary>
  public class Quadtree {

    /*  ┌─────┬─────┐  Default
     *  │ II. │  I. │  quadrant
     *  ├─────┼─────┤  enumeration
     *  | III.│ IV. │  (counter clockwise).
     *  └─────┴─────┘  Array: 0-3.
     */

    private readonly int _level;                    // Nested depth of quadtree node.
    private readonly Float2 _position;              // Beginning position of this node.
    private readonly Float2 _span;                  // The extent (width and height).
    private readonly List<ISpatialEntity> _objects; // List of contained objects.
    private readonly Quadtree[] _childNodes;        // Sub nodes (if quadtree is segmented).  
    private const int MaxObjects = 5;               // Maximum object count before node is split.


    /// <summary>
    ///   Create a new quadtree.
    /// </summary>
    /// <param name="level">Nested depth of quadtree node.</param>
    /// <param name="pos">Beginning position of this node.</param>
    /// <param name="span">The extent (width and height).</param>
    public Quadtree(int level, Float2 pos, Float2 span) {
      _level = level;
      _position = pos;
      _span = span;
      _objects = new List<ISpatialEntity>();
      _childNodes = new Quadtree[4];
    }


    /// <summary>
    ///   Clears the quadtree. Deletes all stored objects 
    ///   and clears recursively all child nodes.
    /// </summary>
    public void Clear() {
      _objects.Clear();
      for (int i = 0; i < 4; i++) {
        if (_childNodes[i] != null) {
          _childNodes[i].Clear();
          _childNodes[i] = null;
        }
      }
    }


    /// <summary>
    ///   Splits this node into four sub nodes.
    /// </summary>
    private void Split() {     
      Float2 newSize = new Float2(_span.X/2, _span.Y/2);
      float x = _position.X;
      float y = _position.Y;
      _childNodes[0] = new Quadtree(_level+1, new Float2(x+newSize.X, y),           newSize);
      _childNodes[1] = new Quadtree(_level+1, new Float2(x,           y),           newSize);
      _childNodes[2] = new Quadtree(_level+1, new Float2(x,           y+newSize.Y), newSize);
      _childNodes[3] = new Quadtree(_level+1, new Float2(x+newSize.X, y+newSize.Y), newSize);       
    }


    /// <summary>
    ///   Determine to which node a new object belongs to.
    /// </summary>
    /// <param name="pos">The position of the object to insert.</param>
    /// <param name="span">The object's width and height.</param>
    /// <returns>The subtree index or -1, if the object stays in the parent node.</returns>
    private int GetIndex(Float2 pos, Float2 span) {
      int index = -1;
        
      // Check, if object completely fits in top/bottom row. Abort, if not! 
      Float2 center = new Float2(_position.X + _span.X/2, _position.Y + _span.Y/2);
      bool fitsTop    = (pos.Y < center.Y && pos.Y+span.Y < center.Y);
      bool fitsBottom = (pos.Y > center.Y);
      if (!fitsTop && !fitsBottom) return -1;

      // Object can completely fit within the left quadrants.
      if (pos.X < center.X && pos.X+span.X < center.X) index = fitsTop? 1 : 2;    

      // Object can completely fit within the right quadrants.
      else if (pos.X > center.X) index = fitsTop? 0 : 3;   

      return index;
    }


    /// <summary>
    ///   Inserts an object into the quadtree.
    /// </summary>
    /// <param name="obj"></param>
    public void Insert(ISpatialEntity obj) {
      Float2 pos  = new Float2((float) obj.Shape.Position.X,   (float) obj.Shape.Position.Y);
      Float2 span = new Float2((float) obj.Shape.Bounds.Width, (float) obj.Shape.Bounds.Height);        
        

      if (_childNodes[0] != null) {       // If this node has subnodes ...
        int index = GetIndex(pos, span);  // find the right node ...
        if (index != -1) {                // and if found ...
          _childNodes[index].Insert(obj); // insert it there!
          return;
        }
      }

      // The object has to be inserted here.
      _objects.Add(obj);
      if (_objects.Count > MaxObjects) {
        if (_childNodes[0] == null) Split();

        int i = 0;
        while (i < _objects.Count) {
          Float2 p2 = new Float2((float) _objects[i].Shape.Position.X,   (float) _objects[i].Shape.Position.Y);
          Float2 s2 = new Float2((float) _objects[i].Shape.Bounds.Width, (float) _objects[i].Shape.Bounds.Height);        
    
          int index = GetIndex(p2, s2);
          if (index != -1) {
            ISpatialEntity tmp = _objects[i];
            _objects.Remove(tmp);
            _childNodes[index].Insert(tmp);
            
          }
          else i ++;  // We can't move this object to a sub node.
        }
      }
    }


    /// <summary>
    ///   Returns all objects that could collide with the given object.
    /// </summary>
    /// <param name="retList">Return list (for recursive additions).</param>
    /// <param name="pos">Position of the given object.</param>
    /// <param name="span">Its width and height.</param>
    /// <returns></returns>
    public List<ISpatialEntity> Retrieve(List<ISpatialEntity> retList, Float2 pos, Float2 span) {
      int index = GetIndex(pos, span);
      if (index != -1 && _childNodes[0] != null) _childNodes[index].Retrieve(retList, pos, span);
      for (int i = 0; i < _objects.Count; i ++) retList.Add(_objects[i]);
      return retList;
    }


    /// <summary>
    ///   Recursive function that outputs the content of this tree and its children.
    /// </summary>
    public void Print(int index) {
      if (_childNodes[0] != null) {
        for (int i = 0; i < 4; i++) _childNodes[i].Print(i);
      }

      for (int i = 0; i < _level; i ++) Console.Write(" ");
      Console.WriteLine("["+index+"]: Pos: ("+_position.X+","+_position.Y+
                        ")  Size: ("+_span.X+","+_span.Y+
                        ")  Objects: "+_objects.Count);
    }
  }
}
