using LIFE.Components.ESC.Implementation;
using LIFE.Components.ESC.SpatialAPI.Environment;

namespace EnvironmentServiceComponentTests.Entities
{
    public static class EnvironmentManager
    {
        public static IESC[] GetAll()
        {
            IESC[] escs =
            {
                //new NoSQLEnvironmentServiceComponent(),
                //new TreeESC(new SpatialObjectOctree<ISpatialEntity>(new Vector3(25, 25, 25), 1)),
                //new TreeESC(new BoundingVolumeHierarchy<ISpatialEntity>()),
                //new DistributedESC(),
                //new NoCollisionESC(),
                new DistributedESC(maxLeafObjectCount: 1000)
            };
            return escs;
        }
    }
}