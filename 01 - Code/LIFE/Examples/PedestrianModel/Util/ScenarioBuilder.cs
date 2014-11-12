using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using PedestrianModel.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Util
{
    class ScenarioBuilder
    {
        public enum ScenarioTypes { Test, Density, Bottleneck }

        public static void CreateTestScenario(IExecution exec, IEnvironment env, int pedestrianCount)
        {
            // Obstacle with center (10,15) going from x=9.5 to x=10.5 and y=10 to y=20
            var obsPosition = new Vector(10f, 15f);
            var obsDimension = new Vector(1f, 10f, 0.4f); // same height as pedestrians
            var obsDirection = new Direction();
            obsDirection.SetPitch(0f);
            obsDirection.SetYaw(0f);

            // OBSTACLES HAVE TO BE CREATED BEFORE THE AGENTS!
            new Obstacle(exec, env, obsPosition, obsDimension, obsDirection);

            var random = new Random();
            // WALK agents are 0.4m x 0.4m x 0.4m
            var pedDimension = new Vector(0.4f, 0.4f, 0.4f);
            var pedDirection = new Direction();
            pedDirection.SetPitch(0f);
            pedDirection.SetYaw(0f);

            for (var i = 0; i < pedestrianCount; i++)
            {
                // Random position between (0,10) and (9,20)
                var startPos = new Vector((float)random.NextDouble() * 9, (float)random.NextDouble() * 10 + 10f);
                // Random position between (11,10) and (20,20)
                var targetPos = new Vector((float)random.NextDouble() * 9 + 11f, (float)random.NextDouble() * 10 + 10f);
                new Pedestrian(exec, env, "sim0", startPos, pedDimension, pedDirection, targetPos);
            }
        }

        public static void CreateDensityScenario(IExecution exec, IEnvironment env, int pedestrianCount)
        {
            // top wall
            new Obstacle(exec, env, new Vector(20f, -0.025f, 0), new Vector(40f, 0.05f, 0.4f), new Direction());
            // bottom wall
            new Obstacle(exec, env, new Vector(20f, 10.025f, 0), new Vector(40f, 0.05f, 0.4f), new Direction());
            // left wall
            new Obstacle(exec, env, new Vector(-0.025f, 5f, 0), new Vector(0.05f, 10f, 0.4f), new Direction());
        }

        public static void CreateBottleneckScenario(IExecution exec, IEnvironment env, int pedestrianCount)
        {
            float wallWidth = 0.05f;
            float wallCorrection = wallWidth / 2f;
            float maxY = 10f;
            float connectionCenterY = 5f;
            float bottleneckLength = 0.4f;

            float bottleneckWidth = 0.4f;
                        
            // walls of left room, connection and right room
            // left room, top wall
            new Obstacle(exec, env, new Vector(5f, -0.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // connection, top wall
            float conTopY = connectionCenterY - (bottleneckWidth / 2f) - wallCorrection;
            new Obstacle(exec, env, new Vector(10.2f, conTopY, 0), new Vector(bottleneckLength, 0.05f, 0.4f), new Direction());
            // right room, top wall
            new Obstacle(exec, env, new Vector(15.4f, -0.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // left room, bottom wall
            new Obstacle(exec, env, new Vector(5f, 10.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // connection, bottom wall
            float conBotY = connectionCenterY + (bottleneckWidth / 2f) + wallCorrection;
            new Obstacle(exec, env, new Vector(10.2f, conBotY, 0), new Vector(bottleneckLength, 0.05f, 0.4f), new Direction());
            // right room, bottom wall
            new Obstacle(exec, env, new Vector(15.4f, 10.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());

            // left room, left wall
            new Obstacle(exec, env, new Vector(-0.025f, 5f, 0), new Vector(0.05f, 10f, 0.4f), new Direction());
            // left room, right wall above exit
            float sideWallWidthY = connectionCenterY - (bottleneckWidth / 2f);
            float lrrwaeY = sideWallWidthY / 2f;
            new Obstacle(exec, env, new Vector(10.025f, lrrwaeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // left room, right wall below exit
            float lrrwbeY = maxY - lrrwaeY;
            new Obstacle(exec, env, new Vector(10.025f, lrrwbeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());

            // right room, left wall above entrance
            new Obstacle(exec, env, new Vector(10.375f, lrrwaeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // right room, left wall below entrance
            new Obstacle(exec, env, new Vector(10.375f, lrrwbeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // right room, right wall above exit
            new Obstacle(exec, env, new Vector(20.425f, lrrwaeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // right room, right wall below exit
            new Obstacle(exec, env, new Vector(20.425f, lrrwbeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
        }

    }
}
