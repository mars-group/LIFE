using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Cloo;
using EnvironmentServiceComponent.Implementation;
using EnvironmentServiceComponent.SpatialAPI.Entities.Movement;
using GpuEnvironment.Helper;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace GpuEnvironment
{

    public class HierarchicalGpuESC : IAsyncEnvironment {

        public const string LOG_BASE_PATH = @"C:\Users\PhilippK\Desktop\OpenCL_DebugLogs\logs\";
//        public const string OPENCL_BASE_PATH = @"D:\Projects\environmentservicecomponent\LIFE Services\EnvironmentServiceComponent\Implementation\GPU-ESC\";

        private const int CELL_DATA_ELEMENT_SIZE = 8;
        private const int CELL_ID_ELEMENT_SIZE = 4;
        private const int GRID_FIELDS_PER_ELEMENT = 4;

        private bool DEBUG = false;

        private object _sync = new object();
        private static Random _random = new Random();
        // Auch Thread Blocks unter CUDA -> Gruppe von Threads mit gemeinsamen shared memory.
        private const int numBlocks = 4;
        private ulong actTick = 0;

        // Anzahl von WorkItems / Threads, die sich in einer Work-Group befinden
        private const int numThreadsPerBlock = 32;
        private const int numLvls = 5;


        private EventWaitHandle layerTickWait;
        private EventWaitHandle gpuActiveWait;
        private static volatile bool gpuActive;

        public const int NUM_BANKS = 16;
        public ComputeContext cxGPUContext { get; set; }
        public ComputeCommandQueue cqCommandQueue { get; set; }
        private ComputeDevice _device { get; set; }

        private GPURadixSort sort;
        // ObjId MoveDelegates from all methodcalls since the last commit
        private List<Tuple<uint, MovementDelegate>> m_AddDelegates;
        private List<Tuple<uint, MovementDelegate>> m_MoveDelegates;
        private List<Tuple<uint, ExploreDelegate>> m_ExploreDelegates;
        private List<Tuple<Guid, Action<ISpatialEntity>>> m_ActRemoves;

        private List<ExploreDelegate> m_ExploreAllDelegates;





        #region CollisionParameters

        private const ulong FLAG_NEW = (1L << 35);
        private const ulong FLAG_MOVE = (1L << 36);
        private const ulong FLAG_EXLORE = (1L << 37);
        private uint objIdGenBase = 0;
        private int exploreIdGenBase = Int32.MaxValue/2;

        private static int m_ActiveOperationCount = 0;



        public cl2DCollisionConstants _constants;

        private ComputeBuffer<uint>[] cl_lvlCellIds;
        private ComputeBuffer<ulong>[] cl_lvlCellData;

        // Kernel 1 and 2
        private ComputeBuffer<clShapeIdTupel> cl_shapeIdInputMem;
        private ComputeBuffer<clShapeIdTupel> cl_lvlShapeIdMem;
        private ComputeBuffer<ulong> cl_objIdMem;

        // Kernel 2
        private ComputeBuffer<uint> cl_lvlElements;
        // REMOVE MAYBE
        private ComputeBuffer<uint> cl_lvlTotalElements;
        private ComputeBuffer<CollisionCell2D>[] cl_DebugLvl2DPosList;

        // CreateCollisionSublists
        private ComputeBuffer<clCollisionSublist>[] cl_narrowCheckSublists;

        // CreateCollisionTuples
        private ComputeBuffer<CollisionTupel>[] cl_lvlCollisionCheckTuples;

        // CheckCollisions
        private ComputeBuffer<CollisionTupel>[] cl_lvlCollisonTuples;
        private ComputeBuffer<CollisionTupel>[] cl_lvlExporeTuples;
        private ComputeBuffer<clCircleShapeTupel>[] cl_lvlCollisonShapes;
        private ComputeBuffer<int>[] cl_lvlExploreElementIdx;

        // Used for the last 3 kernels as index for the resultdata 
        private ComputeBuffer<int>[] cl_sharedIdxMem;

        // Check if its worth to create a merged list

        public struct clShapeIdTupel {
            public ulong objId;
            public clCircleShapeObject objShape;
        }

        public struct clCircleShapeObject {
            public clPoint2D center;
            public float radius;
        }

        public struct clCircleShapeTupel {
            public clCircleShapeObject obj1;
            public clCircleShapeObject obj2;
        }

        public struct clCollisionSublist {
            public int startIdx;
            public short nHome;
            public short nPhant;
            public short iHome;
            public short objCnt;
        }

        public struct cl2DCollisionConstants {
            public uint numThreadsPerBlock;
            public uint numTotalElements;
            public uint numLvls;
            public uint cellArraySize;
            public float xMax;
            public float yMax;
            public float maxCellSizeX;
            public float maxCellSizeY;
            public uint xGridBoundary;
            public uint yGridBoundary;
            public uint numBlocks;
        }

        private List<clCircleShapeObject> _clShapeArray;
        private List<clShapeIdTupel> _clShapeIdArray;

        //shapes.
        private List<ulong> _clObjArray;
        private int[][] _sharedIdx;
        private bool[] _freeCellList;
        private int _sortElements;

        // Variables for reading the debug output
//        private ulong[] _debugReadInput;
        private uint[] _lvlElements;
        private uint[] _lvlTotalElements;
        private clShapeIdTupel[] _readLvlShapeElements;
        private uint[][] _readCellIds;
        private ulong[][] _readCellData;
        private CollisionCell2D[][] _readLvlCell2DPosList;
        private CollisionTupel[][] _readLvlCheckTupelList;
        private CollisionTupel[][] _readLvlCollisionList;
        private CollisionTupel[][] _readLvlExploreList;
        private int[][] _lvlExploreIdx;
        private clCollisionSublist[][] _readNarrowCheckList;
        private clCircleShapeTupel[][] _collisionShapeTupels;


        private ConcurrentDictionary<uint, clCircleShapeObject> _objIdClShapeMap;
        private ConcurrentDictionary<uint, ISpatialEntity> _objIdSpatialMap;// TODO: Maybe not needed
        private ConcurrentDictionary<Guid, uint> _spatialObjIdMap;
        private ConcurrentDictionary<uint, HashSet<uint>> _collisionMap;

        private ConcurrentDictionary<uint, clCircleShapeObject> _EnvFreshAddedObjs;
        private ConcurrentDictionary<uint, clCircleShapeObject> _EnvActEnvObjs;
        private ConcurrentDictionary<uint, clCircleShapeObject> _EnvExploreObjs;

        private List<uint> _objIdList;
        private List<clCircleShapeObject> _shapeList;
        private ConcurrentDictionary<uint, CollisionCell2D> _cellMap;

        ComputeKernel ckReorderElements;
        ComputeKernel ckCreateCellIdArray;
        ComputeKernel ckCreateNarrowCheckList;
        ComputeKernel ckCreateCollsionTuples;
        ComputeKernel ckCheckCollsions;


        private Vector3 _OuterBoundary;
        private Vector3 _CellSize;

        #endregion

        private bool checkBoundarys(Vector3 min, Vector3 max)
        {
            return min.X >= 0 && min.Y >= 0 && min.Z >= 0 && max.X <= _constants.xMax && max.Y <= _constants.yMax;
        }

        private uint getNextObjId()
        {
            lock ("s")
            {
                return objIdGenBase++;
            }

        }

        private bool IsValidShape(IShape obj)
        {
            //TODO: implement.....
            return true;

        }

        public HierarchicalGpuESC(Vector3 maxElementSize, Vector3 enviromentSize)
        {
            m_ActRemoves = new List<Tuple<Guid, Action<ISpatialEntity>>>();
            _EnvFreshAddedObjs = new ConcurrentDictionary<uint, clCircleShapeObject>();
            _EnvActEnvObjs = new ConcurrentDictionary<uint, clCircleShapeObject>();
            _EnvExploreObjs = new ConcurrentDictionary<uint, clCircleShapeObject>();
            m_ExploreDelegates = new List<Tuple<uint, ExploreDelegate>>();
            m_ExploreAllDelegates = new List<ExploreDelegate>();
            m_MoveDelegates = new List<Tuple<uint, MovementDelegate>>();
            m_AddDelegates = new List<Tuple<uint, MovementDelegate>>();
            _collisionMap = new ConcurrentDictionary<uint, HashSet<uint>>();
            _objIdClShapeMap = new ConcurrentDictionary<uint, clCircleShapeObject>();
            _shapeList = new List<clCircleShapeObject>();
            _objIdSpatialMap = new ConcurrentDictionary<uint, ISpatialEntity>();// TODO: Maybe not needed
            _spatialObjIdMap = new ConcurrentDictionary<Guid, uint>();
            _collisionMap = new ConcurrentDictionary<uint, HashSet<uint>>();
            _clShapeIdArray = new List<clShapeIdTupel>();
            //            _lvlElements = new uint[numLvls];
            //            _lvlTotalElements = new uint[numLvls];
            //            _lvlTotalElements.Initialize();
            //            _lvlElements.Initialize();


            _OuterBoundary = enviromentSize;
            _CellSize = maxElementSize;
            _cellMap = new ConcurrentDictionary<uint, CollisionCell2D>();
            _objIdList = new List<uint>();
            /*objValueList.Clear();/*#1#*/

            // List of free gridcells
            _freeCellList = new bool[1];//(int) ((m_OuterBoundary.X / m_CellSize.X) * (m_OuterBoundary.Y / m_CellSize.Y) * (m_OuterBoundary.Z / m_CellSize.Z) )];


            layerTickWait = new EventWaitHandle(false, EventResetMode.ManualReset, "addWait");
            gpuActiveWait = new EventWaitHandle(false, EventResetMode.ManualReset, "gpuActiveWait");

            gpuActive = new bool();
            gpuActive = false;
            InitOpenCl();
        }



        private Vector3 GetNextFreeCell(Vector3 min, Vector3 max)
        {
            Vector3 res = new Vector3();
//            uint xBegin = (uint)(min.X / _constants.cellSizeX);
//            uint yBegin = (uint)(min.Y / _constants.cellSizeY);
//            uint zBegin = (uint)(min.Z / _constants.cellSizeZ);
//            uint xEnd = (uint)(max.X / _constants.cellSizeX);
//            uint yEnd = (uint)(max.Y / _constants.cellSizeY);
//            uint zEnd = (uint)(max.Z / _constants.cellSizeZ);*/
//            /*  cellBegin.Y = min.Y / constants.cellSizeY;
//              cellBegin.Z = min.Z / constants.cellSizeZ;
//              cellEnd.X = max.X / constants.cellSizeX;
//              cellEnd.Y = max.Y / constants.cellSizeY;
//              cellEnd.Z = max.Z / constants.cellSizeZ;
//  */
//
//            for (uint x = xBegin; x < xEnd; x++)
//            {
//                for (uint y = yBegin; y < yEnd; y++)
//                {
//                    for (uint z = zBegin; z < zEnd; z++)
//                    {
//                        if (_freeCellList[
//                            z * _constants.xGridBoundary * _constants.yGridBoundary
//                            + y * _constants.xGridBoundary
//                            + x
//                            ])
//                        {
//                            res.X = x * _constants.cellSizeX + (_constants.cellSizeX / 2);
//                            res.Y = y * _constants.cellSizeY + (_constants.cellSizeY / 2);
//                            res.Z = z * _constants.cellSizeZ + (_constants.cellSizeZ / 2);
//                            _freeCellList[z * _constants.xGridBoundary * _constants.yGridBoundary + y * _constants.xGridBoundary + x] = false;
//                            return res;
//                        }
//                    }
//                }
//            }

            return Vector3.Null;
        }

        private void RemoveByObjId(uint objId)
        {
            lock (_sync)
            {
                _objIdList.Remove(objId);
            }
            uint tmp;
            while (!_spatialObjIdMap.TryRemove(_objIdSpatialMap[objId].AgentGuid, out tmp)) ;
            ISpatialEntity tmp2;
            clCircleShapeObject tmp3;
            _objIdSpatialMap.TryRemove((uint)objId, out tmp2);
           if(DEBUG) Debug.WriteLine("Removed  Agent by objId {0}", tmp2.AgentGuid);
 
            _objIdClShapeMap.TryRemove((uint)objId, out tmp3);
            //shapeList.Remove(objIdClShapeMap[(uint)objId]);

        }
//        private void CheckErr(ErrorCode err, string name)
//        {
//            if (err != ErrorCode.Success)
//            {
//                Debug.WriteLine("error: " + name + " (" + err.ToString() + ")");
//                Debug.WriteLine("error: " + name + " (" + err + ")");
//                using (StreamWriter sw = File.AppendText(LOG_BASE_PATH + @"OpenCLDebugLog.txt"))
//                {
//                    sw.WriteLine("error: " + name + " (" + err + ")");
//                }
//            }
//        }
        private void InitOpenCl()
        {
            if (DEBUG)
            {
               if(DEBUG) Debug.WriteLine(Path.Combine(LOG_BASE_PATH + "log.txt"));
                 if (File.Exists(Path.Combine(LOG_BASE_PATH + "log.txt")))
                {
                    File.Delete(Path.Combine(LOG_BASE_PATH + "log.txt"));
                }

                if (File.Exists(Path.Combine(LOG_BASE_PATH + "sortLog.txt")))
                {
                    File.Delete(Path.Combine(LOG_BASE_PATH + "sortLog.txt"));
                }
                if (File.Exists(Path.Combine(LOG_BASE_PATH + "OpenCLDebugLog.txt")))
                {
                    File.Delete(Path.Combine(LOG_BASE_PATH + "OpenCLDebugLog.txt"));
                }
            }

            _constants.numBlocks = numBlocks;
            _constants.numThreadsPerBlock = numThreadsPerBlock;
            _constants.xMax = (float)_OuterBoundary.X;
            _constants.yMax = (float)_OuterBoundary.Y;
            _constants.maxCellSizeX = (float)_CellSize.X;
            _constants.maxCellSizeY = (float)_CellSize.Y;
            _constants.xGridBoundary = (uint)(_OuterBoundary.X / _CellSize.X);
            _constants.yGridBoundary = (uint)(_OuterBoundary.Y / _CellSize.Y);
            _constants.numLvls = numLvls;
            _lvlElements = new uint[numLvls];

            List<ComputeDevice> devicesList = new List<ComputeDevice>();


            foreach (ComputePlatform platform in ComputePlatform.Platforms)
            {

                Console.WriteLine("Platform Name: " + platform.Name);
                Console.WriteLine("Platform Profile: " + platform.Profile);
                Console.WriteLine("Platform Vendor: " + platform.Vendor);
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

                    Console.WriteLine("Vendor: " + device.Vendor + " , " + device.Name);

                    Console.WriteLine("Device: " + device.Type);
                    Console.WriteLine("Workgroupsize: " + device.MaxWorkGroupSize);
                    Console.WriteLine("Global Mem: " + device.GlobalMemorySize);
                    Console.WriteLine("Local: " + device.LocalMemorySize);
                    Console.WriteLine("CUS: " + device.MaxComputeUnits);
                    devicesList.Add(device);
                }
            }

            if (devicesList.Count <= 0)
            {
                Console.WriteLine("No devices found.");
                return;
            }

            _device = devicesList[0];

            cqCommandQueue = new ComputeCommandQueue(cxGPUContext, _device, ComputeCommandQueueFlags.None);

            sort = new GPURadixSort(cqCommandQueue, cxGPUContext, _device);


            string programSource = GpuOpenClCode.CollisionDetection2DHierarchicalKernel;
            IntPtr[] progSize = new IntPtr[] { (IntPtr)programSource.Length };
            string flags = "-cl-fast-relaxed-math";

            ComputeProgram prog = new ComputeProgram(cxGPUContext, programSource);
            try
            {
                prog.Build(new List<ComputeDevice>() { _device }, flags, null, IntPtr.Zero);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            if (prog.GetBuildStatus(_device) != ComputeProgramBuildStatus.Success)
            {
                Console.WriteLine(prog.GetBuildLog(_device));
                throw new ArgumentException("UNABLE to build programm");
            }
            ckReorderElements = prog.CreateKernel("ReorderElements");
            ckCreateCellIdArray = prog.CreateKernel("CreateCellId");
            ckCreateNarrowCheckList = prog.CreateKernel("CreateNarrowCheckList");
            ckCreateCollsionTuples = prog.CreateKernel("CreateCollsionTuples");
            ckCheckCollsions = prog.CreateKernel("CheckCollsions");


            //
            //            string programSource = System.IO.File.ReadAllText(Path.Combine(OPENCL_BASE_PATH + "CollisionDetection2D.cl"));
            //            IntPtr[] progSize = new IntPtr[] { (IntPtr)programSource.Length };
            //            OpenCL.Net.Program clProgramCollision = Cl.CreateProgramWithSource(cxGPUContext, 1, new[] { programSource }, progSize,
            //                out error);
            //            CheckErr(error, "createProgramm");
            //            string flags = "-cl-fast-relaxed-math";
            //
            //            error = Cl.BuildProgram(clProgramCollision, 1, new[] { _device }, flags, null, IntPtr.Zero);
            //            CheckErr(error, "Cl.BuildProgram");
            //            //Check for any compilation errors
            //            if (Cl.GetProgramBuildInfo(clProgramCollision, _device, ProgramBuildInfo.Status, out error).CastTo<BuildStatus>()
            //                != BuildStatus.Success)
            //            {
            //                CheckErr(error, "Cl.GetProgramBuildInfo");
            //                Debug.WriteLine("Cl.GetProgramBuildInfo != Success");
            //                Debug.WriteLine(Cl.GetProgramBuildInfo(clProgramCollision, _device, ProgramBuildInfo.Log, out error));
            //                using (StreamWriter sw = File.AppendText(Path.Combine(LOG_BASE_PATH + "OpenCLDebugLog.txt")))
            //                {
            //                    sw.WriteLine(Cl.GetProgramBuildInfo(clProgramCollision, _device, ProgramBuildInfo.Log, out error));
            //                }
            //                return;
            //            }
            //            uint ciErrNum;
            //
            //
            //
            //            ckReorderElements = Cl.CreateKernel(clProgramCollision, "ReorderElements", out error);
            //            CheckErr(error, "Cl.CreateKernel");
            //            ckCreateCellIdArray = Cl.CreateKernel(clProgramCollision, "CreateCellId", out error);
            //            CheckErr(error, "Cl.CreateKernel");
            //            ckCreateNarrowCheckList = Cl.CreateKernel(clProgramCollision, "CreateNarrowCheckList", out error);
            //            CheckErr(error, "Cl.CreateKernel");
            //            ckCreateCollsionTuples = Cl.CreateKernel(clProgramCollision, "CreateCollsionTuples", out error);
            //            CheckErr(error, "Cl.CreateKernel");
            //            ckCheckCollsions = Cl.CreateKernel(clProgramCollision, "CheckCollsions", out error);
            //            CheckErr(error, "Cl.CreateKernel");
            //            _freeCellList = new bool[_constants.xGridBoundary * _constants.yGridBoundary ];
            //
            //      /*      cl_freeCellMem = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)((_freeCellList.Length > 0) ? _freeCellList.Length : 1 * Marshal.SizeOf(typeof(bool))), cl_sharedIdxMem,
            //                 out error);*/
            //            CheckErr(error, "Createbuffer");

        }





        public void Add(ISpatialEntity entity, MovementDelegate movementDelegate)
        {
            // First check boundarys
            Console.WriteLine("Added agent with id {0}", entity.AgentGuid);

            if (!checkBoundarys(entity.Shape.Bounds.LeftBottomFront, entity.Shape.Bounds.RightTopRear))
            {
                //                movementDelegate.Invoke(new EnvironmentResult());
                return;
            }
            bool res;
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            uint objId = getNextObjId();
            //ulong objData = objId | FLAG_NEW;
            lock (_sync)
            {
                m_AddDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
                _objIdList.Add(objId);
            }
            while (!_spatialObjIdMap.TryAdd(entity.AgentGuid, objId)) ;




            while (!_objIdSpatialMap.TryAdd(objId, entity)) ;

            clCircleShapeObject act = ConvertShapeToClShape(entity.Shape);
            while (!_objIdClShapeMap.TryAdd(objId, act)) ;
            _EnvFreshAddedObjs.TryAdd(objId, act);
            /*
                        shapeList.Add(act);
                        objIdClShapeMap.Add(objId, act);
                        */


            Interlocked.Decrement(ref m_ActiveOperationCount);

        }

        public void AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid, MovementDelegate movementDelegate)
        {
            //Debug.WriteLine("Added agent with id {0}", entity.AgentGuid);

            //Evaluate dimesions
            var shapeTmp = new Vector3(entity.Shape.Bounds.Length / 2, entity.Shape.Bounds.Height / 2, entity.Shape.Bounds.Width / 2);
            if (!checkBoundarys(min, max))
            {
                movementDelegate.Invoke(new EnvironmentResult(new List<ISpatialEntity>(), EnvironmentResultCode.ERROR_OUT_OF_BOUNDS), new DummySpatialEntity());
                return;
            }
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            Vector3 freepos = GetNextFreeCell(min, max);
            if (freepos == Vector3.Null)
            {
                var rnd = _random;
                min += shapeTmp;
                max -= shapeTmp;
                freepos = new Vector3((double)rnd.Next((int)min.X, (int)max.X), (double)rnd.Next((int)min.Y, (int)max.Y), 0);
            }
            var objId = getNextObjId();
            var objData = objId | FLAG_NEW;

            lock (_sync)
            {
                m_AddDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
                _objIdList.Add(objId);
            }
            entity.Shape = new Circle(freepos.X, freepos.Y, entity.Shape.Bounds.Width / 2);
            while (!_spatialObjIdMap.TryAdd(entity.AgentGuid, objId)) ;

            while (!_objIdSpatialMap.TryAdd(objId, entity)) ;



            clCircleShapeObject act = ConvertShapeToClShape(entity.Shape);
            _objIdClShapeMap.TryAdd(objId, act);
            _EnvFreshAddedObjs.TryAdd(objId, act);
            /*
                        shapeList.Add(act);
                        
                        */
            // Added Element -> Now wait until the CollisionDetection did its work at the LayerTick
            Interlocked.Decrement(ref m_ActiveOperationCount);

            // envActEnvObjs.Add(objId, act);


            // We need the list with free positions first
        }



        public void ExploreAll(ExploreDelegate exploreDelegate)
        {
            lock (_sync)
            {
                m_ExploreAllDelegates.Add(exploreDelegate);
            }
        }

        public void Remove(Guid agentId, Action<ISpatialEntity> removeDelegate)
        {
            lock (_sync)
            {
                m_ActRemoves.Add(new Tuple<Guid, Action<ISpatialEntity>>(agentId, removeDelegate));
            }

        }

        public void Resize(Guid agentId, IShape shape, MovementDelegate movementDelegate)
        {
            var entity = _objIdSpatialMap[_spatialObjIdMap[agentId]];
            // TODO: Check if shape > cellsize
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            var objId = _spatialObjIdMap[entity.AgentGuid];
            var newEnt = entity;
            newEnt.Shape = shape;
            //            uint tmpId;
            //            while (!_spatialObjIdMap.TryRemove(entity.AgentGuid, out tmpId));
            //            while (!_spatialObjIdMap.TryAdd(newEnt.AgentGuid, (uint)objId));
            _objIdSpatialMap[(uint)objId].Shape = shape;

            lock (_sync)
            {
                _shapeList.Remove(_objIdClShapeMap[(uint)objId]);
                m_MoveDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
            }
            clCircleShapeObject act = ConvertShapeToClShape(entity.Shape);
            lock (_sync)
            {
                _shapeList.Add(act);
            }
            _objIdClShapeMap[(uint)objId] = act;
            Interlocked.Decrement(ref m_ActiveOperationCount);

        }

        public void Move(Guid agentId, Vector3 movementVector, Direction rotation, MovementDelegate movementDelegate, int maxResults = 100)
        {
            var entity = _objIdSpatialMap[_spatialObjIdMap[agentId]];
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            var objId = _spatialObjIdMap[agentId];
            lock (_sync)
            {
                m_MoveDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
            }



            var newEnt = entity;
            var newShape = newEnt.Shape.Transform(movementVector, rotation);
            newEnt.Shape = newShape;
            uint tmpId;

            _objIdSpatialMap[(uint)objId].Shape = newShape;

            //shapeList.Remove(objIdClShapeMap[objId]);

            clCircleShapeObject tmpCircleShape;
            _EnvActEnvObjs.TryRemove(objId, out tmpCircleShape);
            // TODO Testen ob es auch mit Konstruktoren funktioniert
            clCircleShapeObject act = ConvertShapeToClShape(entity.Shape);


            _EnvActEnvObjs[objId] = act;
            //shapeList.Add(act);

            _objIdClShapeMap[(uint)objId] = act;

            Interlocked.Decrement(ref m_ActiveOperationCount);
        }

        public void Explore(IShape shape, ExploreDelegate exploreDelegate, Type agentType = null, Enum collisionType = null, int maxResults = 100)
        {

            IEnumerable<ISpatialEntity> res;
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            uint objId = (uint)Interlocked.Increment(ref exploreIdGenBase);

            lock (_sync)
            {
                _objIdList.Add(objId);
                m_ExploreDelegates.Add(new Tuple<uint, ExploreDelegate>(objId, exploreDelegate));
            }


            clCircleShapeObject act = ConvertShapeToClShape(shape);

            _objIdClShapeMap[objId] = act;
            _EnvExploreObjs.TryAdd(objId, act);



            Interlocked.Decrement(ref m_ActiveOperationCount);


        }

        #region CollisionHelper

        private void CreateBuffers()
        {


            // Create Buffers
            // Actually we need to create the CELLID and CELLOBJ buffers after the reorder kernel


            _clShapeIdArray = new List<clShapeIdTupel>();
            for (int i = 0; i < _clShapeArray.Count; i++)
            {
                var tmp = new clShapeIdTupel();
                tmp.objId = _clObjArray[i];
                tmp.objShape = _clShapeArray[i];
                _clShapeIdArray.Add(tmp);
            }
            // All shapes and objIds as input
            cl_shapeIdInputMem = new ComputeBuffer<clShapeIdTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, _clShapeIdArray.ToArray());
            // Resultbuffer for the ReorderElements Kernel, since we don't know the distribution we need to allocate enough space for every element at every lvl
            cl_lvlShapeIdMem = new ComputeBuffer<clShapeIdTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite , _constants.numTotalElements * _constants.numLvls);


            //            // All shapes and objIds as input
            //            cl_shapeIdInputMem = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_constants.numTotalElements * Marshal.SizeOf(typeof(clShapeIdTupel))),_clShapeIdArray.ToArray(),
            //                   out error);
            //            CheckErr(error, "Createbuffer");
            //
            //            // Resultbuffer for the ReorderElements Kernel, since we don't know the distribution we need to allocate enough space for every element at every lvl
            //            cl_lvlShapeIdMem = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_constants.numTotalElements * _constants.numLvls * Marshal.SizeOf(typeof(clShapeIdTupel))),
            //                   out error);
            //            CheckErr(error, "Createbuffer");


            // lvlElements contains the number of native elements for each lvl 
            cl_lvlElements = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _lvlElements);
            //            cl_lvlElements = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_constants.numLvls * Marshal.SizeOf(typeof(uint))),
            //                 out error);
            //            CheckErr(error, "Createbuffer");

            //Contains the total amout of element at this lvl, these numbers will be higher than lvlElements because the elements of the lower lvls will be importe into the upper lvls
            cl_lvlTotalElements = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _constants.numLvls);

