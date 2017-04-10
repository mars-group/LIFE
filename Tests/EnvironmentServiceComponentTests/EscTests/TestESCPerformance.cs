using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using EnvironmentServiceComponentTests.Entities;
using LIFE.Components.ESC.Implementation;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.EscTests
{
    [Ignore("Ignored for better build time on CI Server")]
    public class TestEscPerformance
    {
        [Test] // Do a number of insertion tests to measure general performance.
        public void PerformanceTestAddingEntities()
        {
            // Test series.
            int[] tests = {1000, 2000, 4000, 8000, 16000, 32000, 64000};
            const int envWidth = 1000; // Important: Ensure that width*height
            const int envHeight = 1000; // is sufficient to store max. number of agents!

            // Perform tests and output elapsed time.
            for (var t = 0; t < tests.Length; t++)
                foreach (var esc in EnvironmentManager.GetAll())
                {
                    var noSqlEsc = esc as NoSqlESC;
                    if (noSqlEsc != null)
                    {
                        var stopwatch = Stopwatch.StartNew();

                        for (var i = 0; i < tests[t]; i++)
                        {
                            var b1 = BoundingBox.GenerateByDimension
                                (Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
                            var t1 = new TestSpatialEntity(b1);
                            Assert.IsTrue(esc.Add(t1, new Vector3(-25, 31)));
                        }
                        noSqlEsc.CommitTick();

                        Console.WriteLine
                        ("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t"
                         + esc.GetType().Name);
                        stopwatch.Restart(); // Reset stopwatch.
                    }
                    else
                    {
                        var stopwatch = Stopwatch.StartNew();

                        for (var i = 0; i < tests[t]; i++)
                        {
                            var b1 = BoundingBox.GenerateByDimension
                                (Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
                            var t1 = new TestSpatialEntity(b1);
                            Assert.IsTrue(esc.Add(t1, new Vector3(i % envWidth, (double) i / envHeight)));
                        }
                        Console.WriteLine
                        ("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t"
                         + esc.GetType().Name);
                        stopwatch.Restart(); // Reset stopwatch.
                    }
                }
        }

        [Test] // Do a number of insertion tests to measure general performance.
        public void PerformanceTestParallelAddingEntities()
        {
            // Test series.
            int[] tests = {1000, 2000, 4000, 8000, 16000, 32000, 64000};
            var random = new Random();
            // Perform tests and output elapsed time.
            for (var t = 0; t < tests.Length; t++)
                foreach (var esc in EnvironmentManager.GetAll())
                {
                    var noSqlEsc = esc as NoSqlESC;
                    if (noSqlEsc != null)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        Parallel.For
                        (0,
                            tests[t],
                            i =>
                            {
                                var b1 = new Cuboid
                                (
                                    new Vector3(0.000008931761343, 0, 0.000008931761343),
                                    new Vector3(31.1 + random.NextDouble(), 0, -21.23 + random.NextDouble())
                                );
//                                    var b1 = BoundingBox.GenerateByDimension(Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
                                var t1 = new TestSpatialEntity(b1);
                                Assert.IsTrue(esc.Add(t1, new Vector3(-25, 31)));
                            });


                        noSqlEsc.CommitTick();


                        Console.WriteLine
                        ("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t" +
                         esc.GetType().Name);
                        stopwatch.Restart(); // Reset stopwatch.
                    }
                    else
                    {
                        var stopwatch = Stopwatch.StartNew();
                        Parallel.For
                        (0,
                            tests[t],
                            i =>
                            {
                                var b1 = new Cuboid
                                (
                                    new Vector3(0.000008931761343, 0, 0.000008931761343),
                                    new Vector3(31.1 + random.NextDouble(), 0, -21.23 + random.NextDouble())
                                );
                                var t1 = new TestSpatialEntity(b1);
                                //TODO was ist mir größer werdendem Raum new Vector3(i, i)
                                Assert.IsTrue(esc.Add(t1, new Vector3(random.NextDouble(), random.NextDouble())));
                            });

                        Console.WriteLine
                        ("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t" +
                         esc.GetType().Name);
                        stopwatch.Restart(); // Reset stopwatch.
                    }
                }
        }

        [Test] // Do a number of insertion tests to measure general performance.
        public void PerformanceTestParallelExploreEntities()
        {
            // Test series.
            int[] tests = {1400000};

            var random = new Random();
            // Perform tests and output elapsed time.
            for (var t = 0; t < tests.Length; t++)
                foreach (var esc in EnvironmentManager.GetAll())
                {
                    var noSqlEsc = esc as NoSqlESC;
                    if (noSqlEsc != null)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        Parallel.For
                        (0,
                            tests[t],
                            i =>
                            {
                                var position = new Vector3(31.1 + random.NextDouble(), 0, -21.23 + random.NextDouble());
                                var b1 = new Cuboid
                                (
                                    new Vector3(0.000008931761343, 0, 0.000008931761343),
                                    position
                                );
                                //                                    var b1 = BoundingBox.GenerateByDimension(Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
                                var t1 = new TestSpatialEntity(b1);
                                Assert.IsTrue(esc.Add(t1, position));
                            });


                        noSqlEsc.CommitTick();


                        Console.WriteLine
                        ("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t" +
                         esc.GetType().Name);
                        stopwatch.Restart(); // Reset stopwatch.
                    }
                    else
                    {
                        Parallel.For
                        (0,
                            tests[t],
                            i =>
                            {
                                var position = new Vector3(31.1 + random.NextDouble(), 0, -21.23 + random.NextDouble());
                                var b1 = new Sphere
                                (
                                    position,
                                    GetDecimalDegreesByMeters(7000)
                                );

                                var t1 = new TestSpatialEntity(b1);
                                esc.Add(t1, position);
                            });

                        var exploreShapes = new Sphere[14500];
                        Parallel.For(0, 14500,
                            i =>
                            {
                                var lat = 31.1 + random.NextDouble();
                                var lon = -21.23 + random.NextDouble();
                                exploreShapes[i] =
                                    new Sphere(new Vector3(lat, 0, lon), GetDecimalDegreesByMeters(7000));
                            });

                        var stopwatch = Stopwatch.StartNew();
                        Console.WriteLine("Start exploring...");
                        Parallel.For(0, 14500, i => esc.Explore(exploreShapes[i], 3));

                        Console.WriteLine
                        ("Explored [" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t" +
                         esc.GetType().Name);
                        stopwatch.Restart(); // Reset stopwatch.
                    }
                }
        }

        private static double GetDecimalDegreesByMeters(double distanceInMeters, double distanceOfOneArcSecond = 31.1)
        {
            var arcSeconds = distanceInMeters / distanceOfOneArcSecond;
            return arcSeconds / 60 / 60;
        }


        [Test] // Do a number of insertion tests to measure general performance.
        public void PerformanceTestAddingEntitiesAtRandomPosition()
        {
            // Test series.
            int[] tests = {1000, 2000, 4000, 8000, 16000, 32000, 64000};

            // Perform tests and output elapsed time.
            for (var t = 0; t < tests.Length; t++)
                foreach (var esc in EnvironmentManager.GetAll())
                {
                    var stopwatch = Stopwatch.StartNew();

                    for (var i = 0; i < tests[t]; i++)
                    {
                        var b1 = BoundingBox.GenerateByDimension
                            (Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
                        var t1 = new TestSpatialEntity(b1);
                        Assert.IsTrue(esc.AddWithRandomPosition(t1, Vector3.Zero, new Vector3(i, i), false));
                    }
                    var noSqlEsc = esc as NoSqlESC;
                    if (noSqlEsc != null) noSqlEsc.CommitTick();
                    Console.WriteLine
                        ("[" + tests[t] + " agents]: " + stopwatch.ElapsedMilliseconds + " ms\t" + esc.GetType().Name);
                    stopwatch.Restart(); // Reset stopwatch.
                }
        }

        [Test]
        public void TestPerfomanceMovement()
        {
            foreach (var esc in EnvironmentManager.GetAll())
            {
                var noSqlEsc = esc as NoSqlESC;
                if (noSqlEsc != null)
                {
                    Console.WriteLine("\n" + esc.GetType().Name);
                    const int agentCount = 30000;
                    const int ticks = 10;

                    var maxEnvDim = Math.Sqrt(agentCount) * 1.7;
                    var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
                    Console.WriteLine("Maximal environment dimension: " + maxEnvVector);

                    var initTime = Stopwatch.StartNew();
                    var agents = new List<ISpatialEntity>();
                    for (var tick = 0; tick < agentCount; tick++)
                    {
                        ISpatialEntity a1 = new TestSpatialEntity(1, 1);
                        agents.Add(a1);
                        Assert.True(esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false));
                    }
                    Console.WriteLine
                        ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

                    var moveTime = Stopwatch.StartNew();
                    var random = new Random();

                    var collisions = 0;
                    var movementSucess = 0;
                    for (var i = 0; i < ticks; i++)
                    {
                        var tickTime = Stopwatch.StartNew();
                        foreach (var agent in agents)
                            if (esc.Move(agent, new Vector3(random.Next(-2, 2), random.Next(-2, 2))).Success)
                                movementSucess++;
                            else collisions++;
                        noSqlEsc.CommitTick();
                        Console.WriteLine("Tick " + i + " required " + tickTime.ElapsedMilliseconds + "ms");
                    }
                    Console.WriteLine("Collisions: " + collisions);
                    Console.WriteLine("Movement succeeded: " + movementSucess);
                    Console.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
                }
                else
                {
                    Console.WriteLine("\n" + esc.GetType().Name);
                    const int agentCount = 100000;
                    const int ticks = 10;

                    var maxEnvDim = Math.Sqrt(agentCount) * 1.7;
                    var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
                    Console.WriteLine("Maximal environment dimension: " + maxEnvVector);

                    var initTime = Stopwatch.StartNew();
                    var agents = new List<ISpatialEntity>();
                    for (var tick = 0; tick < agentCount; tick++)
                    {
                        ISpatialEntity a1 = new TestSpatialEntity(1, 1);
                        agents.Add(a1);
                        Assert.True(esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false));
                    }
                    Console.WriteLine
                        ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

                    var moveTime = Stopwatch.StartNew();
                    var random = new Random();

                    var collisions = 0;
                    var movementSucess = 0;
                    for (var i = 0; i < ticks; i++)
                    {
                        var tickTime = Stopwatch.StartNew();
                        foreach (var agent in agents)
                            if (esc.Move(agent, new Vector3(random.Next(-2, 2), random.Next(-2, 2))).Success)
                                movementSucess++;
                            else collisions++;
                        Console.WriteLine("Tick " + i + " required " + tickTime.ElapsedMilliseconds + "ms");
                    }
                    Console.WriteLine("Collisions: " + collisions);
                    Console.WriteLine("Movement succeeded: " + movementSucess);
                    Console.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
                }
            }
        }

        [Test]
        public void TestPerfomanceParallelMovement()
        {
            foreach (var esc in EnvironmentManager.GetAll())
            {
                var noSqlEsc = esc as NoSqlESC;
                if (noSqlEsc != null)
                {
                    Console.WriteLine("\n" + esc.GetType().Name);
                    const int agentCount = 30000;
                    const int ticks = 10;

                    var maxEnvDim = Math.Sqrt(agentCount) * 1.7;
                    var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
                    Console.WriteLine("Maximal environment dimension: " + maxEnvVector);

                    var initTime = Stopwatch.StartNew();
                    var agents = new ConcurrentBag<ISpatialEntity>();

                    Parallel.For
                    (0,
                        agentCount,
                        t =>
                        {
                            ISpatialEntity a1 = new TestSpatialEntity(1, 1);
                            agents.Add(a1);
                            Assert.True(esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false));
                        });

                    Console.WriteLine
                        ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

                    var moveTime = Stopwatch.StartNew();
                    var random = new Random();

                    var collisions = 0;
                    var movementSucess = 0;
                    for (var i = 0; i < ticks; i++)
                    {
                        var tickTime = Stopwatch.StartNew();
                        Parallel.ForEach
                        (agents,
                            agent =>
                            {
                                if (esc.Move(agent, new Vector3(random.Next(-2, 2), 0, random.Next(-2, 2))).Success)
                                    movementSucess++;
                                else collisions++;
                            });

                        noSqlEsc.CommitTick();

                        Console.WriteLine("Tick " + i + " required " + tickTime.ElapsedMilliseconds + "ms");
                    }
                    Console.WriteLine("Collisions: " + collisions);
                    Console.WriteLine("Movement succeeded: " + movementSucess);
                    Console.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
                }
                else
                {
                    Console.WriteLine("\n" + esc.GetType().Name);
                    const int agentCount = 30000;
                    const int ticks = 10;

                    var maxEnvDim = Math.Sqrt(agentCount) * 1.7;
                    var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
                    Console.WriteLine("Maximal environment dimension: " + maxEnvVector);

                    var initTime = Stopwatch.StartNew();
                    var agents = new ConcurrentBag<ISpatialEntity>();

                    Parallel.For
                    (0,
                        agentCount,
                        t =>
                        {
                            ISpatialEntity a1 = new TestSpatialEntity(1, 1);
                            agents.Add(a1);
                            Assert.True(esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false));
                        });

                    Console.WriteLine
                        ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

                    var moveTime = Stopwatch.StartNew();
                    var random = new Random();

                    var collisions = 0;
                    var movementSucess = 0;
                    for (var i = 0; i < ticks; i++)
                    {
                        var tickTime = Stopwatch.StartNew();
                        Parallel.ForEach
                        (agents,
                            agent =>
                            {
                                if (esc.Move(agent, new Vector3(random.Next(-2, 2), random.Next(-2, 2))).Success)
                                    movementSucess++;
                                else collisions++;
                            });

                        Console.WriteLine("Tick " + i + " required " + tickTime.ElapsedMilliseconds + "ms");
                    }
                    Console.WriteLine("Collisions: " + collisions);
                    Console.WriteLine("Movement succeeded: " + movementSucess);
                    Console.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
                }
            }
        }
    }
}