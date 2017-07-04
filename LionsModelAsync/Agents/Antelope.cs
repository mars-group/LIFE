using System;
using System.Collections.Generic;
using System.Text;
using AsyncAgents.Agents;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using LionsModelAsync.Environment;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using WolvesModel.Agents;
using WolvesModel.Interactions;

namespace LionsModelAsync.Agents
{
    public class Antelope: Agent2DAsync, IEatInteractionTarget, IEatInteractionSource, ISimResult
    {
        public int Energy { get; private set; }            // Current energy (with initial value).
        public int EnergyMax { get; private set; }         // Maximum health.
        public int Hunger { get; private set; }            // Hunger value of this antelope.
        public Antelope AgentReference => this;      // Concrete agent reference. 
        private readonly Random _random;                   // Random number generator for energy loss.
        private readonly IEnvironmentLayer _environment;   // Environment reference for random movement.
        public Antelope(IEnvironmentLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IAsyncEnvironment env, byte[] id = null, string collisionType = null, int freq = 1) 
            : base(layer, regFkt, unregFkt, env, id, collisionType, freq)
        {
//            Mover2D.InsertIntoEnvironment();
        }

        protected override IInteraction Reason()
        {

            // TODO : Implement
            throw new NotImplementedException();
        }

        public int GetFoodValue()
        {
            return Energy;
        }

        public void RemoveAgent()
        {
            IsAlive = false;
        }

        public void IncreaseEnergy(int points)
        {
            Energy += points;
            if (Energy > EnergyMax) Energy = EnergyMax;
        }
    }
}