//            cl_lvlTotalElements = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_constants.numLvls * Marshal.SizeOf(typeof(uint))),
//                 out error);
//            CheckErr(error, "Createbuffer");

            // TODO; we first need to know the amount of lvls....


            /*
                        cl_lvlCellData = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_clObjArray.Count * CELL_DATA_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                            out error);
                        CheckErr(error, "Createbuffer");

                        cl_lvlCellIds = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_clObjArray.Count * CELL_ID_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                            out error);
                        CheckErr(error, "Createbuffer");
            */

//
//            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_shapeIdInputMem, Bool.True, IntPtr.Zero, (IntPtr)(_clShapeIdArray.Count * Marshal.SizeOf(typeof(clShapeIdTupel))),
//                _clShapeIdArray.ToArray(), 0, null,
//                out eve);
//            CheckErr(error, "EnqBuffer");
//
//
//            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_lvlElements, Bool.True, IntPtr.Zero, (IntPtr) (_constants.numLvls * 4),
//                _lvlElements, 0, null,
//                out eve);
//            CheckErr(error, "EnqBuffer");
//
//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");
        }

        private void ClReorderElements() {


            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            ckReorderElements.SetMemoryArgument(0, cl_shapeIdInputMem);
            ckReorderElements.SetMemoryArgument(1, cl_lvlShapeIdMem);
            ckReorderElements.SetMemoryArgument(2, cl_lvlElements);
            ckReorderElements.SetValueArgument(3, _constants);
            cqCommandQueue.Execute(ckReorderElements, null, globalWorkSize, localWorkSize, eventList);
            cqCommandQueue.Finish();


//            ErrorCode error;
//            Event eve;
//            // Enqueue create list kernel
//            IntPtr agentPtrSize = (IntPtr) 0;
//            var ptrSize = (IntPtr) Marshal.SizeOf(typeof(Mem));
//
//
////            uint globalWorkSize = _constants.numThreadsPerBlock*(uint) _constants.numBlocks;
////            uint localWorkSize = _constants.numThreadsPerBlock;
//
//            IntPtr[] workGroupSizePtr = new IntPtr[] {(IntPtr) globalWorkSize};
//            IntPtr[] localWorkGroupSizePtr = new IntPtr[] {(IntPtr) localWorkSize};
//            Event clevent;
//            error = Cl.SetKernelArg(ckReorderElements, 0, ptrSize, cl_shapeIdInputMem);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckReorderElements, 1, ptrSize, cl_lvlShapeIdMem);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckReorderElements, 2, ptrSize, cl_lvlElements);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckReorderElements, 3, (IntPtr) (Marshal.SizeOf(typeof(cl2DCollisionConstants))), _constants);
//            CheckErr(error, "Cl.SetKernelArg");
//
//            error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckReorderElements, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
//            CheckErr(error, "Cl.EnqueueNDRangeKernel REORDER_ELEMENTS");
//
//
//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish REORDER_ELEMENTS");
//
//            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlElements, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numLvls * 4),
//                _lvlElements, 0, null, out eve);    
//            CheckErr(error, "Cl.EnqueueReadBuffer");


            if (DEBUG) {
                cqCommandQueue.ReadFromBuffer(cl_lvlShapeIdMem,ref _readLvlShapeElements, false, eventList);
//                error = Cl.EnqueueReadBuffer
//                    (cqCommandQueue, cl_lvlShapeIdMem, Bool.True, IntPtr.Zero, (IntPtr) ((IntPtr)(_constants.numTotalElements * _constants.numLvls * Marshal.SizeOf(typeof(clShapeIdTupel)))),
//                        _readLvlShapeElements, 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");

                var tmp = new clShapeIdTupel[_constants.numTotalElements];
                cqCommandQueue.ReadFromBuffer(cl_shapeIdInputMem, ref tmp, false, eventList);

//                error = Cl.EnqueueReadBuffer
//                    (cqCommandQueue, cl_shapeIdInputMem, Bool.True, IntPtr.Zero, (IntPtr)((IntPtr)(_constants.numTotalElements  * Marshal.SizeOf(typeof(clShapeIdTupel)))),
//                        tmp, 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");
                cqCommandQueue.Finish();


                DebugHelper.PrintElementBuffer( _lvlElements, 5, "Level elements after reorder");
                DebugHelper.PrintLevelShapeElements(_readLvlShapeElements, _lvlElements,_constants,  "Level shape reorder result");

//                DebugHelper.PrintCellIdBufferExtended
//                    (_debugReadInput, _readCellIds, _readCellData, _cell3DPosList, "CreateCellId Array", (int) _constants.numTotalElements);

            }
        }

        private void ClCreateCellId()
        {


            ComputeEventList eventList = new ComputeEventList();

            // First we need to create the cellIdBuffers based on the calculated size in reorder elements
            // TODO: Find criteria to reduce the amout of layers based on the memory size and the amount of elements per layer


            // First check the amount of occupied lvls 


            _lvlTotalElements = new uint[_constants.numLvls];
            _lvlTotalElements[_constants.numLvls - 1] = _lvlElements[_constants.numLvls - 1];
            for (int i = (int)_constants.numLvls -2; i >= 0; i--) {
                _lvlTotalElements[i] = _lvlTotalElements[i+1] + _lvlElements[i];
            }

            // First check the amount of occupied lvls 
            uint lvlCnt = 0;
            for (var i = 0; i < _constants.numLvls; i++)
            {
                if (_lvlTotalElements[i] > 0)
                {
                    lvlCnt++;
                }
            }
            _constants.numLvls = lvlCnt;


            _constants.cellArraySize = 0;
            for (int i = 0; i < _constants.numLvls; i++) {
                _constants.cellArraySize += _lvlTotalElements[i];
            }
    
        // Init cl_Memory elements since we know the amount of lvls now
        cl_sharedIdxMem = new ComputeBuffer<int>[_constants.numLvls];
            cl_lvlCellData = new ComputeBuffer<ulong>[_constants.numLvls];
            cl_lvlCellIds = new ComputeBuffer<uint>[_constants.numLvls];
            cl_narrowCheckSublists = new ComputeBuffer<clCollisionSublist>[_constants.numLvls];
            cl_lvlCollisionCheckTuples= new ComputeBuffer<CollisionTupel>[_constants.numLvls];
            cl_lvlCollisonTuples= new ComputeBuffer<CollisionTupel>[_constants.numLvls];
            cl_lvlCollisonShapes = new ComputeBuffer<clCircleShapeTupel>[_constants.numLvls];
            cl_lvlExporeTuples = new ComputeBuffer<CollisionTupel>[_constants.numLvls];
            cl_lvlExploreElementIdx = new ComputeBuffer<int>[_constants.numLvls];
            cl_DebugLvl2DPosList = new ComputeBuffer<CollisionCell2D>[_constants.numLvls];
            // Same for the buffer in which the gpu buffers are read back
            _lvlExploreIdx = new int[_constants.numLvls][];
            _sharedIdx = new int[_constants.numLvls][];
            for (int i = 0; i < _constants.numLvls; i++) {
                _lvlExploreIdx[i] = new int[1];
                _sharedIdx[i] = new int[1];
            }
            _readNarrowCheckList = new clCollisionSublist[_constants.numLvls][];
            _readCellIds = new uint[_constants.numLvls][];
            _readCellData = new ulong[_constants.numLvls][];
            _readLvlCell2DPosList = new CollisionCell2D[_constants.numLvls][];
            _readLvlCollisionList = new CollisionTupel[_constants.numLvls][];
            _readLvlCheckTupelList = new CollisionTupel[_constants.numLvls][];
            _collisionShapeTupels = new clCircleShapeTupel[_constants.numLvls][];
            _readLvlExploreList = new CollisionTupel[_constants.numLvls][];
            for (int i = 0; i < _constants.numLvls; i++)
            {
                _lvlExploreIdx[i] = new int[1];
                _sharedIdx[i] = new int[1];
                _readCellIds[i] = new uint[_lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT];
                _readCellData[i] = new ulong[_lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT];
                _readLvlCell2DPosList[i] = new CollisionCell2D[_lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT];

            }

            // Create the buffers for each level
            for (int i = 0; i < _constants.numLvls; i++) {
                cl_sharedIdxMem[i] = new ComputeBuffer<int>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _sharedIdx[i]);

//                error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_sharedIdxMem[i], Bool.True, IntPtr.Zero, (IntPtr)(4),
//                    0, 0, null,
//                    out eve);
//                CheckErr(error, "EnqBuffer");
//                // Create the cellbuffers, the size is now know after the reorder happened
                cl_lvlCellData[i] = new ComputeBuffer<ulong>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _lvlTotalElements[i]  * GRID_FIELDS_PER_ELEMENT);
                //Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_lvlTotalElements[i] * CELL_DATA_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                //                    out error);
                //                CheckErr(error, "Createbuffer");

                cl_lvlCellIds[i] = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT);

