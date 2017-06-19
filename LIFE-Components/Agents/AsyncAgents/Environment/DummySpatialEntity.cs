using System;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace AsyncAgents.Environment
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
