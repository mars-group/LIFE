using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using SpatialCommon.Collision;
using SpatialCommon.Datatypes;

namespace ESCTest.Entities {

    internal class TestSpatialAgent : SpatialAgent {
        public TestSpatialAgent(IExecution exec, IEnvironment env, Vector pos, Vector dim = null, Direction dir = null)
            : base(exec, env, CollisionType.MassiveAgent, pos, dim, dir) {}
    }

}