//                cl_lvlCellIds[i] = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_lvlTotalElements[i] * CELL_ID_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
//                    out error);
//                CheckErr(error, "Createbuffer");

                // When debugmode is enabled allocate memory for the debug cells, elsewise just create a dummy buffer to prevent the kernel call from crashing
                if (DEBUG)
                {
                    cl_DebugLvl2DPosList[i] = new ComputeBuffer<CollisionCell2D>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _constants.cellArraySize * GRID_FIELDS_PER_ELEMENT);

//                    cl_DebugLvl2DPosList[i] = Cl.CreateBuffer
//                        (cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_constants.cellArraySize * GRID_FIELDS_PER_ELEMENT * Marshal.SizeOf(typeof(CollisionCell2D))),
//                            out error);
                }
                else
                {
                    cl_DebugLvl2DPosList[i] = new ComputeBuffer<CollisionCell2D>(cxGPUContext, ComputeMemoryFlags.ReadWrite, 1);
//
//                    cl_DebugLvl2DPosList[i] = Cl.CreateBuffer
//                        (cxGPUContext, MemFlags.ReadWrite, (IntPtr)(4),
//                            out error);

                }

            }
            cqCommandQueue.ReadFromBuffer(cl_lvlTotalElements,ref _lvlTotalElements,true, eventList);


            //            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_lvlTotalElements, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numLvls * 4),
            //                _lvlTotalElements, 0, null,
            //                         out eve);
            //            CheckErr(error, "EnqBuffer");


            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };

       

            // Enqueue create list kernel
