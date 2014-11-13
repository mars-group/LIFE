using System;
using CellLayer;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    internal class SmartHuman : SpatialAgent, IAgentLogic {
        private readonly IExecution _exec;
        private readonly IEnvironment _env;
        private readonly Vector _pos;
        public readonly Guid GuidID;
        private CellLayerImpl _cellLayer;
        private Blackboard _blackboard;
        private AbstractGoapSystem _goapActionSystem;

        public SmartHuman(IExecution exec, IEnvironment env, Vector pos, Guid guid, CellLayerImpl cellLayer) : base(exec, env, pos) {
            _exec = exec;
            _env = env;
            _pos = pos;
            GuidID = guid;
            _cellLayer = cellLayer;
            

            DataSensor cellSensor = new DataSensor
                (this, env, (int) CellLayerImpl.CellDataTypes.CellData, new CellHalo(Data.Position));
            PerceptionUnit.AddSensor(cellSensor);

            _cellLayer.AddAgent((int)this.Id, _pos.X, _pos.Y, CellLayerImpl.BehaviourType.Reflective);

            _blackboard = new Blackboard();
            //_goapActionSystem = GoapComponent.LoadGoapConfiguration(agentConfigFileName, namespaceOfModelDefinition, _blackboard);

            Init(); 
        }

        #region IAgentLogic Members

        public IInteraction Reason() {

            CellData sensorData = (CellData)PerceptionUnit.GetData((int)CellLayerImpl.CellDataTypes.CellData).Data; // ! hier muß auf den konkreten Datentyp, den der cell layer gibt gecastet werden
            HumanLayerImpl.Log.Info(sensorData.ToString());

           



            return new IdleAround();
        }

        #endregion
    }

    public class IdleAround : IInteraction {
        #region IInteraction Members

        public void Execute() {
            //HumanLayerImpl.Log.Info("Smart Action done");
        }

        #endregion
    }
}