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

        [SetUp]
        public void initOpenCL()
        {

            //            System.IO.Directory.CreateDirectory(@"C:\Users\PhilippK\Desktop\OpenCL_DebugLogs\logs");
            //            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Users\PhilippK\Desktop\OpenCL_DebugLogs\logs");
            //            foreach (FileInfo file in di.GetFiles())
            //            {
            //                file.Delete();
            //            }
            //            foreach (DirectoryInfo dir in di.GetDirectories())
            //            {
            //                dir.Delete(true);
            //            }

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
            //int[] numOfSortValues = new[] { 16,1 << 10, 1 << 15, 100, 10000};//, 100000, 500000};
            List<int> numOfSortValues = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                numOfSortValues.Add(1 << 16);
            }
            //uint[] data = new uint[] { 0xa39, 0xdd1, 0x2671, 0x2da, 0x37a, 0x138, 0x2385, 0x26ba, 0x78c, 0x138f, 0x1041, 0x1ff4, 0x236e, 0x1d99, 0x44b, 0x1568, 0x210c, 0x225, 0x91a, 0x158f, 0x867, 0x19d8, 0x2081, 0x17a6, 0x1129, 0x4d0, 0x16da, 0x18b7, 0x166, 0x87e, 0x12b6, 0xb0, 0x2420, 0x5d8, 0x23cf, 0xadd, 0xca6, 0x1acf, 0x23b9, 0x1df9, 0x357, 0x11a3, 0x7d0, 0x123e, 0x53d, 0xf13, 0x16c1, 0xe3f, 0x1c1a, 0x0 };


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
        public void TestPerfomanceParallelMovement()
        {


            const int agentCount = 1000;
            const int ticks = 10;

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
            esc.Commit();
            Debug.WriteLine
                ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();




                    Debug.WriteLine("Collisions: " + _collisionCnt);
                    Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");
        }

        [Test]
        public void TestCollisionCase()
        {


            const int agentCount = 16;
            const int ticks = 10;

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

            a1 = new TestSpatialEntity(4, 4);
            a1.Shape = new Circle(20, 20, 7);
            agents.Add(a1);
            esc.Add(a1, EnvironmentDelegate);


            esc.Commit();
            Debug.WriteLine
                ("Time for adding " + agentCount + " agents:" + initTime.ElapsedMilliseconds + "ms");

            Stopwatch moveTime = Stopwatch.StartNew();
            Random random = new Random();

            Debug.WriteLine("InvolvedEntities: " + _collisionCnt);
            Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");

            /*    int collisions = 0;
                int movementSucess = 0;
                for (int i = 0; i < ticks; i++)
                {
                    Stopwatch tickTime = Stopwatch.StartNew();
                    Parallel.ForEach
                        (agents,
                            agent => {
                                if (esc.Move(agent.AgentGuid, new Vector3(random.Next(-2, 2), 0, random.Next(-2, 2)))
                                {
                                    movementSucess++;
                                }
                                else
                                {
                                    collisions++;
                                }
                            });

                    noSqlEsc.CommitTick();

                    Debug.WriteLine("Tick " + i + " required " + tickTime.ElapsedMilliseconds + "ms");
                }*/
            /*            Debug.WriteLine("InvolvedEntities: " + collisions);
                        Debug.WriteLine("Movement succeeded: " + movementSucess);
                        Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");

                        Debug.WriteLine("InvolvedEntities: " + collisions);
                        Debug.WriteLine("Movement succeeded: " + movementSucess);
                        Debug.WriteLine("Complete movement time: " + moveTime.ElapsedMilliseconds + "ms");*/
        }

    }
}