//            IntPtr agentPtrSize = (IntPtr)0;
//            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));


//            uint globalWorkSize = _constants.numThreadsPerBlock * (uint)_constants.numBlocks;
//            uint localWorkSize = _constants.numThreadsPerBlock;
//
//            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
//            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
            for (int i = 0; i < _constants.numLvls; i++) {
                // Calculate offsets and startidx 
                long numKernelElements = 0;
                for (int j = i; j < _constants.numLvls; j++) {
                    numKernelElements += _lvlElements[i];
                }


                ckCreateCellIdArray.SetMemoryArgument(0, cl_lvlShapeIdMem);
                ckCreateCellIdArray.SetMemoryArgument(1, cl_lvlElements);
                ckCreateCellIdArray.SetMemoryArgument(2, cl_lvlCellIds[i]);
                ckCreateCellIdArray.SetMemoryArgument(3, cl_lvlCellData[i]);
                ckCreateCellIdArray.SetValueArgument(4, _constants);
                ckCreateCellIdArray.SetValueArgument(5, i);
                ckCreateCellIdArray.SetValueArgument(6, _lvlTotalElements[i]);
                ckCreateCellIdArray.SetMemoryArgument(7, cl_DebugLvl2DPosList[i]);
                cqCommandQueue.Execute(ckCreateCellIdArray, null, globalWorkSize, localWorkSize, eventList);

//                error = Cl.SetKernelArg(ckCreateCellIdArray, 0, ptrSize, cl_lvlShapeIdMem);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 1, ptrSize, cl_lvlElements);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 2, ptrSize, cl_lvlCellIds[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 3, ptrSize, cl_lvlCellData[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 4, (IntPtr)(Marshal.SizeOf(typeof(cl2DCollisionConstants))), _constants);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 5, (IntPtr)4, i);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 6, (IntPtr)4, _lvlTotalElements[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 7, ptrSize, cl_DebugLvl2DPosList[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//
//                error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCreateCellIdArray, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
//                CheckErr(error, "Cl.EnqueueNDRangeKernel");

            }

            cqCommandQueue.Finish();

