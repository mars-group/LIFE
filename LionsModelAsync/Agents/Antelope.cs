using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsyncAgents.Agents;
using AsyncAgents.Perception;
using EnvironmentServiceComponent.SpatialAPI.Entities.Movement;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using LionsModelAsync.Environment;
using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using WolvesModel.Agents;
using WolvesModel.Interactions;

namespace LionsModelAsync.Agents
{
    public class Antelope: Agent2DAsync, IEatInteractionTarget, IEatInteractionSource, ISimResult
    {
        private IEnumerable<ISpatialEntity> _elementsInSight;

        private const int SIGHT_RADIUS = 50;
        public double Speed { get; private set; }            // Current energy (with initial value).

        public Vector3 CurrentPosition { get; private set; }
        public int Energy { get; private set; }            // Current energy (with initial value).
        public int EnergyMax { get; private set; }         // Maximum health.
        public int Hunger { get; private set; }            // Hunger value of this antelope.
        public string State { get; private set; }           // Agent State.
        public string Targets { get; private set; }        // Agent target sightings.
        public double TargetDistance { get; private set; } // Distance to active target. (-1 if not used)
        public Antelope AgentReference => this;      // Concrete agent reference. 
        private readonly Random _random;                   // Random number generator for energy loss.
        private readonly IEnvironmentLayer _environment;   // Environment reference for random movement.
        public Antelope(IEnvironmentLayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IAsyncEnvironment env, byte[] id = null, string collisionType = null, int freq = 1) 
            : base(layer, regFkt, unregFkt, env, id, collisionType, freq)
        {
            _random = new Random(ID.GetHashCode());
            _environment = layer;
            Mover2D.InsertIntoEnvironment(_random.Next(layer.DimensionX), _random.Next(layer.DimensionY));
            Energy = 80;
            EnergyMax = 100;
            State = "";
            Speed = 200;
            Targets = "";
            TargetDistance = -1f;
        }

        private void ExploreDelegate(EnvironmentResult res)
        {
            if (res.ResultCode == EnvironmentResultCode.OK)
            {
                _elementsInSight = res.InvolvedEntities;
            }
            else
            {
                _elementsInSight = new List<ISpatialEntity>();
                throw new Exception("Error while ESC explore : " + res.ResultCode);
            }
        }

        protected override IInteraction Reason()
        {
            //this.SensorArray.SenseAll();
            List<ISpatialEntity> collisions = new List<ISpatialEntity>();
            CurrentPosition = _env.GetSpatialEntity(this.ID).Shape.Position;
            //            EnvironmentResult moveResult = (EnvironmentResult) SensorArray.Get<MovementSensorAsync>(typeof(MovementSensorAsync).ToString()).Sense();
            EnvironmentResult moveResult = SensorArray.Get<MovementSensorAsync, EnvironmentResult>();
            //             SensorArray.Get>()
            //            moveResult.
            if (!moveResult.Success)
            {
                foreach (var act in moveResult.InvolvedEntities)
                {
                    if (act.AgentType == typeof(Antelope))
                    {
                        collisions.Add(act);
                    }
                }
                //                collisions[0].AgentType;
            }
            // Energy substraction is made first. 
            Energy -= 1 + _random.Next(3);
            if (Energy <= 0)
            {
                IsAlive = false;
                _environment.RemoveAgent(this.ID);
                return null;
            }
            IInteraction interaction;

            // Calculate hunger percentage, read-out nearby agents and remove own agent from perception list.
            Hunger = (int)((double)(EnergyMax - Energy) / EnergyMax * 100);

            var  grass= _elementsInSight.ToList();
            Targets = "A:" + grass.Count;
            // If grass exists and the sheep is hungry at all.
            if (grass.Count > 0 && Hunger > 20)
            {
                if (collisions.Any())
                {
                    State = "Eating antelope ( ID: " + collisions[0].AgentGuid + "  pos " + collisions[0].Shape.Position.X + ":" + collisions[0].Shape.Position.Y + ").";
                    foreach (var act in collisions)
                    {
                        var actAntelope = _environment.GetAntelope(act.AgentGuid);
                        if (actAntelope == null)
                            continue;
                        interaction = new EatInteraction(this, actAntelope);
                        // Write the properties to the result structure.
                        AgentData["Energy"] = Energy;
                        AgentData["EnergyMax"] = EnergyMax;
                        AgentData["Hunger"] = Hunger;
                        AgentData["State"] = State;
                        AgentData["Targets"] = Targets;
                        AgentData["TargetDistance"] = TargetDistance;
                        _env.Explore(new Circle(CurrentPosition.X, CurrentPosition.Y, SIGHT_RADIUS), ExploreDelegate);
                        return interaction;
                    }
                }
                // Get the nearest grass agent and calculate the distance towards it.
                var nearest = grass[0];
                TargetDistance = AgentMover.CalculateDistance2D(CurrentPosition.X, CurrentPosition.Y, nearest.Shape.Position.X, nearest.Shape.Position.Y);

                foreach (var g in grass)
                {
                    var dist = AgentMover.CalculateDistance2D(CurrentPosition.X, CurrentPosition.Y, nearest.Shape.Position.X, nearest.Shape.Position.Y);
                    if (dist < TargetDistance)
                    {
                        nearest = g;
                        TargetDistance = dist;
                    }
                }
                Direction dir;
                Vector3 pos;
                _environment.GetMoveTowardsPositionVector(this.ID, Speed, nearest.Shape.Position.X, nearest.Shape.Position.Y, out pos, out dir);
                _env.Explore(new Circle(pos.X, pos.Y, SIGHT_RADIUS), ExploreDelegate);
                interaction = Mover2D.SetToPosition(pos.X, pos.Y);
                State = "R2 - Moving towards grass (" + nearest.Shape.Position.X + "," + nearest.Shape.Position.Y + ").";
            }


            // Either the grass is too far away or the sheep is not hungry.
            // Wander randomly around.
            else
            {
                TargetDistance = -1f;
                State = "R3 - No target: Random movement.";
                var x= _random.Next(_environment.DimensionX);
                var y = _random.Next(_environment.DimensionY);

                Direction dir;
                Vector3 pos;
                _environment.GetMoveTowardsPositionVector(this.ID, Speed, x, y, out pos, out dir);
                _env.Explore(new Circle(pos.X, pos.Y, SIGHT_RADIUS), ExploreDelegate);
                interaction = Mover2D.SetToPosition(pos.X, pos.Y);
            }


            // Write the properties to the result structure.
            AgentData["Energy"] = Energy;
            AgentData["EnergyMax"] = EnergyMax;
            AgentData["Hunger"] = Hunger;
            AgentData["State"] = State;
            AgentData["Targets"] = Targets;
            AgentData["TargetDistance"] = TargetDistance;
            //Console.WriteLine(this);
            return interaction;  // End of reasoning.            throw new NotImplementedException();
        }

        public int GetFoodValue()
        {
            return Energy;
        }

        public void RemoveAgent()
        {
            IsAlive = false;
            _environment.RemoveAgent(this.ID);

        }

        public void IncreaseEnergy(int points)
        {
            Energy += points;
            if (Energy > EnergyMax) Energy = EnergyMax;
        }
    }
}
