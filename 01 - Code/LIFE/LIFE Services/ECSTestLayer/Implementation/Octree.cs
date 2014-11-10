using System.Collections.Generic;
using CommonTypes.TransportTypes;
using GenericAgentArchitectureCommon.Datatypes;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Implementation {

  public class Octree {

    private readonly List<ISpatialEntity> Entities = new List<ISpatialEntity>();
    private readonly AABB Bounds;
    private Octree[,,] Children;

    public Octree() {
      Bounds = new AABB();
    }

    public Octree(AABB bounds) {
      Bounds = bounds;
    }


    // Inserts an item into the category-th list (possibly of children, based on the item's bounding box)
    public void Insert(ISpatialEntity item) {
      if (AABB.CheckIntersection(item.GetBounds().GetAABB(), Bounds)) {
        Entities.Add(item);

        if (Children != null) {
          foreach (Octree grid in Children) {
            grid.Insert(item);
          }
        }
        else if (ConditionallySubdivide())
          Subdivide();
      }
    }

    // Subdivides this PartitioningGrid, splitting it exactly in half
    public void Subdivide() {
      Subdivide(SplitPoint());
    }

    protected virtual TVector SplitPoint() {
      return CenterPoint();
    }

    public TVector CenterPoint() {
      return new TVector
        ((Bounds.Bounds[0][0] + Bounds.Bounds[0][1])*0.5f,
          (Bounds.Bounds[1][0] + Bounds.Bounds[1][1])*0.5f,
          (Bounds.Bounds[2][0] + Bounds.Bounds[2][1])*0.5f);
    }

    // Subdivides this PartitioningGrid, splitting it at the specified point (which ought to be insidwe this grid's AABB)
    public void Subdivide(TVector split) {
      Children = new Octree[2, 2, 2];
      float[][] array = {
        new[] {Bounds.Bounds[0][0], split.X, Bounds.Bounds[0][1]},
        new[] {Bounds.Bounds[1][0], split.Y, Bounds.Bounds[1][1]},
        new[] {Bounds.Bounds[2][0], split.Z, Bounds.Bounds[2][1]}
      };
      for (int i = 0; i < 2; i++) {
        for (int j = 0; j < 2; j++) {
          for (int k = 0; k < 2; k++) {
            Children[i, j, k] = Subdivision
              (new AABB
                (new[] {
                  new[] {array[0][i], array[0][i + 1]},
                  new[] {array[1][j], array[1][j + 1]},
                  new[] {array[2][k], array[2][k + 1]}
                }));
          }
        }
      }
      foreach (Octree child in Children) {
        foreach (ISpatialEntity item in Entities) {
          child.Insert(item);
        }
      }
    }

    // Utility function to figure out how well a subdivision point splits stuff
    public int[,,] CheckSubdivision(TVector split) {
      int[,,] results = new int[2, 2, 2];
      float[][] array = {
        new[] {Bounds.Bounds[0][0], split.X, Bounds.Bounds[0][1]},
        new[] {Bounds.Bounds[1][0], split.Y, Bounds.Bounds[1][1]},
        new[] {Bounds.Bounds[2][0], split.Z, Bounds.Bounds[2][1]}
      };
      for (int i = 0; i < 2; i++) {
        for (int j = 0; j < 2; j++) {
          for (int k = 0; k < 2; k++) {
            AABB subregion = new AABB
              (
              new[] {
                new[] {array[0][i], array[0][i + 1]},
                new[] {array[1][j], array[1][j + 1]},
                new[] {array[2][k], array[2][k + 1]}
              });
            foreach (ISpatialEntity item in Entities) {
              if (AABB.CheckIntersection(item.GetBounds().GetAABB(), subregion))
                results[i, j, k]++;
            }
          }
        }
      }
      return results;
    }

    public static int RateSubdivision(int[,,] checkValues) {
      int total = 0;
      foreach (int i in checkValues) {
        total += i*i;
      }
      return total;
    }

    // Override in order to control whether an octree should be subdivided or not
    protected virtual bool ConditionallySubdivide() {
      return false;
    }

    // Function defining how to subdivide an Octree
    // Return values should generally be the same type as the subtype of Octree
    protected virtual Octree Subdivision(AABB aabb) {
      return new Octree(aabb);
    }
  }
}