//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");


            /*   error = Cl.EnqueueReadBuffer(cqCommandQueue, freeCellMem, Bool.True, IntPtr.Zero, (IntPtr)(2),
                   freeCellList, 0, null, out eve);
               CheckErr(error, "Cl.EnqueueReadBuffer");*/


            if (DEBUG)
            {

                /*error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_shapeIdInputMem, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * CELL_DATA_ELEMENT_SIZE),
                    _debugReadInput, 0, null, out eve);
                CheckErr(error, "Cl.EnqueueReadBuffer");
*/
                for (int i = 0; i < _constants.numLvls; i++) {
                    cqCommandQueue.ReadFromBuffer(cl_lvlCellIds[i], ref _readCellIds[i],false, eventList);
//                    error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlCellIds[i], Bool.True, IntPtr.Zero, (IntPtr)(_lvlTotalElements[i] * CELL_ID_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
//                    _readCellIds[i], 0, null, out eve);
//                    CheckErr(error, "Cl.EnqueueReadBuffer");

                    cqCommandQueue.ReadFromBuffer(cl_lvlCellData[i], ref _readCellData[i], false, eventList);

                    //                    error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlCellData[i], Bool.True, IntPtr.Zero, (IntPtr)(_lvlTotalElements[i] * CELL_DATA_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                    //                    _readCellData[i], 0, null, out eve);
                    //                    CheckErr(error, "Cl.EnqueueReadBuffer");

                    cqCommandQueue.ReadFromBuffer(cl_DebugLvl2DPosList[i], ref _readLvlCell2DPosList[i], false, eventList);

//                    error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_DebugLvl2DPosList[i], Bool.True, IntPtr.Zero, (IntPtr)(_lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT * Marshal.SizeOf(typeof(CollisionCell2D))),
//                    _readLvlCell2DPosList[i], 0, null, out eve);
//                    CheckErr(error, "Cl.EnqueueReadBuffer");

                    for (int j = 0; j < _lvlTotalElements[i]; j++)
                    {
                        if (!_cellMap.ContainsKey(_readCellIds[i][j]))
                            _cellMap.TryAdd(_readCellIds[i][j], _readLvlCell2DPosList[i][j]);
                    }
                }


               
              

                DebugHelper.PrintCellIdBufferExtended( _readCellIds, _readCellData, _readLvlCell2DPosList, "CreateCellId Array", _constants, _lvlTotalElements);

                //DebugHelper.PrintCellIdBuffer(debugReadInput,readCellIdList,readCellData,"CreateCellId Array",constants.numTotalElements);

                //Debug.WriteLine();
            };
            cl_shapeIdInputMem.Dispose();
//            error = Cl.ReleaseMemObject(cl_shapeIdInputMem);
//            CheckErr(error, "Cl.ReleaseMemObj");

            for (int i = 0; i < _constants.numLvls; i++) {
                cl_DebugLvl2DPosList[i].Dispose();
//                error = Cl.ReleaseMemObject(cl_DebugLvl2DPosList[i]);
//                CheckErr(error, "Cl.ReleaseMemObj");
            }
            //Cl.ReleaseMemObject(objIdMem);
        }

        private void ClCreateNarrowCheckSublists()
        {
            for (int i = 0; i < _constants.numLvls; i++)
            {
                // TODO: Find a proper way to choose a size....
                cl_narrowCheckSublists[i] = new ComputeBuffer<clCollisionSublist>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _lvlTotalElements[i]/2 );
                //                    Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_lvlTotalElements[i]  * Marshal.SizeOf(typeof(clCollisionSublist)) / 2) , 
                //                     out error);
                //                CheckErr(error, "Createbuffer");
            }
            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };
            ComputeEventList eventList = new ComputeEventList();




            //            int globalWorkSize = (int)(_constants.numThreadsPerBlock * _constants.numBlocks);
            //            int localWorkSize = (int)_constants.numThreadsPerBlock;
            //
            //            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
            //            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
            //            Event clevent;
            for (int i = 0; i < _constants.numLvls; i++) {
                // Create Collision List

                ckCreateNarrowCheckList.SetMemoryArgument(0, cl_lvlCellIds[i]);
                ckCreateNarrowCheckList.SetMemoryArgument(1, cl_lvlCellData[i]);
                ckCreateNarrowCheckList.SetMemoryArgument(2, cl_narrowCheckSublists[i]);
                ckCreateNarrowCheckList.SetMemoryArgument(3, cl_sharedIdxMem[i]);
                ckCreateNarrowCheckList.SetValueArgument(4, _constants);
                ckCreateNarrowCheckList.SetValueArgument(5, _lvlTotalElements[i]);
                cqCommandQueue.Execute(ckCreateNarrowCheckList, null, globalWorkSize, localWorkSize, eventList);

//
//                error = Cl.SetKernelArg(ckCreateNarrowCheckList, 0, ptrSize, cl_lvlCellIds[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateNarrowCheckList, 1, ptrSize, cl_lvlCellData[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateNarrowCheckList, 2, ptrSize, cl_narrowCheckSublists[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateNarrowCheckList, 3, ptrSize, cl_sharedIdxMem[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateNarrowCheckList, 4, (IntPtr)(Marshal.SizeOf(typeof(cl2DCollisionConstants))), _constants);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateNarrowCheckList, 5, (IntPtr)4, _lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT);
//                CheckErr(error, "Cl.SetKernelArg");
//
//                error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCreateNarrowCheckList, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
//                CheckErr(error, "Cl.EnqueueNDRangeKernel");

            }
            cqCommandQueue.Finish();

