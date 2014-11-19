using System;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using PedestrianModel.Agents;
using PedestrianModel.Util.Math;

namespace PedestrianModel.Util {

    internal class ScenarioBuilder {
        private static readonly Random Random = new Random();

        public static void CreateDensityScenario(IExecution exec, IEnvironment env, int pedestrianCount) {
            // top wall
            new Obstacle(exec, env, new Vector(20f, -0.025f, 0), new Vector(40f, 0.05f, 0.4f), new Direction());
            // bottom wall
            new Obstacle(exec, env, new Vector(20f, 10.025f, 0), new Vector(40f, 0.05f, 0.4f), new Direction());
            // left wall
            new Obstacle(exec, env, new Vector(-0.025f, 5f, 0), new Vector(0.05f, 10f, 0.4f), new Direction());

            float startMinX = 0.2f;
            float startMaxX = 9.8f;
            float startMinY = 0.2f;
            float startMaxY = 9.8f;

            float targetMinX = 28f;
            float targetMaxX = 30f;
            float targetMinY = 0.2f;
            float targetMaxY = 9.8f;

            for (int i = 0; i < pedestrianCount; i++) {
                Vector startPos = new Vector
                    ((float) Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        (float) Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    ((float) Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        (float) Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                float maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", startPos, Config.PedestrianDimensions, new Direction(), targetPos, maxVelocity);
            }
        }

        public static void CreateBottleneckScenario
            (IExecution exec, IEnvironment env, int pedestrianCount, float bottleneckWidth) {
            float wallWidth = 0.05f;
            float wallCorrection = wallWidth/2f;
            float maxY = 10f;
            float connectionCenterY = 5f;
            float bottleneckLength = 0.4f;

            // walls of left room, connection and right room
            // left room, top wall
            new Obstacle(exec, env, new Vector(5f, -0.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // connection, top wall
            float conTopY = connectionCenterY - (bottleneckWidth/2f) - wallCorrection;
            new Obstacle
                (exec, env, new Vector(10.2f, conTopY, 0), new Vector(bottleneckLength, 0.05f, 0.4f), new Direction());
            // right room, top wall
            new Obstacle(exec, env, new Vector(15.4f, -0.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // left room, bottom wall
            new Obstacle(exec, env, new Vector(5f, 10.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // connection, bottom wall
            float conBotY = connectionCenterY + (bottleneckWidth/2f) + wallCorrection;
            new Obstacle
                (exec, env, new Vector(10.2f, conBotY, 0), new Vector(bottleneckLength, 0.05f, 0.4f), new Direction());
            // right room, bottom wall
            new Obstacle(exec, env, new Vector(15.4f, 10.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());

            // left room, left wall
            new Obstacle(exec, env, new Vector(-0.025f, 5f, 0), new Vector(0.05f, 10f, 0.4f), new Direction());
            // left room, right wall above exit
            float sideWallWidthY = connectionCenterY - (bottleneckWidth/2f);
            float lrrwaeY = sideWallWidthY/2f;
            new Obstacle
                (exec, env, new Vector(10.025f, lrrwaeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // left room, right wall below exit
            float lrrwbeY = maxY - lrrwaeY;
            new Obstacle
                (exec, env, new Vector(10.025f, lrrwbeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());

            // right room, left wall above entrance
            new Obstacle
                (exec, env, new Vector(10.375f, lrrwaeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // right room, left wall below entrance
            new Obstacle
                (exec, env, new Vector(10.375f, lrrwbeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // right room, right wall above exit
            new Obstacle
                (exec, env, new Vector(20.425f, lrrwaeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());
            // right room, right wall below exit
            new Obstacle
                (exec, env, new Vector(20.425f, lrrwbeY, 0), new Vector(0.05f, sideWallWidthY, 0.4f), new Direction());

            float startMinX = 0.2f;
            float startMaxX = 4.8f;
            float startMinY = 0.2f;
            float startMaxY = 9.8f;

            float targetMinX = 28f;
            float targetMaxX = 30f;
            float targetMinY = 3f;
            float targetMaxY = 7f;

            for (int i = 0; i < pedestrianCount; i++) {
                Vector startPos = new Vector
                    ((float) Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        (float) Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    ((float) Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        (float) Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                float maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", startPos, Config.PedestrianDimensions, new Direction(), targetPos, maxVelocity);
            }
        }

        public static void CreateRimeaScenario(IExecution exec, IEnvironment env, int pedestrianCount) {
            // walls of left room, connection and right room
            // left room, top wall
            new Obstacle(exec, env, new Vector(5f, -0.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // connection, top wall
            new Obstacle(exec, env, new Vector(12.5f, 4.475f, 0), new Vector(5f, 0.05f, 0.4f), new Direction());
            // right room, top wall
            new Obstacle(exec, env, new Vector(20f, -0.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // left room, bottom wall
            new Obstacle(exec, env, new Vector(5f, 10.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());
            // connection, bottom wall
            new Obstacle(exec, env, new Vector(12.5f, 5.525f, 0), new Vector(5f, 0.05f, 0.4f), new Direction());
            // right room, bottom wall
            new Obstacle(exec, env, new Vector(20f, 10.025f, 0), new Vector(10f, 0.05f, 0.4f), new Direction());

            // left room, left wall
            new Obstacle(exec, env, new Vector(-0.025f, 5f, 0), new Vector(0.05f, 10f, 0.4f), new Direction());
            // left room, right wall above exit
            new Obstacle(exec, env, new Vector(10.025f, 2.25f, 0), new Vector(0.05f, 4.5f, 0.4f), new Direction());
            // left room, right wall below exit
            new Obstacle(exec, env, new Vector(10.025f, 7.75f, 0), new Vector(0.05f, 4.5f, 0.4f), new Direction());

            // right room, left wall above entrance
            new Obstacle(exec, env, new Vector(14.975f, 2.25f, 0), new Vector(0.05f, 4.5f, 0.4f), new Direction());
            // right room, left wall below entrance
            new Obstacle(exec, env, new Vector(14.975f, 7.75f, 0), new Vector(0.05f, 4.5f, 0.4f), new Direction());
            // right room, right wall above exit
            new Obstacle(exec, env, new Vector(25.025f, 2.25f, 0), new Vector(0.05f, 4.5f, 0.4f), new Direction());
            // right room, right wall below exit
            new Obstacle(exec, env, new Vector(25.025f, 7.75f, 0), new Vector(0.05f, 4.5f, 0.4f), new Direction());

            float startMinX = 0.2f;
            float startMaxX = 4.8f;
            float startMinY = 0.2f;
            float startMaxY = 9.8f;

            float targetMinX = 28f;
            float targetMaxX = 30f;
            float targetMinY = 3f;
            float targetMaxY = 7f;

            for (int i = 0; i < pedestrianCount; i++) {
                Vector startPos = new Vector
                    ((float) Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        (float) Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    ((float) Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        (float) Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                float maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", startPos, Config.PedestrianDimensions, new Direction(), targetPos, maxVelocity);
            }
        }

        public static void CreateUShapeScenario
            (IExecution exec, IEnvironment env, int pedestrianCount, float wallLength) {
            // move the old scenario positions to positive positions
            float xOffset = 100;
            float yOffset = 100;

            float wallWidth = 0.2f;
            float wallHeight = 0.4f;

            Vector topWallCenter = new Vector(0 + xOffset, -wallLength/2f - wallWidth/2f + yOffset, 0);
            Vector topWallSize = new Vector(wallLength, wallWidth, wallHeight);
            new Obstacle(exec, env, topWallCenter, topWallSize, new Direction());

            Vector rightWallCenter = new Vector(wallLength/2f + wallWidth/2f + xOffset, 0 + yOffset, 0);
            Vector rightWallSize = new Vector(wallWidth, wallLength, wallHeight);
            new Obstacle(exec, env, rightWallCenter, rightWallSize, new Direction());

            Vector bottomWallCenter = new Vector(0 + xOffset, wallLength/2f + wallWidth/2f + yOffset, 0);
            Vector bottomWallSize = new Vector(wallLength, wallWidth, wallHeight);
            new Obstacle(exec, env, bottomWallCenter, bottomWallSize, new Direction());

            float startMinX = -5f + xOffset;
            float startMaxX = -4f + xOffset;
            float startMinY = -0.5f + yOffset;
            float startMaxY = 0.5f + yOffset;

            float targetMinX = 51.5f + xOffset;
            float targetMaxX = 52.5f + xOffset;
            float targetMinY = -0.5f + yOffset;
            float targetMaxY = 0.5f + yOffset;

            for (int i = 0; i < pedestrianCount; i++) {
                Vector startPos = new Vector
                    ((float) Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        (float) Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    ((float) Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        (float) Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                float maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", startPos, Config.PedestrianDimensions, new Direction(), targetPos, maxVelocity);
            }
        }

        public static void CreateOscillationScenario
            (IExecution exec, IEnvironment env, int pedestrianCount, float bottleneckWidth) {
            // move the old scenario positions to positive positions
            float xOffset = 10;
            float yOffset = 10;

            float wallWidth = 0.05f;
            float wallLengthX = 20f;
            float wallLengthY = 10f;
            float wallHeight = 0.4f;

            Vector topWallCenter = new Vector(0 + xOffset, -wallLengthY/2f - wallWidth/2f + yOffset, 0);
            Vector topWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, topWallCenter, topWallSize, new Direction());

            Vector bottomWallCenter = new Vector(0 + xOffset, wallLengthY/2f + wallWidth/2f + yOffset, 0);
            Vector bottomWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, bottomWallCenter, bottomWallSize, new Direction());

            Vector leftWallCenter = new Vector(-wallLengthX/2f - wallWidth/2f + xOffset, 0 + yOffset, 0);
            Vector leftWallSize = new Vector(wallWidth, wallLengthY, wallHeight);
            new Obstacle(exec, env, leftWallCenter, leftWallSize, new Direction());

            Vector rightWallCenter = new Vector(wallLengthX/2f + wallWidth/2f + xOffset, 0 + yOffset, 0);
            Vector rightWallSize = new Vector(wallWidth, wallLengthY, wallHeight);
            new Obstacle(exec, env, rightWallCenter, rightWallSize, new Direction());

            float sideWallLengthY = wallLengthY/2f - (bottleneckWidth/2f);

            Vector upperBottleneckWallCenter = new Vector
                (0 + xOffset, -sideWallLengthY/2f - bottleneckWidth/2f + yOffset, 0);
            Vector upperBottleneckWallSize = new Vector(wallWidth, sideWallLengthY, wallHeight);
            new Obstacle(exec, env, upperBottleneckWallCenter, upperBottleneckWallSize, new Direction());

            Vector lowerBottleneckWallCenter = new Vector
                (0 + xOffset, sideWallLengthY/2f + bottleneckWidth/2f + yOffset, 0);
            Vector lowerBottleneckWallSize = new Vector(wallWidth, sideWallLengthY, wallHeight);
            new Obstacle(exec, env, lowerBottleneckWallCenter, lowerBottleneckWallSize, new Direction());

            float startMinX = -7.5f + xOffset;
            float startMaxX = -2.5f + xOffset;
            float startMinY = -2.5f + yOffset;
            float startMaxY = 2.5f + yOffset;

            float targetMinX = 2.5f + xOffset;
            float targetMaxX = 7.5f + xOffset;
            float targetMinY = -2.5f + yOffset;
            float targetMaxY = 2.5f + yOffset;

            for (int i = 0; i < pedestrianCount/2; i++) {
                Vector startPos = new Vector
                    ((float) Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        (float) Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    ((float) Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        (float) Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                float maxVelocity1;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity1 = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity1 = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", startPos, Config.PedestrianDimensions, new Direction(), targetPos, maxVelocity1);

                float maxVelocity2;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity2 = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity2 = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", targetPos, Config.PedestrianDimensions, new Direction(), startPos, maxVelocity2);
            }
        }

        public static void CreateLaneScenario(IExecution exec, IEnvironment env, int pedestrianCount, float laneWidth) {
            // move the old scenario positions to positive positions
            float xOffset = 10;
            float yOffset = 10;

            float wallWidth = 0.05f;
            float wallLengthX = 20f;
            float wallHeight = 2f;

            Vector topWallCenter = new Vector(0 + xOffset, -laneWidth/2f - wallWidth/2f + yOffset, 0);
            Vector topWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, topWallCenter, topWallSize, new Direction());

            Vector bottomWallCenter = new Vector(0 + xOffset, laneWidth/2f + wallWidth/2f + yOffset, 0);
            Vector bottomWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, bottomWallCenter, bottomWallSize, new Direction());

            Vector leftWallCenter = new Vector(-wallLengthX/2f - wallWidth/2f + xOffset, 0 + yOffset, 0);
            Vector leftWallSize = new Vector(wallWidth, laneWidth, wallHeight);
            new Obstacle(exec, env, leftWallCenter, leftWallSize, new Direction());

            Vector rightWallCenter = new Vector(wallLengthX/2f + wallWidth/2f + xOffset, 0 + yOffset, 0);
            Vector rightWallSize = new Vector(wallWidth, laneWidth, wallHeight);
            new Obstacle(exec, env, rightWallCenter, rightWallSize, new Direction());

            float startMinX = -7.5f + xOffset;
            float startMaxX = -2.5f + xOffset;
            float startMinY = -1.05f + yOffset;
            float startMaxY = 1.05f + yOffset;

            float targetMinX = 2.5f + xOffset;
            float targetMaxX = 7.5f + xOffset;
            float targetMinY = -1.05f + yOffset;
            float targetMaxY = 1.05f + yOffset;

            for (int i = 0; i < pedestrianCount/2; i++) {
                Vector startPos = new Vector
                    ((float) Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        (float) Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    ((float) Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        (float) Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                float maxVelocity1;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity1 = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity1 = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", startPos, Config.PedestrianDimensions, new Direction(), targetPos, maxVelocity1);

                float maxVelocity2;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity2 = (float) Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity2 = Config.MaxVelocity;
                new Pedestrian
                    (exec, env, "sim0", targetPos, Config.PedestrianDimensions, new Direction(), startPos, maxVelocity2);
            }
        }

        public static void CreateScenario(IExecution exec, IEnvironment env, ScenarioType scenario) {
            switch (scenario) {
                case ScenarioType.Lane250Cm:
                    CreateLaneScenario(exec, env, 50, 2.5f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 240;
                    Config.VisualizationOffsetY = -60;
                    break;
                case ScenarioType.Lane500Cm:
                    CreateLaneScenario(exec, env, 50, 5f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 240;
                    Config.VisualizationOffsetY = -60;
                    break;
                case ScenarioType.UShape500Cm:
                    CreateUShapeScenario(exec, env, 1, 5f);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = -2350;
                    Config.VisualizationOffsetY = -2650;
                    break;
                case ScenarioType.UShape10000Cm:
                    CreateUShapeScenario(exec, env, 1, 100f);
                    Config.VisualizationZoom = 5;
                    Config.VisualizationOffsetX = 100;
                    Config.VisualizationOffsetY = -160;
                    break;
                case ScenarioType.Oscillation50Cm:
                    CreateOscillationScenario(exec, env, 50, 0.5f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 625;
                    Config.VisualizationOffsetY = 340;
                    break;
                case ScenarioType.Oscillation100Cm:
                    CreateOscillationScenario(exec, env, 50, 1f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 230;
                    Config.VisualizationOffsetY = -60;
                    break;
                case ScenarioType.Oscillation200Cm:
                    CreateOscillationScenario(exec, env, 50, 2f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 625;
                    Config.VisualizationOffsetY = 340;
                    break;
                case ScenarioType.Density50Agents:
                    CreateDensityScenario(exec, env, 50);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = 25;
                    Config.VisualizationOffsetY = 175;
                    break;
                case ScenarioType.Density100Agents:
                    CreateDensityScenario(exec, env, 100);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = 25;
                    Config.VisualizationOffsetY = 175;
                    break;
                case ScenarioType.Density200Agents:
                    CreateDensityScenario(exec, env, 200);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = 25;
                    Config.VisualizationOffsetY = 175;
                    break;
                case ScenarioType.Density300Agents:
                    CreateDensityScenario(exec, env, 300);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = 25;
                    Config.VisualizationOffsetY = 175;
                    break;
                case ScenarioType.Density400Agents:
                    CreateDensityScenario(exec, env, 400);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = 25;
                    Config.VisualizationOffsetY = 175;
                    break;
                case ScenarioType.Density500Agents:
                    CreateDensityScenario(exec, env, 500);
                    Config.VisualizationZoom = 30;
                    Config.VisualizationOffsetX = 25;
                    Config.VisualizationOffsetY = 175;
                    break;
                case ScenarioType.Bottleneck50Agents:
                    CreateRimeaScenario(exec, env, 50);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 125;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck75Agents:
                    CreateRimeaScenario(exec, env, 75);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 125;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck100Agents:
                    CreateRimeaScenario(exec, env, 100);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 125;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck125Agents:
                    CreateRimeaScenario(exec, env, 125);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 125;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck150Agents:
                    CreateRimeaScenario(exec, env, 150);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 125;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck40Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.4f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck50Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.5f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck60Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.6f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck70Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.7f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck80Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.8f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck90Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.9f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck100Cm:
                    CreateBottleneckScenario(exec, env, 100, 1f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck120Cm:
                    CreateBottleneckScenario(exec, env, 100, 1.2f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck140Cm:
                    CreateBottleneckScenario(exec, env, 100, 1.4f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
                case ScenarioType.Bottleneck160Cm:
                    CreateBottleneckScenario(exec, env, 100, 1.6f);
                    Config.VisualizationZoom = 40;
                    Config.VisualizationOffsetX = 225;
                    Config.VisualizationOffsetY = 140;
                    break;
            }
        }
    }

}