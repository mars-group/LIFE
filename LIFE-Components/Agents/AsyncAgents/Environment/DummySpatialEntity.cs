using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace EnvironmentServiceComponent.Implementation
{
    class DummySpatialEntity : ISpatialEntity
    {
        
        public IShape Shape { get; set; }
        public Enum CollisionType { get; }
        public Guid AgentGuid { get; }
        public Type AgentType { get; }

        public DummySpatialEntity()
        {
            AgentGuid = Guid.Empty;
            AgentType = typeof(void);
            CollisionType = null;
        }
    }
}
