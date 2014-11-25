using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using GenericAgentArchitectureCommon.Datatypes;

namespace ESCTest.Entities
{
    class TestSpatialAgent : SpatialAgent
    {
        public TestSpatialAgent(IExecution exec, IEnvironment env, Vector pos, Vector dim = null, Direction dir = null) : base(exec, env, pos, dim, dir) { }
    }
}