//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");

            for (int i = 0; i < _constants.numLvls; i++) {
                cqCommandQueue.ReadFromBuffer(cl_sharedIdxMem[i], ref _sharedIdx[i], false, eventList);
//                error = Cl.EnqueueReadBuffer
//                    (cqCommandQueue, cl_sharedIdxMem[i], Bool.True, IntPtr.Zero, (IntPtr) (4),
//                        _sharedIdx[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");

            }

            if (DEBUG)
            {

                for (int i = 0; i < _constants.numLvls; i++) {

                    _readNarrowCheckList[i] = new clCollisionSublist[_sharedIdx[i][0]];
                    cqCommandQueue.ReadFromBuffer(cl_narrowCheckSublists[i], ref _readNarrowCheckList[i], false, eventList);

//                    error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_narrowCheckSublists[i], Bool.True, IntPtr.Zero, (IntPtr)(_sharedIdx[i][0] * GRID_FIELDS_PER_ELEMENT),
//                        _readNarrowCheckList[i], 0, null, out eve);
//                    CheckErr(error, "Cl.EnqueueReadBuffer");
                }
          
                cqCommandQueue.Finish();


                /*                for (int i = 0; i < readCellIdList.Count(); i++)
                                {
                                    if (cellMap.ContainsKey(readCellIdList[i]))
                                    {
                                        cellPosList[i] = cellMap[readCellIdList[i]];
                                    }
                                    else
                                    {
                                        cellPosList[i] = new CollisionCell();
                                    }
                                }*/

                DebugHelper.PrintCollisionList(_readNarrowCheckList, _readCellIds, _readCellData, _readLvlCell2DPosList, "CollisionList", _sharedIdx, _constants.numLvls);
            }
        }

        private void ClCreateCollsionTuples()
        {

            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };
            ComputeEventList eventList = new ComputeEventList();
            //            ErrorCode error;
            //            Event eve;
            //            IntPtr agentPtrSize = (IntPtr)0;
            //            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));

//            uint globalWorkSize = _constants.numThreadsPerBlock * (uint)_constants.numBlocks;
//            uint localWorkSize = _constants.numThreadsPerBlock;
//
//            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
//            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
            for (int i = 0; i < _constants.numLvls; i++) {
                cl_lvlCollisionCheckTuples[i] = new ComputeBuffer<CollisionTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT);

//                cl_lvlCollisionCheckTuples[i] = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT * Marshal.SizeOf(typeof(CollisionTupel))),
//                    out error);
//                CheckErr(error, "Createbuffer");
                
                cqCommandQueue.WriteToBuffer(new int[]{0}, cl_sharedIdxMem[i] ,true,eventList);
//                error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_sharedIdxMem[i], Bool.True, IntPtr.Zero, (IntPtr)(4),
//                     0, 0, null, out eve);


                if (DEBUG) Debug.WriteLine(" Num Elements {0} total {1}", _sharedIdx[0], _constants.numTotalElements);
                //                CheckErr(error, "EnqBuffer")
                //                    ;
                ckCreateCollsionTuples.SetMemoryArgument(0, cl_lvlCellData[i]);
                ckCreateCollsionTuples.SetMemoryArgument(1, cl_narrowCheckSublists[i]);
                ckCreateCollsionTuples.SetMemoryArgument(2, cl_lvlCollisionCheckTuples[i]);
                ckCreateCollsionTuples.SetMemoryArgument(3, cl_sharedIdxMem[i]);
                ckCreateCollsionTuples.SetValueArgument(4, _constants);
                ckCreateCollsionTuples.SetValueArgument(5, _sharedIdx[i][0]);
                cqCommandQueue.Execute(ckCreateCollsionTuples, null, globalWorkSize, localWorkSize, eventList);


//                error = Cl.SetKernelArg(ckCreateCollsionTuples, 0, ptrSize, cl_lvlCellData[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCollsionTuples, 1, ptrSize, cl_narrowCheckSublists[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCollsionTuples, 2, ptrSize, cl_lvlCollisionCheckTuples[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCollsionTuples, 3, ptrSize, cl_sharedIdxMem[i]);
//
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCollsionTuples, 4, (IntPtr)(Marshal.SizeOf(typeof(cl2DCollisionConstants))), _constants);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCreateCollsionTuples, 5, (IntPtr)4, _sharedIdx[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//
//                error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCreateCollsionTuples, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
//                CheckErr(error, "Cl.EnqueueNDRangeKernel");

            }

            cqCommandQueue.Finish();

//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");
            for (int i = 0; i < _constants.numLvls; i++) {
                cqCommandQueue.ReadFromBuffer(cl_sharedIdxMem[i], ref _sharedIdx[i],true,eventList);
//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_sharedIdxMem[i], Bool.True, IntPtr.Zero, (IntPtr)(4),
//                    _sharedIdx[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");

                _readLvlCheckTupelList[i] = new CollisionTupel[_sharedIdx[i][0]];
                cqCommandQueue.ReadFromBuffer(cl_lvlCollisionCheckTuples[i], ref _readLvlCheckTupelList[i], true, eventList);

//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlCollisionCheckTuples[i], Bool.True, IntPtr.Zero, (IntPtr)(_sharedIdx[i][0] * Marshal.SizeOf(typeof(CollisionTupel))),
//                _readLvlCheckTupelList[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");


                cl_narrowCheckSublists[i].Dispose();
//                error = Cl.ReleaseMemObject(cl_narrowCheckSublists[i]);
//                CheckErr(error, "Cl.ReleaseMem");



            }
            if (DEBUG)
            {
                //                    for (int j = 0; j < _readCellIds.Count(); j++)
                //                    {
                //                        if (_cellMap.ContainsKey(_readLvlCheckTupelList[i][j]))
                //                        {
                //                            _readLvlCell2DPosList[i] = _cellMap[_readLvlCheckTupelList[i][j]];
                //                        }
                //                        else
                //                        {
                //                            _readLvlCell2DPosList[i] = new CollisionCell2D();
                //                        }
                //}
                DebugHelper.PrintCollisionTuples(_readLvlCheckTupelList, _readCellIds, _readCellData, _readLvlCell2DPosList, "Collision tuples to check", _sharedIdx, _constants.numLvls);

            }


        }

        private void ClCheckCollsions()
        {

            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };
            ComputeEventList eventList = new ComputeEventList();
            //            ErrorCode error;
            //            Event eve;
            //            IntPtr agentPtrSize = (IntPtr)0;
            //            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));

//            uint globalWorkSize = _constants.numThreadsPerBlock * (uint)_constants.numBlocks;
//            uint localWorkSize = _constants.numThreadsPerBlock;

//            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
//            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
//            Event clevent;
            for (int i = 0; i < _constants.numLvls; i++) {
                if (_sharedIdx[i][0] <= 0)
                    continue;
                cl_lvlCollisonTuples[i] = new ComputeBuffer<CollisionTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _sharedIdx[i][0]);
                cl_lvlExporeTuples[i] = new ComputeBuffer<CollisionTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _sharedIdx[i][0]);
                cl_lvlCollisonShapes[i] = new ComputeBuffer<clCircleShapeTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _collisionShapeTupels[i]);
                cl_lvlExploreElementIdx[i] = new ComputeBuffer<int>(cxGPUContext, ComputeMemoryFlags.ReadWrite, 1);

//                cl_lvlCollisonTuples[i] = Cl.CreateBuffer
//                    (cxGPUContext, MemFlags.ReadWrite, (IntPtr) (_sharedIdx[i][0]*Marshal.SizeOf(typeof(CollisionTupel))),
//                        out error);
//                CheckErr(error, "Createbuffer");
//
//                cl_lvlExporeTuples[i] = Cl.CreateBuffer
//                   (cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_sharedIdx[i][0] * Marshal.SizeOf(typeof(CollisionTupel))),
//                       out error);
//                CheckErr(error, "Createbuffer");
//
//                cl_lvlCollisonShapes[i] = Cl.CreateBuffer
//                    (cxGPUContext, MemFlags.ReadWrite, (IntPtr) (_sharedIdx[i][0]*Marshal.SizeOf(typeof(clCircleShapeTupel))),
//                        out error);
//                CheckErr(error, "Createbuffer");
//
//                cl_lvlExploreElementIdx[i] = Cl.CreateBuffer
//                    (cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_sharedIdx[i][0] * Marshal.SizeOf(typeof(clCircleShapeTupel))),
//                        out error);
//                CheckErr(error, "Createbuffer");

                _collisionShapeTupels[i] = new clCircleShapeTupel[_sharedIdx[i][0]];
                _readLvlExploreList[i] = new CollisionTupel[_sharedIdx[i][0]];
                // Merge all tupels to one list -> 

                for (int j = 0; j < _sharedIdx[i][0]; j++) {
                    _collisionShapeTupels[i][j] = new clCircleShapeTupel();
                    // _readLvlCheckTupelList contains the lvl specific idx of the elements
                    uint keytmp = (uint) (_readCellData[i][_readLvlCheckTupelList[i][j].obj1] & 0xFFFFFFFF);
                    if (!_objIdClShapeMap.ContainsKey(keytmp))
                       if(DEBUG) Debug.WriteLine("Wrong Tempkey : " + keytmp);
                     _collisionShapeTupels[i][j].obj1 = _objIdClShapeMap[keytmp];

                    keytmp = (uint) (_readCellData[i][_readLvlCheckTupelList[i][j].obj2] & 0xFFFFFFFF);

                    _collisionShapeTupels[i][j].obj2 = _objIdClShapeMap[keytmp];


//                    // Set idx to 0
//                    error = Cl.EnqueueWriteBuffer
//                    (cqCommandQueue, cl_lvlExploreElementIdx[i], Bool.True, IntPtr.Zero, (IntPtr)(4),
//                        0, 0, null, out eve);
//                    CheckErr(error, "EnqBuffer");
//
//                    error = Cl.EnqueueWriteBuffer
//                    (cqCommandQueue, cl_sharedIdxMem[i], Bool.True, IntPtr.Zero, (IntPtr)(4),
//                        0, 0, null, out eve);
//                    CheckErr(error, "EnqBuffer");
//                    // CheckCollsions
//
//                    error = Cl.EnqueueWriteBuffer
//                        (cqCommandQueue, cl_lvlCollisonShapes[i], Bool.False, IntPtr.Zero, (IntPtr)(_sharedIdx[i][0] * Marshal.SizeOf(typeof(clCircleShapeTupel))),
//                            _collisionShapeTupels[i], 0, null, out eve);
//
//                    CheckErr(error, "EnqBuffer");
                }
                // Set idx to 0
                cqCommandQueue.WriteToBuffer(new int[] { 0 }, cl_lvlExploreElementIdx[i], true, eventList);
                cqCommandQueue.WriteToBuffer(new int[] { 0 }, cl_sharedIdxMem[i], true, eventList);

                ckCheckCollsions.SetMemoryArgument(0, cl_lvlCellData[i]);
                ckCheckCollsions.SetMemoryArgument(1, cl_lvlCollisionCheckTuples[i]);
                ckCheckCollsions.SetMemoryArgument(2, cl_lvlCollisonShapes[i]);
                ckCheckCollsions.SetMemoryArgument(3, cl_lvlCollisonTuples[i]);
                ckCheckCollsions.SetMemoryArgument(4, cl_lvlExporeTuples[i]);
                ckCheckCollsions.SetMemoryArgument(5, cl_sharedIdxMem[i]);
                ckCheckCollsions.SetMemoryArgument(6, cl_lvlExploreElementIdx[i]);
                ckCheckCollsions.SetValueArgument(7, _constants);
                ckCheckCollsions.SetValueArgument(8, _sharedIdx[i][0]);
                cqCommandQueue.Execute(ckCheckCollsions, null, globalWorkSize, localWorkSize, eventList);

