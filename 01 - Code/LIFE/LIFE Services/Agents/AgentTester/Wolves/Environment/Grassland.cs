﻿using System;
using System.Collections.Generic;
using System.Linq;
using AgentTester.Wolves.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using ESCTestLayer.Implementation;
using GenericAgentArchitectureCommon.Datatypes;
using GenericAgentArchitectureCommon.Interfaces;

namespace AgentTester.Wolves.Environment {
  
  /// <summary>
  ///   This grassland is home to sheeps and wolves ... and yes, 'grass'.
  ///   It serves two purposes here: Environment representation and data source.
  /// </summary>
  class Grassland : IEnvironment, IGenericDataSource {

    private readonly IEnvironment _env;   // Environment implementation.


    /// <summary>
    ///   Create a new grassland.
    /// </summary>
    /// <param name="useESC">Shall an ESC be used?</param>
    /// <param name="boundaries">Positive extent of the environment.</param>
    /// <param name="isGrid">Shall a grid be used?</param>
    public Grassland(bool useESC, Vector boundaries, bool isGrid) {
      if (useESC) _env = new ESCAdapter(new UnboundESC(), boundaries, isGrid);  
      else        _env = new Environment2D(boundaries, isGrid);
    }


    /// <summary>
    ///   Retrieve information from a data source.
    ///   Overrides GetData to provide additional "Grass" agent queries.
    /// </summary>
    /// <param name="spec">Information object describing which data to query.</param>
    /// <returns>An object representing the percepted information.</returns>
    public object GetData(ISpecificator spec) {
      
      if (!(spec is Halo)) throw new Exception(
        "[Grassland] Error on GetData() specificator: Not of type 'Halo'!");
      var halo = (Halo) spec;

      switch ((InformationTypes) spec.GetInformationType()) {

        case InformationTypes.AllAgents: {
          var objects = new List<ISpatialObject>();
          foreach (var obj in _env.GetAllObjects())
            if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
          return objects;
        }

        case InformationTypes.Grass: {
          var grass = new List<ISpatialObject>();
          foreach (var obj in _env.GetAllObjects().OfType<Grass>())
            if (halo.IsInRange(obj.GetPosition().GetTVector())) grass.Add(obj);
          return grass;
        }

        default: return null;
      }
    }


    /// <summary>
    ///   Returns a random, free position (for random agent movement).
    ///   ATTENTION: This function is only supported for the Env2D version!
    /// </summary>
    /// <returns>A vector representing a free position.</returns>
    public Vector GetRandomPosition() {
      if (_env is Environment2D) return ((Environment2D) _env).GetRandomPosition();
      throw new Exception(
        "[[Grassland] Error on GetRandomPosition(): Not implemented for ESC version!");
    }



    /* Redirection of environment functions to concrete implementation. */
    /********************************************************************/

    public void AddObject(ISpatialObject obj, Vector pos, out DataAccessor acc, Vector dim, Direction dir) {
      _env.AddObject(obj, pos, out acc, dim, dir);
    }

    public void RemoveObject(ISpatialObject obj) {
      _env.RemoveObject(obj);
    }

    public void MoveObject(ISpatialObject obj, Vector movement, Direction dir = null) {
      _env.MoveObject(obj, movement, dir);
    }

    public List<ISpatialObject> GetAllObjects() {
      return _env.GetAllObjects();
    }

    public void AdvanceEnvironment() {
      _env.AdvanceEnvironment();
    }
  }
}
