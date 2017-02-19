using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hik.Communication.ScsServices.Service;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using LIFE.Components.CloudSupport.File;

[assembly: InternalsVisibleTo("GridPotentialFieldLayerTests")]
[assembly: InternalsVisibleTo("GeoPotentialFieldLayerTests")]

namespace LIFE.Components.GridPotentialFieldLayer {

  public abstract class AbstractPotentialFieldLayer<TPotentialFieldType> : ScsService, ILayer
    where TPotentialFieldType : PotentialField {

    private const int MaxPotential = 100;
    private long _currentTick;
    protected TPotentialFieldType Field;

    public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle,
      UnregisterAgent unregisterAgentHandle) {
      var fileId = layerInitData.FileInitInfo.FileId;
      var filePath = new FileClient().DownloadFile(fileId);
      LoadField(filePath);
      return true;
    }

    public long GetCurrentTick() {
      return _currentTick;
    }

    public void SetCurrentTick(long currentTick) {
      _currentTick = currentTick;
    }

    internal void LoadField(string filePath) {
      Field = GetPotentialFieldLoader().LoadPotentialField(filePath);
    }

    protected abstract IFieldLoader<TPotentialFieldType> GetPotentialFieldLoader();

    protected virtual int ExploreClosestWithEndlessSight(int cell) {
      if (Field.PotentialFieldData[cell] == MaxPotential)
        return cell;
      if (Field.PotentialFieldData[cell] == 0)
        return GetClosestFullPotentialCell(FindAnyCellWithPotential(cell));
      return GetClosestFullPotentialCell(cell);
    }

    private int FindAnyCellWithPotential(int cell) {
      var toVisit = new Queue<int>();
      toVisit.Enqueue(cell);

      IList<int> visited = new List<int>();
      while (toVisit.Count != 0) {
        var itemToVisit = toVisit.Dequeue();
        if (Field.PotentialFieldData[itemToVisit] > 0)
          return itemToVisit;
        var neighbors = GetNeighborCells(itemToVisit);
        foreach (var neighbor in neighbors)
          if (!toVisit.Contains(neighbor) && !visited.Contains(neighbor) && (Field.PotentialFieldData.Length > neighbor))
            toVisit.Enqueue(neighbor);
        visited.Add(itemToVisit);
      }
      throw new InvalidOperationException(
        "There should always be a field with potential except there is no potential on the whole field.");
    }

    protected virtual int ExploreClosestFullPotentialField(int cell) {
      if (Field.PotentialFieldData[cell] == MaxPotential)
        return cell;
      if (Field.PotentialFieldData[cell] == 0)
        return -1;
      return GetClosestFullPotentialCell(cell);
    }

    protected virtual bool HasFullPotential(int cell) {
      return (cell != -1) && (Field.PotentialFieldData[cell] == MaxPotential);
    }

    private int GetClosestFullPotentialCell(int cell) {
      var neighbors = GetNeighborCells(cell);
      var maxNeighborValue = -1;
      var neighborWithMaxValue = -1;
      foreach (var neighbor in neighbors)
        if (Field.PotentialFieldData[neighbor] > maxNeighborValue) {
          maxNeighborValue = Field.PotentialFieldData[neighbor];
          neighborWithMaxValue = neighbor;
        }
      if (neighborWithMaxValue == -1)
        throw new InvalidOperationException("There should be a valid neighbor. Otherwise this method should return null");
      if (maxNeighborValue == MaxPotential)
        return neighborWithMaxValue;
      return GetClosestFullPotentialCell(neighborWithMaxValue);
    }

    private IEnumerable<int> GetNeighborCells(int currentCell) {
      var neighbors = new List<int>();
      var upperMostRow = currentCell < Field.NumberOfGridCellsX;
      var bottomMostRow = currentCell > Field.NumberOfGridCellsX*(Field.NumberOfGridCellsY - 1);
      var leftColumn = (currentCell == 0) || (currentCell%Field.NumberOfGridCellsX == 0);
      var rightColumn = (currentCell != 0) && (currentCell%Field.NumberOfGridCellsX == Field.NumberOfGridCellsX - 1);

      if (!upperMostRow) {
        neighbors.Add(currentCell - Field.NumberOfGridCellsX);
        if (!leftColumn) neighbors.Add(currentCell - Field.NumberOfGridCellsX - 1);
        if (!rightColumn) neighbors.Add(currentCell - Field.NumberOfGridCellsX + 1);
      }
      if (!leftColumn) {
        neighbors.Add(currentCell - 1);
        if (!bottomMostRow) neighbors.Add(currentCell + Field.NumberOfGridCellsX - 1);
      }
      if (!rightColumn) {
        neighbors.Add(currentCell + 1);
        if (!bottomMostRow) neighbors.Add(currentCell + Field.NumberOfGridCellsX + 1);
      }
      if (!bottomMostRow)
        neighbors.Add(currentCell + Field.NumberOfGridCellsX);
      return neighbors;
    }
  }
}