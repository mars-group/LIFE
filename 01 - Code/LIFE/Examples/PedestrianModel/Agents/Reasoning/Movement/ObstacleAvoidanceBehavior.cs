using DalskiAgent.Agents;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using PedestrianModel.Agents.Reasoning.Movement.Potential;
using PedestrianModel.Agents.Reasoning.Movement.Potential.Emitter;
using PedestrianModel.Util.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents.Reasoning.Movement
{
	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class ObstacleAvoidanceBehavior : ReactiveMovingBehavior
	{
		/// <summary>
		/// The function to calculate the potential value for other agent distances.
		/// </summary>
		private static readonly UnivariateRealFunction AGENT_DISTANCE_POTENTIAL_FUNCTION = new UnivariateRealFunctionAnonymousInnerClassHelper();

		private class UnivariateRealFunctionAnonymousInnerClassHelper : UnivariateRealFunction
		{
			public UnivariateRealFunctionAnonymousInnerClassHelper()
			{
			}

			//public override double Value(double x)
            public double Value(double x)
			{
				if (x <= 0.401)
				{
					return -10000000000d;
				}
				else if (x > 1.3333)
				{
					return 0.0;
				}

				return -(50 / x - 15);
			}
		}

		/// <summary>
		/// The function to calculate the potential value for the target position.
		/// </summary>
		private static readonly UnivariateRealFunction TARGET_DISTANCE_POTENTIAL_FUNCTION = new UnivariateRealFunctionAnonymousInnerClassHelper2();

		private class UnivariateRealFunctionAnonymousInnerClassHelper2 : UnivariateRealFunction
		{
			public UnivariateRealFunctionAnonymousInnerClassHelper2()
			{
			}

			//public override double Value(double x)
            public double Value(double x)
			{
				if (x == 0.0)
				{
					return 0.0;
				}
				return 4000 / (x + 1);
			}
		}

		/// <summary>
		/// The function to calculate the potential value for obstacles.
		/// </summary>
		private static readonly UnivariateRealFunction OBSTACLE_DISTANCE_POTENTIAL_FUNCTION = new UnivariateRealFunctionAnonymousInnerClassHelper3();

		private class UnivariateRealFunctionAnonymousInnerClassHelper3 : UnivariateRealFunction
		{
			public UnivariateRealFunctionAnonymousInnerClassHelper3()
			{
			}

			//public override double Value(double x)
            public double Value(double x)
			{
				if (x <= 0.21)
				{
					return double.NaN;
				}
				else if (x > 0.25)
				{
					return 0.0;
				}
				return -20 / x + 15;
			}
		}

		/// <summary>
		/// The agent associated with this behavior control.
		/// </summary>
		//private readonly AbstractAgent agent;
        private readonly SpatialAgent agent;

        private readonly PerceptionUnit perceptionUnit;

		/// <summary>
		/// The obstacle potential field.
		/// </summary>
		private readonly PotentialField obstacleField = new SimplePotentialField();

		/// <summary>
		/// Creates a new obstacle avoidance behavior. This behavior modifies the movement vector so that the agent
		/// does not collide with an obstacle.
		/// </summary>
		/// <param name="agent"> the agent for this behavior </param>
		//public ObstacleAvoidanceBehavior(AbstractAgent agent)
        public ObstacleAvoidanceBehavior(SpatialAgent agent, PerceptionUnit perceptionUnit)
		{
			this.agent = agent;
            this.perceptionUnit = perceptionUnit;

			// creating the static obstacle field
			//IList<SimulationObject> obstacles = ((VisionSensor) agent.Environment.VisionSensor).ObstaclesAsObjectList;
            var rawObstacleData = perceptionUnit.GetData((int)InformationTypes.Obstacles).Data;
            var obstacles = (List<Obstacle>)rawObstacleData;

			//foreach (SimulationObject obstacle in obstacles)
            foreach (Obstacle obstacle in obstacles)
			{
				//if (!obstacle.Id.StartsWith("floor", StringComparison.Ordinal))
				//{
					//obstacleField.addEmitter(new CuboidEmitter(obstacle.Position, (Vector3D) obstacle.Bounds, OBSTACLE_DISTANCE_POTENTIAL_FUNCTION));
				//}
                Vector position = obstacle.GetPosition();
                Vector bounds = obstacle.GetDimension();
                obstacleField.AddEmitter(new CuboidEmitter(position, bounds, OBSTACLE_DISTANCE_POTENTIAL_FUNCTION));
			}
		}

		public Vector ModifyMovementVector(Vector targetPosition, Vector currentPipelineVector)
		{
			PotentialField agentField = new SimplePotentialField();

			//IList<SimulationObject> perceptedAgents = ((VisionSensor) agent.Environment.VisionSensor).AgentsAsObjectList;
            var rawPedestrianData = perceptionUnit.GetData((int)InformationTypes.Pedestrians).Data;
            IList<Pedestrian> pedestrians = (List<Pedestrian>)rawPedestrianData;

			//foreach (SimulationObject so in perceptedAgents)
            foreach (Pedestrian ped in pedestrians)
			{
				if (!ped.Id.Equals(agent.Id))
				{
					//agentField.addEmitter(new PointEmitter(so.calcCurrentPosition(agent.Environment.CurrentSimulationTime + 20), AGENT_DISTANCE_POTENTIAL_FUNCTION));
                    #warning Old "WALK" code seems looking at the future positions of agents here!
                    Vector position = ped.GetPosition();
                    agentField.AddEmitter(new PointEmitter(position, AGENT_DISTANCE_POTENTIAL_FUNCTION));
				}
			}
			agentField.AddEmitter(new PointEmitter(targetPosition, TARGET_DISTANCE_POTENTIAL_FUNCTION));

			PotentialFieldCollection potentialFieldCollection = new PotentialFieldCollection();
			potentialFieldCollection.Add(obstacleField);
			potentialFieldCollection.Add(agentField);

			//Vector3D currentPosition = agent.Environment.CurrentPosition;
            Vector currentPosition = agent.GetPosition();

			Vector bestDirection = currentPipelineVector;
			double lastPotential = double.NegativeInfinity;

			// try several directions for the best potential
			for (double angle = -Math.PI / 2; angle <= Math.PI / 2; angle += Math.PI / 8)
			{
                //Vector tempDirection = new Vector(currentPipelineVector.X * (float)Math.Cos(angle) - currentPipelineVector.Z * (float)Math.Sin(angle), currentPipelineVector.Y, currentPipelineVector.X * (float)Math.Sin(angle) + currentPipelineVector.Z * (float)Math.Cos(angle));
                Vector tempDirection = new Vector(currentPipelineVector.X * (float)Math.Cos(angle) - currentPipelineVector.Y * (float)Math.Sin(angle), currentPipelineVector.X * (float)Math.Sin(angle) + currentPipelineVector.Y * (float)Math.Cos(angle), currentPipelineVector.Z);

				//Vector3D position = currentPosition.add(0.2, tempDirection.normalize());
                Vector position = currentPosition + (0.2f * tempDirection.GetNormalVector());

				double potential = potentialFieldCollection.CalculatePotential(position);

				if (!double.IsNaN(potential) && potential > lastPotential)
				{
					lastPotential = potential;
					bestDirection = tempDirection;
				}
			}

			// obstacles everywhere, no movement
			if (double.IsInfinity(lastPotential))
			{
				//bestDirection = Vector3D.ZERO;
                bestDirection = new Vector(0, 0, 0);
			}

			return bestDirection;
		}

	}

}