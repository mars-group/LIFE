﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CellLayer;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GoapActionSystem.Implementation;

namespace HumanLayer.Agents
{
    class SmartHuman : SpatialAgent

    {
        private readonly IExecution _exec;
        private readonly IEnvironment _env;
        private readonly Vector _pos;
        public readonly Guid GuidID;

        public SmartHuman(IExecution exec, IEnvironment env, Vector pos, Guid guid) : base(exec, env, pos) {
            _exec = exec;
            _env = env;
            _pos = pos;
            GuidID = guid;


            DataSensor cellSensor = new DataSensor(this, env, (int)CellLayerImpl.CellDataTypes.CellData,new CellHalo(Data.Position));
            PerceptionUnit.AddSensor(cellSensor);


            //ReasoningComponent = GoapComponent.LoadGoapConfiguration(agentConfigFileName, namespaceOfModelDefinition, _blackboard);

         

            //CellData sensorData = (CellData)PerceptionUnit.GetData((int)CellLayerImpl.CellDataTypes.CellData).Data; // ! hier muß auf den konkreten Datentyp, den der cell layer gibt gecastet werden

            Init(); // !!
        }




    }
}
