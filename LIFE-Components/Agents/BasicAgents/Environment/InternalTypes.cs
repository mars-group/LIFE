﻿using System;
using LIFE.API.GridCommon;

namespace LIFE.Components.Agents.BasicAgents.Environment {

  /// <summary>
  ///   A two-dimensional cartesian position structure.
  ///   It makes (X,Y) and the yaw attribute readable.
  /// </summary>
  public class Position2D {
    private readonly CartesianPosition _pos; // The underlying position container.

    /// <summary>
    ///   Create a new 2D position structure.
    /// </summary>
    /// <param name="pos">Position data container.</param>
    public Position2D(CartesianPosition pos) {
      _pos = pos;
    }

    public double X => _pos.X;     // X-coordinate.
    public double Y => _pos.Y;     // Y-coordinate.
    public double Yaw => _pos.Yaw; // Agent orientation.
  }


  /// <summary>
  ///   The three-dimensional cartesian position structure.
  ///   It allow access to (X,Y,Z), yaw and pitch attributes.
  /// </summary>
  public class Position3D {
    private readonly CartesianPosition _pos; // The underlying position container.

    /// <summary>
    ///   Create a new 2D position structure.
    /// </summary>
    /// <param name="pos">Position data container.</param>
    public Position3D(CartesianPosition pos) {
      _pos = pos;
    }

    public double X => _pos.X;       // X-coordinate.
    public double Y => _pos.Y;       // Y-coordinate.
    public double Z => _pos.Z;       // Z-coordinate.
    public double Yaw => _pos.Yaw;   // Agent orientation (X,Y).
    public double Pitch => _pos.Yaw; // Agent climb angle.
  }


  /// <summary>
  ///   R/O position structure for 2D grid agents.
  ///   It provides an integer (X,Y) position.
  /// </summary>
  public class ReadOnlyGridPosition {
    private readonly GridPosition _pos; // The underlying position container.

    /// <summary>
    ///   Create a new grid position structure.
    /// </summary>
    /// <param name="pos">Position data container.</param>
    public ReadOnlyGridPosition(GridPosition pos) {
      _pos = pos;
    }

    public int X => Convert.ToInt32(_pos.X);               // X-coordinate.
    public int Y => Convert.ToInt32(_pos.Y);               // Y-coordinate.
    public int Yaw => Convert.ToInt32(_pos.GridDirection); // Yaw value.
    public GridDirection Direction => _pos.GridDirection;  // Yaw as grid direction constant.
  }
}