//
//                error = Cl.SetKernelArg(ckCheckCollsions, 0, ptrSize, cl_lvlCellData[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 1, ptrSize, cl_lvlCollisionCheckTuples[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 2, ptrSize, cl_lvlCollisonShapes[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 3, ptrSize, cl_lvlCollisonTuples[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 4, ptrSize, cl_lvlExporeTuples[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 5, ptrSize, cl_sharedIdxMem[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 6, ptrSize, cl_lvlExploreElementIdx[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 7, (IntPtr)(Marshal.SizeOf(typeof(cl2DCollisionConstants))), _constants);
//                CheckErr(error, "Cl.SetKernelArg");
//                error = Cl.SetKernelArg(ckCheckCollsions, 8, (IntPtr)4, _sharedIdx[i]);
//                CheckErr(error, "Cl.SetKernelArg");
//
//                error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCheckCollsions, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
//                CheckErr(error, "Cl.EnqueueNDRangeKernel");
            }

            cqCommandQueue.Finish();
//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");

            for (int i = 0; i < _constants.numLvls; i++) {
                if (_sharedIdx[i][0] <= 0)
                    continue;
                cqCommandQueue.ReadFromBuffer(cl_sharedIdxMem[i], ref _sharedIdx[i], true, eventList );
                cqCommandQueue.ReadFromBuffer(cl_lvlExploreElementIdx[i], ref _lvlExploreIdx[i], true, eventList);
//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_sharedIdxMem[i], Bool.False, IntPtr.Zero, (IntPtr)(4),
//                     _sharedIdx[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");
//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlExploreElementIdx[i], Bool.False, IntPtr.Zero, (IntPtr)(4),
//                    _lvlExploreIdx[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");



                _readLvlCollisionList[i] = new CollisionTupel[_sharedIdx[i][0]];
                _readLvlExploreList[i] = new CollisionTupel[_lvlExploreIdx[i][0]];

                cqCommandQueue.ReadFromBuffer(cl_lvlCollisonTuples[i], ref _readLvlCollisionList[i], false, eventList);
                cqCommandQueue.ReadFromBuffer(cl_lvlExporeTuples[i], ref _readLvlExploreList[i], false, eventList);

//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlCollisonTuples[i], Bool.False, IntPtr.Zero, (IntPtr)(_sharedIdx[i][0] * Marshal.SizeOf(typeof(CollisionTupel))),
//                    _readLvlCollisionList[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");
//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlExporeTuples[i], Bool.False, IntPtr.Zero, (IntPtr)(_lvlExploreIdx[i][0] * Marshal.SizeOf(typeof(CollisionTupel))),
//                    _readLvlExploreList[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");
            }
            cqCommandQueue.Finish();
            

        }
        #endregion

        public void Commit()
        {
            var before = DateTime.Now;
            // Block all incoming requests until the calculation 
            gpuActive = true;
            // Wait till the processing add and move operations finished
            while (m_ActiveOperationCount > 0)
            {
                Thread.Sleep(5);
            }

            // Total number of elements to calculate
            _constants.numTotalElements = (uint)(_EnvActEnvObjs.Count + _EnvExploreObjs.Count + _EnvFreshAddedObjs.Count);


            if (_constants.numTotalElements == 0)
                return;

 
//            _cell2DPosList = new CollisionCell2D[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
//            _tupelList = new CollisionTupel[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
//            _readCellData = new ulong[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
//            _readCellIds = new uint[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];

            _clShapeArray = _EnvActEnvObjs.Values.ToList();
            _clShapeArray.AddRange(_EnvExploreObjs.Values.ToList());
            _clShapeArray.AddRange(_EnvFreshAddedObjs.Values.ToList());

            //shapes.
            _clObjArray = _EnvActEnvObjs.Keys.ToList().Select(i => (ulong)i + FLAG_MOVE).ToList();
            _clObjArray.AddRange(_EnvExploreObjs.Keys.ToList().Select(i => (ulong)i  + FLAG_EXLORE));
            _clObjArray.AddRange(_EnvFreshAddedObjs.Keys.ToList().Select(i => (ulong)i + FLAG_NEW));
//            var fillObjs = new ulong[_constants.numTotalElements - _clObjArray.Count];
//            for (var i = 0; i < fillObjs.Length; i++)
//            {
//                fillObjs[i] = ulong.MaxValue;
//            }

            _constants.numTotalElements = (uint)_clObjArray.Count;
            if(DEBUG) _readLvlShapeElements = new clShapeIdTupel[numLvls * _constants.numTotalElements];

            #region CreateBuffers
            // Create the Buffers
            CreateBuffers();
            #endregion
            #region ReorderElements
           if(DEBUG) Debug.WriteLine("Execution time Create Buffers =" + (DateTime.Now - before).TotalMilliseconds);
             ClReorderElements();
            #endregion
           if(DEBUG) Debug.WriteLine("Execution time ReorderElements =" + (DateTime.Now - before).TotalMilliseconds);
             before = DateTime.Now;

            #region createCellList
            ClCreateCellId();
            #endregion

           if(DEBUG) Debug.WriteLine("Execution time CreateCellIdArray =" + (DateTime.Now - before).TotalMilliseconds);
             before = DateTime.Now;



            #region RadixSort

            ComputeEventList eventList = new ComputeEventList();

            //            ErrorCode error;
            //            Event eve;
            for (int i = 0; i < _constants.numLvls; i++) {
                sort.sortKeysValue(cl_lvlCellIds[i], cl_lvlCellData[i],(int) _lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT);


                cqCommandQueue.ReadFromBuffer(cl_lvlCellData[i], ref _readCellData[i], true ,eventList);
//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_lvlCellData[i], Bool.True, IntPtr.Zero, (IntPtr)(_lvlTotalElements[i] * GRID_FIELDS_PER_ELEMENT * CELL_DATA_ELEMENT_SIZE ),
//                _readCellData[i], 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");
            }

            if (DEBUG)
            {
                for (int i = 0; i < _constants.numLvls; i++) {
                    cqCommandQueue.ReadFromBuffer(cl_lvlCellIds[i], ref _readCellIds[i], true, eventList);

//                    error = Cl.EnqueueReadBuffer
//                        (cqCommandQueue, cl_lvlCellIds[i], Bool.True, IntPtr.Zero,
//                            (IntPtr) (_lvlTotalElements[i]*CELL_ID_ELEMENT_SIZE*GRID_FIELDS_PER_ELEMENT),
//                            _readCellIds[i], 0, null, out eve);
//                    CheckErr(error, "Cl.EnqueueReadBuffer");
                    for (int j = 0; j < _readCellIds[i].Count(); j++)
                    {
                        if (_cellMap.ContainsKey(_readCellIds[i][j]))
                        {
                            _readLvlCell2DPosList[i][j] = _cellMap[_readCellIds[i][j]];
                        }
                        else
                        {
                            _readLvlCell2DPosList[i][j] = new CollisionCell2D();
                        }
                    }

                                   DebugHelper.PrintCellIdBufferExtended( _readCellIds, _readCellData, _readLvlCell2DPosList, "CreateCellId Array", _constants, _lvlTotalElements);

                }

             //   DebugHelper.PrintCellIdBufferExtended(_readCellIds, _readCellData, _readLvlCell2DPosList, "CreateCellId Array", _constants, _lvlTotalElements);

                //DebugHelper.PrintCellIdBufferExtended(_debugReadInput, _readCellIds, _readCellData, _cell3DPosList, "Sorted Keys", (int)_constants.numTotalElements);

            }

            #endregion

           if(DEBUG) Debug.WriteLine("Execution time RadixSort =" + (DateTime.Now - before).TotalMilliseconds);
             before = DateTime.Now;

            #region CollisionList
            ClCreateNarrowCheckSublists();
            #endregion

           if(DEBUG) Debug.WriteLine("Execution time CreateCollisionList =" + (DateTime.Now - before).TotalMilliseconds);
             before = DateTime.Now;

            #region CreateCollisionTuples
            ClCreateCollsionTuples();
            #endregion
           if(DEBUG) Debug.WriteLine("Execution time CreateCollisionTuples =" + (DateTime.Now - before).TotalMilliseconds);
             before = DateTime.Now;
            #region CheckCollisions
            ClCheckCollsions();

            for (int i = 0; i < _constants.numLvls; i++) {
                    
                    // TODO: Call method with lvl idx -> Maybe rework all methods like that
                   if(DEBUG) Debug.WriteLine("Execution time CheckCollisions =" + (DateTime.Now - before).TotalMilliseconds);
                     before = DateTime.Now;

                    //TODO: Maybe do this threadwise to get some more performance.
                    for (int j = 0; j < _sharedIdx[i][0]; j++)
                    {
                        if (_collisionMap.ContainsKey(_readLvlCollisionList[i][j].obj1))
                        {
                            _collisionMap[_readLvlCollisionList[i][j].obj1].Add(_readLvlCollisionList[i][j].obj2);
                        }
                        else
                        {
                            HashSet<uint> tmp = new HashSet<uint>();
                            tmp.Add(_readLvlCollisionList[i][j].obj2);
                            _collisionMap.TryAdd(_readLvlCollisionList[i][j].obj1, tmp);
                        }
                        if (_collisionMap.ContainsKey(_readLvlCollisionList[i][j].obj2))
                        {
                            _collisionMap[_readLvlCollisionList[i][j].obj2].Add(_readLvlCollisionList[i][j].obj1);
                        }
                        else
                        {
                            HashSet<uint> tmp = new HashSet<uint>();
                            tmp.Add(_readLvlCollisionList[i][j].obj1);
                            _collisionMap.TryAdd(_readLvlCollisionList[i][j].obj2, tmp);
                        }
                    

                    // Check Results
                }
            }
  


            // RemoveSet
            var itemsToRemove = new HashSet<uint>();

            foreach (var act in _collisionMap.Keys) {
               if(DEBUG) Debug.Write(String.Format("ID: {0}: [",act));
                 foreach (var actVal in _collisionMap[act]) {
                   if(DEBUG) Debug.Write(String.Format("{0} ", actVal));
 
                }
               if(DEBUG) Debug.WriteLine("]");
             }

            // Call all delegates
            foreach (var actDele in m_AddDelegates.ToList())
            {
                if(actDele.Item2 == null) continue;
                uint tmpId = (uint)(actDele.Item1 & 0xFFFFFFFF);
                ISpatialEntity actAEntity = _objIdSpatialMap[tmpId];


                // TODO: Fresh added elements can collide.
                if (_collisionMap.ContainsKey(tmpId))
                {
                    // Add collided -> dont add item to environment
                    List<ISpatialEntity> collidedList = new List<ISpatialEntity>();//collisionMap[tmpId].Select(actObj => objIdSpatialMap[actObj]).ToList();
                    foreach (var actobjId in _collisionMap[tmpId].ToList())
                    {
                        collidedList.Add(_objIdSpatialMap[actobjId]);
                    }
                    var ret = new EnvironmentResult(collidedList);
                    actDele.Item2.Invoke(ret, actAEntity);
                    // itemsToRemove.Add(tmpId);
                }
                else
                {
                    // Add successful -> Add Element to environment and invoke corresponding delegate
                    actDele.Item2.Invoke(new EnvironmentResult(), actAEntity);
                }
                _EnvActEnvObjs.TryAdd(tmpId, _objIdClShapeMap[tmpId]);

            }
            foreach (var actDele in m_MoveDelegates.ToList())
            {
                if (actDele.Item2 == null) continue;

                uint tmpId = (uint)(actDele.Item1 & 0xFFFFFFFF);
                ISpatialEntity actAEntity = _objIdSpatialMap[tmpId];
                if (_collisionMap.ContainsKey(tmpId))
                {

                    List<ISpatialEntity> deleList = _collisionMap[tmpId].Select(actObj => _objIdSpatialMap[actObj]).ToList();
                    var ret = new EnvironmentResult(deleList);
                    actDele.Item2.Invoke(ret, actAEntity);
                }
                else
                {
                    actDele.Item2.Invoke(new EnvironmentResult(), actAEntity);
                }
            }
            foreach (var actDele in m_ExploreDelegates.ToList())
            {
                if (actDele.Item2 == null) continue;

                uint tmpId = (uint)(actDele.Item1 & 0xFFFFFFFF);
                if (_collisionMap.ContainsKey(tmpId))
                {
                    List<ISpatialEntity> deleList = _collisionMap[tmpId].Select(actObj => _objIdSpatialMap[actObj]).ToList();
                    var ret = new EnvironmentResult(deleList);
                    actDele.Item2.Invoke(ret);
                }
                else
                {
                    actDele.Item2.Invoke(new EnvironmentResult());
                }
            }



            foreach (var actRemove in m_ActRemoves.ToList())
            {
                if (actRemove.Item2 == null) continue;

                if (_spatialObjIdMap.ContainsKey(actRemove.Item1))
                {
                   if(DEBUG) Debug.WriteLine("Removing at delegate agentID: {0} ", actRemove.Item1);
                     var objId = _spatialObjIdMap[actRemove.Item1];
                    var entity = _objIdSpatialMap[objId];


                    //ulong objId = _spatialObjIdMap[entity.AgentGuid];
                    clCircleShapeObject tmp;
                    uint tmp2;
                    ISpatialEntity tmp3;
                    _EnvActEnvObjs.TryRemove(objId, out tmp);
                    _spatialObjIdMap.TryRemove(actRemove.Item1, out tmp2);
                    _objIdSpatialMap.TryRemove((uint)objId, out tmp3);

                    lock (_sync)
                    {
                        _objIdList.Remove(objId);
                        _shapeList.Remove(_objIdClShapeMap[(uint)objId]);
                    }
                    _objIdClShapeMap.TryRemove((uint)objId, out tmp);
                    actRemove.Item2?.Invoke(entity);

                }
                else
                {
                   if(DEBUG) Debug.WriteLine("Tried to remove {0} which does not exist", actRemove.Item1);
                 }


            }
            foreach (var act in m_ExploreAllDelegates.ToList())
            {

                var ret = new EnvironmentResult(_objIdSpatialMap.Values);
                act.Invoke(ret);
            }
            foreach (var act in itemsToRemove)
            {
                RemoveByObjId(act);
            }
            #endregion
            m_ActRemoves.Clear();
            m_AddDelegates.Clear();
            m_MoveDelegates.Clear();
            m_ExploreDelegates.Clear();
            m_ExploreAllDelegates.Clear();
            _EnvFreshAddedObjs.Clear();
            _EnvExploreObjs.Clear();
            _collisionMap.Clear();

            layerTickWait.Set();
            Thread.Sleep(10);
            layerTickWait.Reset();
            gpuActive = false;

            gpuActiveWait.Set();
            Thread.Sleep(10);
            gpuActiveWait.Reset();
           if(DEBUG) Debug.WriteLine("Execution time Call Delegates and Cleanup =" + (DateTime.Now - before).TotalMilliseconds);
             // before = DateTime.Now;


        }


        public ISpatialEntity GetSpatialEntity(Guid agentID)
        {
            if (_spatialObjIdMap.ContainsKey(agentID))
            {
                if (_objIdSpatialMap.ContainsKey(_spatialObjIdMap[agentID]))
                    return _objIdSpatialMap[_spatialObjIdMap[agentID]];
                throw new ArgumentException("ESC - Getspatialentity: Error spatial object not in internal obj id map - ID = " + agentID);
            }
           if(DEBUG) Debug.WriteLine("Agent with GUID: {0} not found", agentID);
             return null;
            //throw new ArgumentException("ESC - Getspatialentity: Spatial entity not added to ESC- ID = " + agentID);
        }


        private static clCircleShapeObject ConvertShapeToClShape(IShape shape)
        {
            clCircleShapeObject retval = new clCircleShapeObject();
            clPoint2D center = new clPoint2D();
            center.x = (float)shape.Bounds.Position.X;
            center.y = (float)shape.Bounds.Position.Y;
            retval.center = center;
            retval.radius = (float) shape.Bounds.Width/2;
            return retval;
        }

        public Vector3 MaxDimension
        {
            get
            {
                return _OuterBoundary;
            }
            set
            {
                _OuterBoundary = value;
                _constants.xMax = (float)value.X;
                _constants.yMax = (float)value.Y;
            }
        }

        public bool IsGrid
        {
            get { return true; }
            set { }
        }

        
    }
}
