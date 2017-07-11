using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloo;
using EnvironmentServiceComponent.SpatialAPI.Entities.Movement;
using EnvironmentServiceComponent.SpatialAPI.Environment;
using EnvironmentServiceComponentTests.Entities;
using GpuEnvironment.Helper;
using GpuEnvironment.Implementation;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.EscTests
{
    public class TestGpuESC
    {
        private ComputeDevice _device;
        private const string LOG_BASE_PATH = "";
        private const string LOG_FOLDER_NAME = "DEBUGLOGS";

        private ComputeContext cxGPUContext;
        private ComputeCommandQueue cqCommandQueue;
        private GPURadixSort sort;

        private MovementDelegate _movementDelegate;
        private int _delegateCallCount = 0;
        private int _collisionCnt = 0;

        private void EnvironmentDelegate(EnvironmentResult result, ISpatialEntity newPos)
        {
            _delegateCallCount++;
            if (result.InvolvedEntities.Any())
            {
                _collisionCnt++;
            }
        }

#if NO_GPU
        [SetUp]
        public void initOpenCL()
        {

            List<ComputeDevice> devicesList = new List<ComputeDevice>();


            foreach (ComputePlatform platform in ComputePlatform.Platforms)
            {

                Debug.WriteLine("Platform Name: " + platform.Name);
                Debug.WriteLine("Platform Profile: " + platform.Profile);
                Debug.WriteLine("Platform Vendor: " + platform.Vendor);
                ComputeContextPropertyList properties = new ComputeContextPropertyList(platform);
                if (platform.Name != "NVIDIA CUDA")
                {
                    continue;
                }
                cxGPUContext = new ComputeContext(ComputeDeviceTypes.Gpu, properties, null, IntPtr.Zero);
                devicesList.AddRange(cxGPUContext.Devices);
                //We will be looking only for GPU devices
                foreach (ComputeDevice device in cxGPUContext.Devices)
                {

                    Debug.WriteLine("Vendor: " + device.Vendor + " , " + device.Name);

                    Debug.WriteLine("Device: " + device.Type);
                    Debug.WriteLine("Workgroupsize: " + device.MaxWorkGroupSize);
                    Debug.WriteLine("Global Mem: " + device.GlobalMemorySize);
                    Debug.WriteLine("Local: " + device.LocalMemorySize);
                    Debug.WriteLine("CUS: " + device.MaxComputeUnits);
                    devicesList.Add(device);
                }
            }

            if (devicesList.Count <= 0)
            {
                Debug.WriteLine("No devices found.");
                return;
            }

            _device = devicesList[0];

            cqCommandQueue = new ComputeCommandQueue(cxGPUContext, _device, ComputeCommandQueueFlags.None);

            sort = new GPURadixSort(cqCommandQueue, cxGPUContext, _device);

        }




        public void TestSortWithRandomValues()
        {
            List<int> numOfSortValues = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                numOfSortValues.Add(1 << 16);
            }


            foreach (var numElements in numOfSortValues)
            {
                uint[] testKeys = new uint[numElements];
                ulong[] testValues = new ulong[numElements];
                uint[] testKeysOutput = new uint[numElements];
                ulong[] testValuesOutput = new ulong[numElements];
                Random rnd = new Random();
                Debug.WriteLine("\n New Sort test with {0} random values", numElements);
                Debug.WriteLine("Init:");
                for (uint i = 0; i < testKeys.Length; i++)
                {
                    uint tmp = (uint)rnd.Next((1 << 16) - 1);
                    testKeys[i] = tmp;
                    testValues[i] = tmp;
                    // Debug.WriteLine("Key: {0} value: {1}", testKeys[i], testValues[i]);
                }



                // create buffers 
                // Create Buffers
                ComputeBuffer<uint> cl_KeyMem = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, testKeys);
                ComputeBuffer<ulong> cl_ValueMem = new ComputeBuffer<ulong>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, testValues);
                ComputeBuffer<uint> cl_KeyOutput = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, testKeys.Length);

                ComputeEventList eventList = new ComputeEventList();

                sort.sortKeysValue(cl_KeyMem, cl_ValueMem, testKeys.Length);
                //                sort.sortKeysOnly(cl_KeyMem, cl_KeyOutput, testKeys.Length);
                cqCommandQueue.ReadFromBuffer(cl_KeyMem, ref testKeysOutput, true, eventList);
                cqCommandQueue.ReadFromBuffer(cl_ValueMem, ref testValuesOutput, true, eventList);


                string log = Path.Combine(GpuESC.LOG_BASE_PATH + "SortInAndOutput.txt");
                using (StreamWriter sw = File.AppendText(log))
                {
                    sw.WriteLine(log);
                }
                Array.Sort(testKeys);
                Debug.WriteLine("Sort finished");
                for (uint i = 0; i < testKeys.Length; i++)
                {
                    Assert.True(testKeysOutput[i] == testKeys[i], "keys not sorted");
                    Assert.True(testValuesOutput[i] == (uint)testKeys[i], "values not sorted");
                }
                // Assert.False(testKeys == testValues, "did not work");
                cl_KeyMem.Dispose();
                cl_ValueMem.Dispose();
                cl_KeyOutput.Dispose();
                Debug.WriteLine("Run with {0} random numbers finished without errors", numElements);

            }
        }


        [Test]
        public void TestPerfomanceParallelMovement3D()
        {


            const int agentCount = 200;
            const int ticks = 10;
            Debug.WriteLine(typeof(IAsyncEnvironment));
            double maxEnvDim = 10000;//Math.Sqrt(agentCount) * 1.7;
            var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
            Debug.WriteLine("Maximal environment dimension: " + maxEnvVector);

            Stopwatch initTime = Stopwatch.StartNew();
            var agents = new ConcurrentBag<ISpatialEntity>();
            var esc = new GpuESC(new Vector3(20, 20), maxEnvVector);
            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(14, 14);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });

            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(7, 7);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });
            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(4, 4);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });
            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(1, 1);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });
            esc.Commit();
            Debug.WriteLine
                ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();




            Debug.WriteLine("Collisions: " + _collisionCnt);
            Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
        }

        [Test]
        public void TestPerfomanceParallelMovement2D()
        {


            const int agentCount = 200;
            Debug.WriteLine(typeof(IAsyncEnvironment));
            double maxEnvDim = 10000;//Math.Sqrt(agentCount) * 1.7;
            var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
            Debug.WriteLine("Maximal environment dimension: " + maxEnvVector);

            Stopwatch initTime = Stopwatch.StartNew();
            var agents = new ConcurrentBag<ISpatialEntity>();
            var esc = new HierarchicalGpuESC(new Vector3(20, 20), maxEnvVector);
            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(14, 14);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });

            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(7, 7);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });
            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(4, 4);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });
            Parallel.For
            (0,
                agentCount / 4,
                t => {
                    ISpatialEntity a1 = new TestSpatialEntity(1, 1);
                    agents.Add(a1);
                    esc.AddWithRandomPosition(a1, Vector3.Zero, maxEnvVector, false, EnvironmentDelegate);
                });
            Assert.AreEqual(esc.Commit(),COMMIT_RESULT.OK);
            Debug.WriteLine
                ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();




                    Debug.WriteLine("Collisions: " + _collisionCnt);
                    Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
        }

        [Test]
        public void TestCollisionCase2D()
        {
            const int agentCount = 16;
            const int ticks = 10;
            _collisionCnt = 0;
            double maxEnvDim = 500;//Math.Sqrt(agentCount) * 1.7;
            var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
            Console.WriteLine("Maximal environment dimension: " + maxEnvVector);

            Stopwatch initTime = Stopwatch.StartNew();
            var agents = new ConcurrentBag<ISpatialEntity>();
            var esc = new HierarchicalGpuESC(new Vector3(20, 20), maxEnvVector);


            ISpatialEntity a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Circle(20, 20, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Circle(21, 21, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Circle(100, 105, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Circle(99, 104, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(7, 7);
            a1.Shape = new Circle(20, 20, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(7, 7);
            a1.Shape = new Circle(20, 20, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);


            esc.Commit();
            Debug.WriteLine
                ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();

            Debug.WriteLine("InvolvedEntities: " + _collisionCnt);

            Assert.AreEqual(6, _collisionCnt);
            Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
        }

        [Test]
        public void TestCollisionCase3D()
        {
            const int agentCount = 16;
            const int ticks = 10;
            _collisionCnt = 0;
            double maxEnvDim = 500;//Math.Sqrt(agentCount) * 1.7;
            var maxEnvVector = new Vector3(maxEnvDim, maxEnvDim);
            Console.WriteLine("Maximal environment dimension: " + maxEnvVector);

            Stopwatch initTime = Stopwatch.StartNew();
            var agents = new ConcurrentBag<ISpatialEntity>();
            var esc = new GpuESC(new Vector3(20, 20), maxEnvVector);


            ISpatialEntity a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Cuboid(new Vector3(5,5,5), new Vector3(14,14,14));
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Cuboid(new Vector3(5, 5, 5), new Vector3(14, 14, 14));

            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(14, 14);

            a1.Shape = new Cuboid(new Vector3(10,10,5), new Vector3(14,14,14));
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(14, 14);
            a1.Shape = new Cuboid(new Vector3(5, 5, 5), new Vector3(10, 10, 14));
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(7, 7);

            a1.Shape = new Cuboid(new Vector3(5,5,5), new Vector3(14,20,4));
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);

            a1 = new TestSpatialEntity(7, 7);
            a1.Shape = new Cuboid(new Vector3(5, 5, 5), new Vector3(14, 14, 14));
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);


            esc.Commit();
            Debug.WriteLine
                ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();

            Debug.WriteLine("InvolvedEntities: " + _collisionCnt);

            Assert.AreEqual(6, _collisionCnt);
            Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
        }
#endif
    }
}
