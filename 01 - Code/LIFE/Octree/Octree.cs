namespace Octree {
    #region Namespace imports

    using System.Collections.Generic;
    using CommonTypes.DataTypes;
    using LayerAPI.Interfaces;

    #endregion

    public class Octree {
        public AABB Bounds;
        public List<Item> items;
        public Octree[,,] Children = null;

        public Octree() {
            items = new List<Item>();
        }

        public Octree(AABB bounds) {
            Bounds = bounds;
            items = new List<Item>();
        }

        // Inserts an item into the category-th list (possibly of children, based on the item's bounding box)
        public void InsertItem(Item item) {
            if (AABB.CheckIntersection(item.aabb, Bounds)) {
                items.Add(item);

                if (Children != null) {
                    foreach (Octree grid in Children) {
                        grid.InsertItem(item);
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

        protected virtual Vector SplitPoint() {
            return CenterPoint();
        }

        public Vector CenterPoint() {
            return new Vector
                ((Bounds.Bounds[0][0] + Bounds.Bounds[0][1])*0.5f,
                    (Bounds.Bounds[1][0] + Bounds.Bounds[1][1])*0.5f,
                    (Bounds.Bounds[2][0] + Bounds.Bounds[2][1])*0.5f);
        }

        // Subdivides this PartitioningGrid, splitting it at the specified point (which ought to be insidwe this grid's AABB)
        public void Subdivide(Vector split) {
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
                            (new AABB {
                                Bounds = new[] {
                                    new[] {array[0][i], array[0][i + 1]},
                                    new[] {array[1][j], array[1][j + 1]},
                                    new[] {array[2][k], array[2][k + 1]}
                                }
                            });
                    }
                }
            }
            foreach (Octree child in Children) {
                foreach (Item item in items) {
                    child.InsertItem(item);
                }
            }
        }

        // Utility function to figure out how well a subdivision point splits stuff
        public int[,,] CheckSubdivision(Vector split) {
            int[,,] results = new int[2, 2, 2];
            float[][] array = {
                new[] {Bounds.Bounds[0][0], split.X, Bounds.Bounds[0][1]},
                new[] {Bounds.Bounds[1][0], split.Y, Bounds.Bounds[1][1]},
                new[] {Bounds.Bounds[2][0], split.Z, Bounds.Bounds[2][1]}
            };
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 2; j++) {
                    for (int k = 0; k < 2; k++) {
                        AABB subregion = new AABB {
                            Bounds =
                                new[] {
                                    new[] {array[0][i], array[0][i + 1]},
                                    new[] {array[1][j], array[1][j + 1]},
                                    new[] {array[2][k], array[2][k + 1]}
                                }
                        };
                        foreach (Item item in items) {
                            if (AABB.CheckIntersection(item.aabb, subregion))
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

        // Anything that can be stuck in an Octree must inmplement this

        #region Nested type: Item

        public struct Item {
            public AABB aabb;
            public object obj;
        }

        #endregion
    }
}