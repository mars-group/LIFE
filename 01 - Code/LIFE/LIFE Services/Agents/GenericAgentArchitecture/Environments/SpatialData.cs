﻿using System;
using DalskiAgent.Movement;

namespace DalskiAgent.Environments {
    
  /// <summary>
  ///   This class holds all spatial data for an agent.
  /// </summary>
  public class SpatialData {
    
    public Vector Position;               // The agent's current position.
    public Direction Direction;           // Pitch and yaw values.


    /// <summary>
    ///   Create a new spatial data set.
    /// </summary>
    /// <param name="pos">The initial position (common transport vector).</param>
    public SpatialData(Vector pos) {
      if (pos == null) throw new Exception("[SpatialData] Error on initialization: Position is 'null'!");
      Position = new Vector(pos.X, pos.Y, pos.Z);
      Direction = new Direction();       // Default facing is straight line northbound.
    }   
  }
}
