using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using SpatialCommon.Datatypes;
using SpatialCommon.Enums;

namespace ESCTest.Entities {

    internal class TestSpatialAgent : SpatialAgent {
        public TestSpatialAgent(IExecution exec, IEnvironment env, Vector pos, Vector dim = null, Direction dir = null)
            : base(exec, env, CollisionType.MassiveAgent, pos, dim, dir) {}
    }

}