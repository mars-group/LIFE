using System;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using LayerAPI.Spatial;
using PedestrianModel.Agents;
using PedestrianModel.Util.Math;

namespace PedestrianModel.Util {

    internal class ScenarioBuilder {
        private static readonly Random Random = new Random();

        public static void CreateDensityScenario(IExecution exec, IEnvironmentOld env, int pedestrianCount) {
            // top wall
            new Obstacle(exec, env, new Vector(20d, -0.025d, 0), new Vector(40d, 0.05d, 0.4d), new Direction());
            // bottom wall
            new Obstacle(exec, env, new Vector(20d, 10.025d, 0), new Vector(40d, 0.05d, 0.4d), new Direction());
            // left wall
            new Obstacle(exec, env, new Vector(-0.025d, 5d, 0), new Vector(0.05d, 10d, 0.4d), new Direction());

            double startMinX = 0.2d;
            double startMaxX = 9.8d;
            double startMinY = 0.2d;
            double startMaxY = 9.8d;

            double targetMinX = 28d;
            double targetMaxX = 30d;
            double targetMinY = 0.2d;
            double targetMaxY = 9.8d;

            for (int i = 0; i < pedestrianCount; i++) {
                double maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;

                Vector targetPos = new Vector
                    (Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                if (Config.UsesESC) {
                    Vector minPos = new Vector(startMinX, startMinY);
                    Vector maxPos = new Vector(startMaxX, startMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
                else {
                    Vector startPos = new Vector
                        (Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                            Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            startPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
            }
        }

        public static void CreateBottleneckScenario
            (IExecution exec, IEnvironmentOld env, int pedestrianCount, double bottleneckWidth) {
            double wallWidth = 0.05d;
            double wallCorrection = wallWidth/2d;
            double maxY = 10d;
            double connectionCenterY = 5d;
            double bottleneckLength = 0.4d;

            // walls of left room, connection and right room
            // left room, top wall
            new Obstacle(exec, env, new Vector(5d, -0.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());
            // connection, top wall
            double conTopY = connectionCenterY - (bottleneckWidth / 2d) - wallCorrection;
            new Obstacle(exec, env, new Vector(10.2d, conTopY, 0), new Vector(bottleneckLength, 0.05d, 0.4d), new Direction());
            // right room, top wall
            new Obstacle(exec, env, new Vector(15.4d, -0.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());
            // left room, bottom wall
            new Obstacle(exec, env, new Vector(5d, 10.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());
            // connection, bottom wall
            double conBotY = connectionCenterY + (bottleneckWidth / 2d) + wallCorrection;
            new Obstacle(exec, env, new Vector(10.2d, conBotY, 0), new Vector(bottleneckLength, 0.05d, 0.4d), new Direction());
            // right room, bottom wall
            new Obstacle(exec, env, new Vector(15.4d, 10.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());

            // left room, left wall
            new Obstacle(exec, env, new Vector(-0.025d, 5d, 0), new Vector(0.05d, 10d, 0.4d), new Direction());
            // left room, right wall above exit
            double sideWallWidthY = connectionCenterY - (bottleneckWidth/2d) - wallWidth;
            double lrrwaeY = sideWallWidthY/2d;
            new Obstacle(exec, env, new Vector(10.025d, lrrwaeY, 0), new Vector(0.05d, sideWallWidthY, 0.4d), new Direction());
            // left room, right wall below exit
            double lrrwbeY = maxY - lrrwaeY;
            new Obstacle(exec, env, new Vector(10.025d, lrrwbeY, 0), new Vector(0.05d, sideWallWidthY, 0.4d), new Direction());

            // right room, left wall above entrance
            new Obstacle
                (exec, env, new Vector(10.375d, lrrwaeY, 0), new Vector(0.05d, sideWallWidthY, 0.4d), new Direction());
            // right room, left wall below entrance
            new Obstacle
                (exec, env, new Vector(10.375d, lrrwbeY, 0), new Vector(0.05d, sideWallWidthY, 0.4d), new Direction());
            // right room, right wall above exit
            new Obstacle
                (exec, env, new Vector(20.425d, lrrwaeY, 0), new Vector(0.05d, sideWallWidthY, 0.4d), new Direction());
            // right room, right wall below exit
            new Obstacle(exec, env, new Vector(20.425d, lrrwbeY, 0), new Vector(0.05d, sideWallWidthY, 0.4d), new Direction());

            double startMinX = 0.2d;
            double startMaxX = 4.8d;
            double startMinY = 0.2d;
            double startMaxY = 9.8d;

            double targetMinX = 28d;
            double targetMaxX = 30d;
            double targetMinY = 3d;
            double targetMaxY = 7d;

            for (int i = 0; i < pedestrianCount; i++) {
                double maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;

                Vector targetPos = new Vector
                    (Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                if (Config.UsesESC) {
                    Vector minPos = new Vector(startMinX, startMinY);
                    Vector maxPos = new Vector(startMaxX, startMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
                else {
                    Vector startPos = new Vector
                        (Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                            Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            startPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
            }
        }

        public static void CreateRimeaScenario(IExecution exec, IEnvironmentOld env, int pedestrianCount) {
            // walls of left room, connection and right room
            // left room, top wall
            new Obstacle(exec, env, new Vector(5d, -0.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());
            // connection, top wall
            new Obstacle(exec, env, new Vector(12.5d, 4.475d, 0), new Vector(4.9d, 0.05d, 0.4d), new Direction());
            // right room, top wall
            new Obstacle(exec, env, new Vector(20d, -0.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());
            // left room, bottom wall
            new Obstacle(exec, env, new Vector(5d, 10.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());
            // connection, bottom wall
            new Obstacle(exec, env, new Vector(12.5d, 5.525d, 0), new Vector(4.9d, 0.05d, 0.4d), new Direction());
            // right room, bottom wall
            new Obstacle(exec, env, new Vector(20d, 10.025d, 0), new Vector(10d, 0.05d, 0.4d), new Direction());

            // left room, left wall
            new Obstacle(exec, env, new Vector(-0.025d, 5d, 0), new Vector(0.05d, 10d, 0.4d), new Direction());
            // left room, right wall above exit
            new Obstacle(exec, env, new Vector(10.025d, 2.25d, 0), new Vector(0.05d, 4.5d, 0.4d), new Direction());
            // left room, right wall below exit
            new Obstacle(exec, env, new Vector(10.025d, 7.75d, 0), new Vector(0.05d, 4.5d, 0.4d), new Direction());

            // right room, left wall above entrance
            new Obstacle(exec, env, new Vector(14.975d, 2.25d, 0), new Vector(0.05d, 4.5d, 0.4d), new Direction());
            // right room, left wall below entrance
            new Obstacle(exec, env, new Vector(14.975d, 7.75d, 0), new Vector(0.05d, 4.5d, 0.4d), new Direction());
            // right room, right wall above exit
            new Obstacle(exec, env, new Vector(25.025d, 2.25d, 0), new Vector(0.05d, 4.5d, 0.4d), new Direction());
            // right room, right wall below exit
            new Obstacle(exec, env, new Vector(25.025d, 7.75d, 0), new Vector(0.05d, 4.5d, 0.4d), new Direction());

            double startMinX = 0.2d;
            double startMaxX = 4.8d;
            double startMinY = 0.2d;
            double startMaxY = 9.8d;

            double targetMinX = 28d;
            double targetMaxX = 30d;
            double targetMinY = 3d;
            double targetMaxY = 7d;

            for (int i = 0; i < pedestrianCount; i++) {
                double maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;

                Vector targetPos = new Vector
                    (Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                if (Config.UsesESC) {
                    Vector minPos = new Vector(startMinX, startMinY);
                    Vector maxPos = new Vector(startMaxX, startMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
                else {
                    Vector startPos = new Vector
                        (Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                            Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            startPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
            }
        }

        public static void CreateUShapeScenario
            (IExecution exec, IEnvironmentOld env, int pedestrianCount, double wallLength) {
            // move the old scenario positions to positive positions
            double xOffset = 100d;
            double yOffset = 100d;

            double wallWidth = 0.2d;
            double wallHeight = 0.4d;

            Vector topWallCenter = new Vector(0 + xOffset, -wallLength/2d - wallWidth/2d + yOffset, 0);
            Vector topWallSize = new Vector(wallLength, wallWidth, wallHeight);
            new Obstacle(exec, env, topWallCenter, topWallSize, new Direction());

            Vector rightWallCenter = new Vector(wallLength/2d + wallWidth/2d + xOffset, 0 + yOffset, 0);
            Vector rightWallSize = new Vector(wallWidth, wallLength, wallHeight);
            new Obstacle(exec, env, rightWallCenter, rightWallSize, new Direction());

            Vector bottomWallCenter = new Vector(0 + xOffset, wallLength/2d + wallWidth/2d + yOffset, 0);
            Vector bottomWallSize = new Vector(wallLength, wallWidth, wallHeight);
            new Obstacle(exec, env, bottomWallCenter, bottomWallSize, new Direction());

            double startMinX = -5d + xOffset;
            double startMaxX = -4d + xOffset;
            double startMinY = -0.5d + yOffset;
            double startMaxY = 0.5d + yOffset;

            double targetMinX = 51.5d + xOffset;
            double targetMaxX = 52.5d + xOffset;
            double targetMinY = -0.5d + yOffset;
            double targetMaxY = 0.5d + yOffset;

            for (int i = 0; i < pedestrianCount; i++) {
                double maxVelocity;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity = Config.MaxVelocity;

                Vector targetPos = new Vector
                    (Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                if (Config.UsesESC) {
                    Vector minPos = new Vector(startMinX, startMinY);
                    Vector maxPos = new Vector(startMaxX, startMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
                else {
                    Vector startPos = new Vector
                        (Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                            Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            startPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity);
                }
            }
        }

        public static void CreateOscillationScenario
            (IExecution exec, IEnvironmentOld env, int pedestrianCount, double bottleneckWidth) {
            // move the old scenario positions to positive positions
            double xOffset = 10d;
            double yOffset = 10d;

            double wallWidth = 0.05d;
            double wallLengthX = 20d;
            double wallLengthY = 10d;
            double wallHeight = 0.4d;

            Vector topWallCenter = new Vector(0 + xOffset, -wallLengthY/2d - wallWidth/2d + yOffset, 0);
            Vector topWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, topWallCenter, topWallSize, new Direction());

            Vector bottomWallCenter = new Vector(0 + xOffset, wallLengthY/2d + wallWidth/2d + yOffset, 0);
            Vector bottomWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, bottomWallCenter, bottomWallSize, new Direction());

            Vector leftWallCenter = new Vector(-wallLengthX/2d - wallWidth/2d + xOffset, 0 + yOffset, 0);
            Vector leftWallSize = new Vector(wallWidth, wallLengthY, wallHeight);
            new Obstacle(exec, env, leftWallCenter, leftWallSize, new Direction());

            Vector rightWallCenter = new Vector(wallLengthX/2d + wallWidth/2d + xOffset, 0 + yOffset, 0);
            Vector rightWallSize = new Vector(wallWidth, wallLengthY, wallHeight);
            new Obstacle(exec, env, rightWallCenter, rightWallSize, new Direction());

            double sideWallLengthY = wallLengthY/2d - (bottleneckWidth/2d);

            Vector upperBottleneckWallCenter = new Vector
                (0 + xOffset, -sideWallLengthY/2d - bottleneckWidth/2d + yOffset, 0);
            Vector upperBottleneckWallSize = new Vector(wallWidth, sideWallLengthY, wallHeight);
            new Obstacle(exec, env, upperBottleneckWallCenter, upperBottleneckWallSize, new Direction());

            Vector lowerBottleneckWallCenter = new Vector
                (0 + xOffset, sideWallLengthY/2d + bottleneckWidth/2d + yOffset, 0);
            Vector lowerBottleneckWallSize = new Vector(wallWidth, sideWallLengthY, wallHeight);
            new Obstacle(exec, env, lowerBottleneckWallCenter, lowerBottleneckWallSize, new Direction());

            double startMinX = -7.5d + xOffset;
            double startMaxX = -2.5d + xOffset;
            double startMinY = -2.5d + yOffset;
            double startMaxY = 2.5d + yOffset;

            double targetMinX = 2.5d + xOffset;
            double targetMaxX = 7.5d + xOffset;
            double targetMinY = -2.5d + yOffset;
            double targetMaxY = 2.5d + yOffset;

            for (int i = 0; i < pedestrianCount/2; i++) {
                Vector startPos = new Vector
                    (Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    (Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                double maxVelocity1;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity1 = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity1 = Config.MaxVelocity;

                if (Config.UsesESC) {
                    Vector minPos = new Vector(startMinX, startMinY);
                    Vector maxPos = new Vector(startMaxX, startMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity1);
                }
                else {
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            startPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity1);
                }

                double maxVelocity2;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity2 = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity2 = Config.MaxVelocity;

                if (Config.UsesESC) {
                    Vector minPos = new Vector(targetMinX, targetMinY);
                    Vector maxPos = new Vector(targetMaxX, targetMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            startPos,
                            maxVelocity2);
                }
                else {
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            targetPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            startPos,
                            maxVelocity2);
                }
            }
        }

        public static void CreateLaneScenario
            (IExecution exec, IEnvironmentOld env, int pedestrianCount, double laneWidth) {
            // move the old scenario positions to positive positions
            double xOffset = 10d;
            double yOffset = 10d;

            double wallWidth = 0.05d;
            double wallLengthX = 20d;
            double wallHeight = 2d;

            Vector topWallCenter = new Vector(0 + xOffset, -laneWidth/2d - wallWidth/2d + yOffset, 0);
            Vector topWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, topWallCenter, topWallSize, new Direction());

            Vector bottomWallCenter = new Vector(0 + xOffset, laneWidth/2d + wallWidth/2d + yOffset, 0);
            Vector bottomWallSize = new Vector(wallLengthX, wallWidth, wallHeight);
            new Obstacle(exec, env, bottomWallCenter, bottomWallSize, new Direction());

            Vector leftWallCenter = new Vector(-wallLengthX/2d - wallWidth/2d + xOffset, 0 + yOffset, 0);
            Vector leftWallSize = new Vector(wallWidth, laneWidth, wallHeight);
            new Obstacle(exec, env, leftWallCenter, leftWallSize, new Direction());

            Vector rightWallCenter = new Vector(wallLengthX/2d + wallWidth/2d + xOffset, 0 + yOffset, 0);
            Vector rightWallSize = new Vector(wallWidth, laneWidth, wallHeight);
            new Obstacle(exec, env, rightWallCenter, rightWallSize, new Direction());

            double startMinX = -7.5d + xOffset;
            double startMaxX = -2.5d + xOffset;
            double startMinY = -1.05d + yOffset;
            double startMaxY = 1.05d + yOffset;

            double targetMinX = 2.5d + xOffset;
            double targetMaxX = 7.5d + xOffset;
            double targetMinY = -1.05d + yOffset;
            double targetMaxY = 1.05d + yOffset;

            for (int i = 0; i < pedestrianCount/2; i++) {
                Vector startPos = new Vector
                    (Random.NextDouble()*(startMaxX - startMinX) + startMinX,
                        Random.NextDouble()*(startMaxY - startMinY) + startMinY);
                Vector targetPos = new Vector
                    (Random.NextDouble()*(targetMaxX - targetMinX) + targetMinX,
                        Random.NextDouble()*(targetMaxY - targetMinY) + targetMinY);

                double maxVelocity1;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity1 = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity1 = Config.MaxVelocity;

                if (Config.UsesESC) {
                    Vector minPos = new Vector(startMinX, startMinY);
                    Vector maxPos = new Vector(startMaxX, startMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity1);
                }
                else {
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            startPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            targetPos,
                            maxVelocity1);
                }

                double maxVelocity2;
                if (Config.IsGaussianNormalDistribution)
                    maxVelocity2 = Distribution.NormalGaussian(Config.MaxVelocity, Config.StandardDeviation);
                else maxVelocity2 = Config.MaxVelocity;

                if (Config.UsesESC) {
                    Vector minPos = new Vector(targetMinX, targetMinY);
                    Vector maxPos = new Vector(targetMaxX, targetMaxY);
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            minPos,
                            maxPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            startPos,
                            maxVelocity2);
                }
                else {
                    new Pedestrian
                        (exec,
                            env,
                            "sim0",
                            targetPos,
                            Config.PedestrianDimensions,
                            new Direction(),
                            startPos,
                            maxVelocity2);
                }
            }
        }

        public static void CreateScenario(IExecution exec, IEnvironmentOld env, ScenarioType scenario) {
            switch (scenario) {
                case ScenarioType.Lane250Cm:
                    CreateLaneScenario(exec, env, 50, 2.5d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 240d;
                    Config.VisualizationOffsetY = -60d;
                    break;
                case ScenarioType.Lane500Cm:
                    CreateLaneScenario(exec, env, 50, 5d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 240d;
                    Config.VisualizationOffsetY = -60d;
                    break;
                case ScenarioType.UShape500Cm:
                    CreateUShapeScenario(exec, env, 1, 5d);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = -2350d;
                    Config.VisualizationOffsetY = -2650d;
                    break;
                case ScenarioType.UShape10000Cm:
                    CreateUShapeScenario(exec, env, 1, 100d);
                    Config.VisualizationZoom = 5d;
                    Config.VisualizationOffsetX = 100d;
                    Config.VisualizationOffsetY = -160d;
                    break;
                case ScenarioType.Oscillation50Cm:
                    CreateOscillationScenario(exec, env, 50, 0.5d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 230d;
                    Config.VisualizationOffsetY = -60d;
                    break;
                case ScenarioType.Oscillation100Cm:
                    CreateOscillationScenario(exec, env, 50, 1d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 230d;
                    Config.VisualizationOffsetY = -60d;
                    break;
                case ScenarioType.Oscillation200Cm:
                    CreateOscillationScenario(exec, env, 50, 2d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 230d;
                    Config.VisualizationOffsetY = -60d;
                    break;
                case ScenarioType.Density50Agents:
                    CreateDensityScenario(exec, env, 50);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = 25d;
                    Config.VisualizationOffsetY = 175d;
                    break;
                case ScenarioType.Density100Agents:
                    CreateDensityScenario(exec, env, 100);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = 25d;
                    Config.VisualizationOffsetY = 175d;
                    break;
                case ScenarioType.Density200Agents:
                    CreateDensityScenario(exec, env, 200);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = 25d;
                    Config.VisualizationOffsetY = 175d;
                    break;
                case ScenarioType.Density300Agents:
                    CreateDensityScenario(exec, env, 300);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = 25d;
                    Config.VisualizationOffsetY = 175d;
                    break;
                case ScenarioType.Density400Agents:
                    CreateDensityScenario(exec, env, 400);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = 25d;
                    Config.VisualizationOffsetY = 175d;
                    break;
                case ScenarioType.Density500Agents:
                    CreateDensityScenario(exec, env, 500);
                    Config.VisualizationZoom = 30d;
                    Config.VisualizationOffsetX = 25d;
                    Config.VisualizationOffsetY = 175d;
                    break;
                case ScenarioType.Bottleneck50Agents:
                    CreateRimeaScenario(exec, env, 50);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 125d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck75Agents:
                    CreateRimeaScenario(exec, env, 75);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 125d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck100Agents:
                    CreateRimeaScenario(exec, env, 100);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 125d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck125Agents:
                    CreateRimeaScenario(exec, env, 125);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 125d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck150Agents:
                    CreateRimeaScenario(exec, env, 150);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 125d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck40Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.4d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck50Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.5d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck60Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.6d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck70Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.7d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck80Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.8d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck90Cm:
                    CreateBottleneckScenario(exec, env, 100, 0.9d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck100Cm:
                    CreateBottleneckScenario(exec, env, 100, 1d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck120Cm:
                    CreateBottleneckScenario(exec, env, 100, 1.2d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck140Cm:
                    CreateBottleneckScenario(exec, env, 100, 1.4d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
                case ScenarioType.Bottleneck160Cm:
                    CreateBottleneckScenario(exec, env, 100, 1.6d);
                    Config.VisualizationZoom = 40d;
                    Config.VisualizationOffsetX = 225d;
                    Config.VisualizationOffsetY = 140d;
                    break;
            }
        }
    }

}