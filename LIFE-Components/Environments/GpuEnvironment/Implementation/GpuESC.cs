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
using EnvironmentServiceComponent.SpatialAPI.Environment;
using GpuEnvironment.Helper;
using GpuEnvironment.Types;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace GpuEnvironment.Implementation
{
    public class GpuESC :IAsyncEnvironment {

        public static string LOG_BASE_PATH = Directory.GetCurrentDirectory();
//        public const string OPENCL_BASE_PATH = @"D:\Projects\lifev2\01-Code\LIFE\MARSLocalStarter\bin\Debug\layers\addins\WolvesModel\";

        private const int CELL_DATA_ELEMENT_SIZE = 8;
        private const int CELL_ID_ELEMENT_SIZE = 4;
        private const int GRID_FIELDS_PER_ELEMENT= 8;

        private bool DEBUG = false;

        private object _sync = new object();
        private static Random _random = new Random();
        // Auch Thread Blocks unter CUDA -> Gruppe von Threads mit gemeinsamen shared memory.
        private const int numBlocks = 4;
        private ulong actTick = 0;

        // Anzahl von WorkItems / Threads, die sich in einer Work-Group befinden
        private const int numThreadsPerBlock = 32;

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

      

        private ComputeBuffer<uint> cl_cellIds;
        private ComputeBuffer<ulong> cl_cellData;

        private ComputeBuffer<clRectangleShapeObject> cl_shapeMem;
        private ComputeBuffer<ulong> cl_objIdMem;

        private ComputeBuffer<CollisionTupel> cl_collisionTuples;
        private ComputeBuffer<CollisionTupel> cl_collisons;
        private ComputeBuffer<ShapeTupel> cl_collisionShapes;
        private ComputeBuffer<int> cl_sharedIdxMem;

        private ComputeBuffer<ulong> cl_collisionList;
        private ComputeBuffer<CollisionCell3D> cl_DebugCollisionPosList;
        private ComputeBuffer<uint> cl_freeCellMem;
        public CollisionConstants _constants;


        private List<clRectangleShapeObject> clShapeArray ;
        
            //shapes.
        private List<ulong> _clObjArray;
        private int[] _sharedIdx = new int[1];
        private bool[] _freeCellList;
        private int _sortElements;

        // Variables for reading the debug output
        private ulong[] _debugReadInput;
        private uint[] _readCellIds;
        private ulong[] _readCellData;
        private ulong[] _debugCollisionList;
        private CollisionCell3D[] _cell3DPosList;
        private CollisionTupel[] _tupelList;
        private CollisionTupel[] _debugCollisionTupelList;

        private ShapeTupel[] _collisionShapeTupels;
        private ConcurrentDictionary<uint, clRectangleShapeObject> _objIdClShapeMap;
        private ConcurrentDictionary<uint, ISpatialEntity> _objIdSpatialMap;// TODO: Maybe not needed
        private ConcurrentDictionary<Guid, uint> _spatialObjIdMap;
        private ConcurrentDictionary<uint, HashSet<uint>> _collisionMap;

        private ConcurrentDictionary<uint, clRectangleShapeObject> _EnvFreshAddedObjs;
        private ConcurrentDictionary<uint, clRectangleShapeObject> _EnvActEnvObjs;
        private ConcurrentDictionary<uint, clRectangleShapeObject> _EnvExploreObjs; 
 
        private List<uint> _objIdList;
        private List<clRectangleShapeObject> _shapeList;
        private ConcurrentDictionary<uint, CollisionCell3D> _cellMap;

        ComputeKernel ckCreateCellIdArray;
        ComputeKernel ckCreateCollisionList;
        ComputeKernel ckCreateCollsionTuples;
        ComputeKernel ckCheckCollsions;


        private Vector3 _OuterBoundary;
        private Vector3 _CellSize;
        
#endregion

        private bool checkBoundarys(Vector3 min, Vector3 max)
        {
            return min.X >= 0 && min.Y >= 0 && min.Z >= 0 && max.X <= _constants.xMax && max.Y <= _constants.yMax && max.Z <= _constants.zMax;
        }

        private uint getNextObjId()
        { 
            lock("s")
            {
                return objIdGenBase++; 
            }
            
        }

        private bool IsValidShape(IShape obj)
        {
            //TODO: implement.....
            return true;

        }

        public GpuESC(Vector3 maxElementSize, Vector3 enviromentSize)
        {
            m_ActRemoves = new List<Tuple<Guid, Action<ISpatialEntity>>>();
            _EnvFreshAddedObjs = new ConcurrentDictionary<uint, clRectangleShapeObject>();
            _EnvActEnvObjs = new ConcurrentDictionary<uint, clRectangleShapeObject>();
            _EnvExploreObjs = new ConcurrentDictionary<uint, clRectangleShapeObject>();
            m_ExploreDelegates = new List<Tuple<uint, ExploreDelegate>>();
            m_ExploreAllDelegates = new List< ExploreDelegate>();
            m_MoveDelegates = new List<Tuple<uint, MovementDelegate>>();
            m_AddDelegates = new List<Tuple<uint, MovementDelegate>>();
            _collisionMap = new ConcurrentDictionary<uint, HashSet<uint>>();
            _objIdClShapeMap = new ConcurrentDictionary<uint, clRectangleShapeObject>();
            _shapeList =  new List<clRectangleShapeObject>();
            _objIdSpatialMap = new ConcurrentDictionary<uint, ISpatialEntity>();// TODO: Maybe not needed
            _spatialObjIdMap = new ConcurrentDictionary<Guid, uint>();
            _collisionMap = new ConcurrentDictionary<uint, HashSet<uint>>();

        


            _OuterBoundary = enviromentSize;
            _CellSize = maxElementSize;
            _cellMap =new ConcurrentDictionary<uint, CollisionCell3D>();
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

        private void ContextNotify(string errInfo, byte[] data, IntPtr cb, IntPtr userData)
        {
            Debug.WriteLine("OpenCL Notification: " + errInfo);
        }

        private Vector3 GetNextFreeCell( Vector3 min, Vector3 max)
        {
            Vector3 res = new Vector3();
            uint xBegin = (uint)(min.X / _constants.cellSizeX);
            uint yBegin = (uint)(min.Y / _constants.cellSizeY);
            uint zBegin = (uint)(min.Z / _constants.cellSizeZ);
            uint xEnd = (uint)(max.X / _constants.cellSizeX);
            uint yEnd = (uint)(max.Y / _constants.cellSizeY);
            uint zEnd = (uint)(max.Z / _constants.cellSizeZ);
          /*  cellBegin.Y = min.Y / constants.cellSizeY;
            cellBegin.Z = min.Z / constants.cellSizeZ;
            cellEnd.X = max.X / constants.cellSizeX;
            cellEnd.Y = max.Y / constants.cellSizeY;
            cellEnd.Z = max.Z / constants.cellSizeZ;
*/

            for (uint x = xBegin; x < xEnd; x++)
            {
                for (uint y = yBegin; y < yEnd; y++)
                {
                    for (uint z = zBegin; z < zEnd; z++)
                    {
                        if (_freeCellList[
                            z*_constants.xGridBoundary*_constants.yGridBoundary 
                            + y*_constants.xGridBoundary 
                            + x
                            ])
                        {
                            res.X = x * _constants.cellSizeX + (_constants.cellSizeX / 2);
                            res.Y = y * _constants.cellSizeY + (_constants.cellSizeY / 2);
                            res.Z = z * _constants.cellSizeZ + (_constants.cellSizeZ / 2);
                            _freeCellList[z * _constants.xGridBoundary * _constants.yGridBoundary + y * _constants.xGridBoundary + x] = false;
                            return res;
                        }
                    }
                }
            }

            return Vector3.Null;
        }

        private void RemoveByObjId(uint objId)
        {
            lock (_sync) {
                _objIdList.Remove(objId);
            }
            uint tmp;
            while(!_spatialObjIdMap.TryRemove(_objIdSpatialMap[objId].AgentGuid, out tmp));
            ISpatialEntity tmp2;
            clRectangleShapeObject tmp3;
            _objIdSpatialMap.TryRemove((uint)objId, out tmp2);
            //Debug.WriteLine("Removed  Agent by objId {0}" ,tmp2.AgentGuid);

            _objIdClShapeMap.TryRemove((uint)objId, out tmp3);
            //shapeList.Remove(objIdClShapeMap[(uint)objId]);
            
        }

        private void InitOpenCl() {

            if (DEBUG)
            {
                //Debug.WriteLine(System.Reflection.Assembly.GetAssembly(typeof(GpuESC)).Location);
                //Debug.WriteLine(Path.Combine(LOG_BASE_PATH + "log.txt"));
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
            _constants.zMax = (float)_OuterBoundary.Z;
            _constants.cellSizeX = (float)_CellSize.X;
            _constants.cellSizeY = (float)_CellSize.Y;
            _constants.cellSizeZ = (float)_CellSize.Z;
            _constants.xGridBoundary = (uint)(_OuterBoundary.X / _CellSize.X);
            _constants.yGridBoundary = (uint)(_OuterBoundary.Y / _CellSize.Y);
            _constants.zGridBoundary = (uint)(_OuterBoundary.Z / _CellSize.Z);

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


            string programSource = GpuOpenClCode.CollisionDetection3DKernel;
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



//
//            string programSource = System.IO.File.ReadAllText(Path.Combine(OPENCL_BASE_PATH + "CollisionDetection.cl"));
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


            ckCreateCellIdArray = prog.CreateKernel("CreateCellId");
            ckCreateCollisionList = prog.CreateKernel("CreateCollisionList");
            ckCreateCollsionTuples = prog.CreateKernel("CreateCollsionTuples");
            ckCheckCollsions = prog.CreateKernel("CheckCollsions");
//            ckCreateCellIdArray = Cl.CreateKernel(clProgramCollision, "CreateCellId", out error);
//            CheckErr(error, "Cl.CreateKernel");
//            ckCreateCollisionList = Cl.CreateKernel(clProgramCollision, "CreateCollisionList", out error);
//            CheckErr(error, "Cl.CreateKernel");
//            ckCreateCollsionTuples = Cl.CreateKernel(clProgramCollision, "CreateCollsionTuples", out error);
//            CheckErr(error, "Cl.CreateKernel");
//            ckCheckCollsions = Cl.CreateKernel(clProgramCollision, "CheckCollsions", out error);
//            CheckErr(error, "Cl.CreateKernel");
//            _freeCellList = new bool[_constants.xGridBoundary*_constants.yGridBoundary*_constants.zGridBoundary];
           cl_freeCellMem = new ComputeBuffer<uint>(cxGPUContext,ComputeMemoryFlags.ReadWrite, 1);

        }





        public void Add(ISpatialEntity entity, MovementDelegate movementDelegate) {
            // First check boundarys
            Console.WriteLine("Added agent with id {0}", entity.AgentGuid);

            if (!checkBoundarys(entity.Shape.Bounds.LeftBottomFront, entity.Shape.Bounds.RightTopRear))
            {
                //                movementDelegate.Invoke(new EnvironmentResult());
                return;
            }
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            uint objId = getNextObjId();
           //ulong objData = objId | FLAG_NEW;
            lock (_sync) {
                m_AddDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
                _objIdList.Add(objId);
            }
            while (!_spatialObjIdMap.TryAdd(entity.AgentGuid, objId));




            while (!_objIdSpatialMap.TryAdd(objId, entity));

            clRectangleShapeObject act = ConvertShapeToClShape(entity.Shape);
            while (!_objIdClShapeMap.TryAdd(objId, act));
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
            if (!checkBoundarys(min , max ))
            {
                movementDelegate.Invoke(new EnvironmentResult(new List<ISpatialEntity>(), EnvironmentResultCode.ERROR_OUT_OF_BOUNDS),new DummySpatialEntity());
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
                freepos = new Vector3((double)rnd.Next((int)min.X, (int)max.X), (double)rnd.Next((int)min.Y, (int)max.Y), (double)rnd.Next((int)min.Z, (int)max.Z));
            }
            var objId = getNextObjId();
            var objData = objId | FLAG_NEW;

            lock (_sync) {
                m_AddDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
                _objIdList.Add(objId);
            }
            while (!_spatialObjIdMap.TryAdd(entity.AgentGuid, objId));
            while (!_objIdSpatialMap.TryAdd(objId, entity));
            entity.Shape = new Cuboid(entity.Shape.Bounds.Dimension, freepos);



            clRectangleShapeObject act = ConvertShapeToClShape(entity.Shape); 
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



        public void ExploreAll(ExploreDelegate exploreDelegate) {
            lock (_sync) {
                m_ExploreAllDelegates.Add(exploreDelegate);
            }
        }

        public void Remove(Guid agentId, Action<ISpatialEntity> removeDelegate)
        {
            lock (_sync) {
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

            lock (_sync) {
                _shapeList.Remove(_objIdClShapeMap[(uint) objId]);
                m_MoveDelegates.Add(new Tuple<uint, MovementDelegate>(objId, movementDelegate));
            }
            clRectangleShapeObject act = ConvertShapeToClShape(entity.Shape);
            lock (_sync) {
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
            lock (_sync) {
                m_MoveDelegates.Add(new Tuple<uint, MovementDelegate>(objId,movementDelegate));
            }


    
            var newEnt = entity;
            var newShape = newEnt.Shape.Transform(movementVector, rotation);
            newEnt.Shape = newShape;

            _objIdSpatialMap[(uint)objId].Shape = newShape;

            //shapeList.Remove(objIdClShapeMap[objId]);

            clRectangleShapeObject tmpRectangleShape;
            _EnvActEnvObjs.TryRemove(objId, out tmpRectangleShape);
            // TODO Testen ob es auch mit Konstruktoren funktioniert
            clRectangleShapeObject act = ConvertShapeToClShape(entity.Shape);


           _EnvActEnvObjs[objId] = act;
               //shapeList.Add(act);

           _objIdClShapeMap[(uint)objId] = act;

           Interlocked.Decrement(ref m_ActiveOperationCount);
        }

        public void Explore(IShape shape, ExploreDelegate exploreDelegate, Type agentType = null, int maxResults = 100)
        {
            
            if (gpuActive) gpuActiveWait.WaitOne();
            Interlocked.Increment(ref m_ActiveOperationCount);
            uint objId =(uint) Interlocked.Increment(ref exploreIdGenBase);

            lock (_sync) {
                _objIdList.Add(objId);
                m_ExploreDelegates.Add(new Tuple<uint, ExploreDelegate>(objId, exploreDelegate));
            }


            clRectangleShapeObject act = ConvertShapeToClShape(shape);

            _objIdClShapeMap[objId] = act;
            _EnvExploreObjs.TryAdd(objId, act);



            Interlocked.Decrement(ref m_ActiveOperationCount);
          

        }

#region CollisionHelper

        private void CreateBuffers()
        {
//            ErrorCode error;
//
//            Event eve;
            cl_collisionList =  new ComputeBuffer<ulong>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _constants.numTotalElements *4);
            cl_shapeMem = new ComputeBuffer<clRectangleShapeObject>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, clShapeArray.ToArray());
            cl_sharedIdxMem = new ComputeBuffer<int>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, _sharedIdx);
            cl_objIdMem = new ComputeBuffer<ulong>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, _clObjArray.ToArray());
            cl_cellData = new ComputeBuffer<ulong>(cxGPUContext, ComputeMemoryFlags.ReadWrite , _clObjArray.Count * GRID_FIELDS_PER_ELEMENT);
            cl_cellIds = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite , _clObjArray.Count * GRID_FIELDS_PER_ELEMENT);

            
            //                Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)( _constants.numTotalElements * CELL_ID_ELEMENT_SIZE * CELL_DATA_ELEMENT_SIZE),
            //                   out error);


            // Create Buffers
            //            cl_shapeMem = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)( _constants.numTotalElements * Marshal.SizeOf(typeof(clRectangleShapeObject))), clShapeArray.ToArray(),
            //                 out error);
            //            CheckErr(error, "Createbuffer");



            //            cl_sharedIdxMem = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(4), cl_sharedIdxMem,
            //                out error);
            //            CheckErr(error, "Createbuffer");
            //            cl_objIdMem = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_clObjArray.Count * CELL_DATA_ELEMENT_SIZE),
            //                out error);
            //            CheckErr(error, "Createbuffer");

            //            cl_cellData = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_clObjArray.Count * CELL_DATA_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
            //                out error);
            //            CheckErr(error, "Createbuffer");

            //            cl_cellIds = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_clObjArray.Count * CELL_ID_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT ),
            //                out error);
            //            CheckErr(error, "Createbuffer");
            if (DEBUG)
            {
                cl_DebugCollisionPosList = new ComputeBuffer<CollisionCell3D>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _constants.numTotalElements * GRID_FIELDS_PER_ELEMENT);
//                cl_DebugCollisionPosList = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)( _constants.numTotalElements * GRID_FIELDS_PER_ELEMENT * Marshal.SizeOf(typeof(CollisionCell3D))),
//                    out error);
            }


//            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_shapeMem, Bool.True, IntPtr.Zero, (IntPtr)(clShapeArray.Count * Marshal.SizeOf(typeof(clRectangleShapeObject))),
//                clShapeArray.ToArray(), 0, null,
//                out eve);
//            CheckErr(error, "EnqBuffer");
//
//            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_objIdMem, Bool.True, IntPtr.Zero, (IntPtr)(_clObjArray.Count * CELL_DATA_ELEMENT_SIZE),
//                _clObjArray.ToArray(), 0, null,
//                out eve);
//            CheckErr(error, "EnqBuffer");
//
//            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_sharedIdxMem, Bool.True, IntPtr.Zero, (IntPtr)(4),
//                0, 0, null,
//                out eve);
//            CheckErr(error, "EnqBuffer");

            /* error = Cl.EnqueueWriteBuffer(cqCommandQueue, freeCellMem, Bool.True, IntPtr.Zero,(IntPtr)(freeCellList.Length * Marshal.SizeOf(typeof(bool))),
                 freeCellList, 0, null,
                 out eve);
             CheckErr(error, "EnqBuffer");*/

//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");
        }

        private void CreateCellId()
        {
            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            ckCreateCellIdArray.SetMemoryArgument(0, cl_objIdMem);
            ckCreateCellIdArray.SetMemoryArgument(1, cl_shapeMem);
            ckCreateCellIdArray.SetMemoryArgument(2, cl_cellIds);
            ckCreateCellIdArray.SetMemoryArgument(3, cl_cellData);
            ckCreateCellIdArray.SetMemoryArgument(4, cl_freeCellMem);
            ckCreateCellIdArray.SetValueArgument(5, _constants);
            ckCreateCellIdArray.SetValueArgument(6, _constants.numTotalElements);
            if(DEBUG)
                ckCreateCellIdArray.SetMemoryArgument(7, cl_DebugCollisionPosList);
            cqCommandQueue.Execute(ckCreateCellIdArray, null, globalWorkSize, localWorkSize, eventList);
            cqCommandQueue.Finish();
            //            ErrorCode error;
            //            Event eve;
            // Enqueue create list kernel
//            IntPtr agentPtrSize = (IntPtr)0;
//            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));


//            uint globalWorkSize = _constants.numThreadsPerBlock *(uint) _constants.numBlocks;
//            uint localWorkSize = _constants.numThreadsPerBlock;
//
//            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
//            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
////            Event clevent;
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 0, ptrSize, cl_objIdMem);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 1, ptrSize, cl_shapeMem);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 2, ptrSize, cl_cellIds);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 3, ptrSize, cl_cellData);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 4, ptrSize, cl_freeCellMem);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 5, (IntPtr)(Marshal.SizeOf(typeof(CollisionConstants))), _constants);
//            CheckErr(error, "Cl.SetKernelArg");
//            error = Cl.SetKernelArg(ckCreateCellIdArray, 6, (IntPtr)4, _constants.numTotalElements);
//            CheckErr(error, "Cl.SetKernelArg");
//
//            if (DEBUG)
//            {
//                error = Cl.SetKernelArg(ckCreateCellIdArray, 7, ptrSize, cl_DebugCollisionPosList);
//                CheckErr(error, "Cl.SetKernelArg");
//            }
//
//            error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCreateCellIdArray, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
//            CheckErr(error, "Cl.EnqueueNDRangeKernel");
//
//
//            error = Cl.Finish(cqCommandQueue);
//            CheckErr(error, "Cl.Finish");


            /*   error = Cl.EnqueueReadBuffer(cqCommandQueue, freeCellMem, Bool.True, IntPtr.Zero, (IntPtr)(2),
                   freeCellList, 0, null, out eve);
               CheckErr(error, "Cl.EnqueueReadBuffer");*/


            if (DEBUG)
            {
                cqCommandQueue.ReadFromBuffer(cl_objIdMem , ref _debugReadInput,false,eventList);
                cqCommandQueue.ReadFromBuffer(cl_cellIds, ref _readCellIds, false, eventList);
                cqCommandQueue.ReadFromBuffer(cl_cellData, ref _readCellData, false, eventList);
                cqCommandQueue.ReadFromBuffer(cl_DebugCollisionPosList, ref _cell3DPosList, false, eventList);

                //                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_objIdMem, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * CELL_DATA_ELEMENT_SIZE),
                //                 _debugReadInput, 0, null, out eve);
                //                CheckErr(error, "Cl.EnqueueReadBuffer");

                //                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_cellIds, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * CELL_ID_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                //                _readCellIds, 0, null, out eve);
                //                CheckErr(error, "Cl.EnqueueReadBuffer");

                //
                //                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_cellData, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * CELL_DATA_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                //                _readCellData, 0, null, out eve);
                //                CheckErr(error, "Cl.EnqueueReadBuffer");

//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_DebugCollisionPosList, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT * Marshal.SizeOf(typeof(CollisionCell3D))),
//                _cell3DPosList, 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");
                for (int i = 0; i < _cell3DPosList.Length; i++)
                {
                    if (!_cellMap.ContainsKey(_readCellIds[i]))
                        _cellMap.TryAdd(_readCellIds[i], _cell3DPosList[i]);
                }

                DebugHelper.PrintCellIdBufferExtended(_debugReadInput, _readCellIds, _readCellData, _cell3DPosList, "CreateCellId Array",(int)_constants.numTotalElements);

                //DebugHelper.PrintCellIdBuffer(debugReadInput,readCellIdList,readCellData,"CreateCellId Array",constants.numTotalElements);

                //Debug.WriteLine();
            };
            cl_shapeMem.Dispose();
            cl_objIdMem.Dispose();
//            error = Cl.ReleaseMemObject(cl_shapeMem);
//            CheckErr(error, "Cl.ReleaseMemObj");
//            error = Cl.ReleaseMemObject(cl_objIdMem);
//            CheckErr(error, "Cl.ReleaseMemObj");
            //Cl.ReleaseMemObject(objIdMem);
        }

        private void CreateCollisionList()
        {

            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            ckCreateCollisionList.SetMemoryArgument(0, cl_cellIds);
            ckCreateCollisionList.SetMemoryArgument(1, cl_cellData);
            ckCreateCollisionList.SetMemoryArgument(2, cl_collisionList);
            ckCreateCollisionList.SetMemoryArgument(3, cl_sharedIdxMem);
            ckCreateCollisionList.SetValueArgument(4, _constants);
            ckCreateCollisionList.SetValueArgument(5, _constants.numTotalElements * GRID_FIELDS_PER_ELEMENT);
            cqCommandQueue.Execute(ckCreateCollisionList, null, globalWorkSize, localWorkSize, eventList);
            cqCommandQueue.Finish();
            //            ErrorCode error;
            //            Event eve;
            //            IntPtr agentPtrSize = (IntPtr)0;
            //            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));
            //
            //            int globalWorkSize = (int) _constants.numThreadsPerBlock * _constants.numBlocks;
            //            int localWorkSize = (int)_constants.numThreadsPerBlock;
            //
            //            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
            //            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
            //            Event clevent;
            //
            //
            //            // Create Collision List
            //            error = Cl.SetKernelArg(ckCreateCollisionList, 0, ptrSize, cl_cellIds);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollisionList, 1, ptrSize, cl_cellData);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollisionList, 2, ptrSize, cl_collisionList);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollisionList, 3, ptrSize, cl_sharedIdxMem);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollisionList, 4, (IntPtr)(Marshal.SizeOf(typeof(CollisionConstants))), _constants);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollisionList, 5, (IntPtr)4, _constants.numTotalElements * GRID_FIELDS_PER_ELEMENT);
            //            CheckErr(error, "Cl.SetKernelArg");
            //
            //            error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCreateCollisionList, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
            //            CheckErr(error, "Cl.EnqueueNDRangeKernel");
            //
            //            error = Cl.Finish(cqCommandQueue);
            //            CheckErr(error, "Cl.Finish");

            cqCommandQueue.ReadFromBuffer(cl_sharedIdxMem, ref _sharedIdx , true, eventList);
//            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_sharedIdxMem, Bool.True, IntPtr.Zero, (IntPtr)(4),
//                _sharedIdx, 0, null, out eve);
//            CheckErr(error, "Cl.EnqueueReadBuffer");



            if (DEBUG)
            {

                cqCommandQueue.ReadFromBuffer(cl_collisionList, ref _debugCollisionList, true, eventList);

//                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_collisionList, Bool.True, IntPtr.Zero, (IntPtr)(_sharedIdx[0] * GRID_FIELDS_PER_ELEMENT),
//                _debugCollisionList, 0, null, out eve);
//                CheckErr(error, "Cl.EnqueueReadBuffer");



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

                DebugHelper.PrintCollisionList(_debugCollisionList, _readCellIds, _readCellData, _cell3DPosList, "CollisionList", _sharedIdx[0]);
            }
        }

        private void CreateCollsionTuples()
        {
            ComputeEventList eventList = new ComputeEventList();

            cl_collisionTuples = new ComputeBuffer<CollisionTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _constants.numTotalElements * GRID_FIELDS_PER_ELEMENT);
            cqCommandQueue.WriteToBuffer(new int[]{0}, cl_sharedIdxMem, true, eventList);
            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };

            ckCreateCollsionTuples.SetMemoryArgument(0, cl_cellData);
            ckCreateCollsionTuples.SetMemoryArgument(1, cl_collisionList);
            ckCreateCollsionTuples.SetMemoryArgument(2, cl_collisionTuples);
            ckCreateCollsionTuples.SetMemoryArgument(3, cl_sharedIdxMem);
            ckCreateCollsionTuples.SetValueArgument(4, _constants);
            ckCreateCollsionTuples.SetValueArgument(5, _sharedIdx[0]);
            cqCommandQueue.Execute(ckCreateCollsionTuples, null, globalWorkSize, localWorkSize, eventList);
            cqCommandQueue.Finish();
            //            ErrorCode error;
            //            Event eve;
            //            IntPtr agentPtrSize = (IntPtr)0;
            //            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));
            //
            //            uint globalWorkSize = _constants.numThreadsPerBlock * (uint) _constants.numBlocks;
            //            uint localWorkSize = _constants.numThreadsPerBlock;
            //
            //            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
            //            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
            //            Event clevent;
            //                        cl_collisionTuples = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)( _constants.numTotalElements * GRID_FIELDS_PER_ELEMENT * Marshal.SizeOf(typeof(CollisionTupel))),
            //                            out error);
            //                        CheckErr(error, "Createbuffer");
            //            //
            //                        error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_sharedIdxMem, Bool.True, IntPtr.Zero, (IntPtr)(4),
            //                             0, 0, null, out eve);
            //            //Debug.WriteLine(" Num Elements {0} total {1}", _sharedIdx[0], _constants.numTotalElements);
            //            CheckErr(error, "EnqBuffer");
            //            error = Cl.SetKernelArg(ckCreateCollsionTuples, 0, ptrSize, cl_cellData);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollsionTuples, 1, ptrSize, cl_collisionList);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollsionTuples, 2, ptrSize, cl_collisionTuples);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollsionTuples, 3, ptrSize, cl_sharedIdxMem);
            //
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollsionTuples, 4, (IntPtr)(Marshal.SizeOf(typeof(CollisionConstants))), _constants);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCreateCollsionTuples, 5, (IntPtr)4, _sharedIdx[0]);
            //            CheckErr(error, "Cl.SetKernelArg");
            //
            //            error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCreateCollsionTuples, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
            //            CheckErr(error, "Cl.EnqueueNDRangeKernel");
            //
            //            error = Cl.Finish(cqCommandQueue);
            //            CheckErr(error, "Cl.Finish");

            cqCommandQueue.ReadFromBuffer(cl_sharedIdxMem, ref _sharedIdx, true, eventList);
            cqCommandQueue.ReadFromBuffer(cl_collisionTuples, ref _tupelList, true, eventList);

//            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_sharedIdxMem, Bool.True, IntPtr.Zero, (IntPtr)(4),
//                _sharedIdx, 0, null, out eve);
//            CheckErr(error, "Cl.EnqueueReadBuffer");

//            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_collisionTuples, Bool.True, IntPtr.Zero, (IntPtr)(_sharedIdx[0] * Marshal.SizeOf(typeof(CollisionTupel))),
//            _tupelList, 0, null, out eve);
//            CheckErr(error, "Cl.EnqueueReadBuffer");

            if (DEBUG)
            {

                for (int i = 0; i < _readCellIds.Count(); i++)
                {
                    if (_cellMap.ContainsKey(_readCellIds[i]))
                    {
                        _cell3DPosList[i] = _cellMap[_readCellIds[i]];
                    }
                    else
                    {
                        _cell3DPosList[i] = new CollisionCell3D();
                    }
                }
               
                DebugHelper.PrintCollisionTuples(_tupelList, _readCellIds, _readCellData, _cell3DPosList, "Collision tuples to check", _sharedIdx[0]);
            }
            cl_collisionList.Dispose();
//            error = Cl.ReleaseMemObject(cl_collisionList);
//            CheckErr(error, "Cl.ReleaseMem");

        }

        private void CheckCollsions()
        {
            long[] globalWorkSize = new long[] { _constants.numThreadsPerBlock * (uint)_constants.numBlocks };
            long[] localWorkSize = new long[] { _constants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();


//            ErrorCode error;
//            Event eve;
//            IntPtr agentPtrSize = (IntPtr)0;
//            var ptrSize = (IntPtr)Marshal.SizeOf(typeof(Mem));
//
////            uint globalWorkSize = _constants.numThreadsPerBlock * (uint) _constants.numBlocks;
////            uint localWorkSize = _constants.numThreadsPerBlock;
//
////            IntPtr[] workGroupSizePtr = new IntPtr[] { (IntPtr)globalWorkSize };
////            IntPtr[] localWorkGroupSizePtr = new IntPtr[] { (IntPtr)localWorkSize };
//            Event clevent;
//            cl_collisons = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_sharedIdx[0] * Marshal.SizeOf(typeof(CollisionTupel))),
//            out error);
//            CheckErr(error, "Createbuffer");
//
//            cl_collisionShapes = Cl.CreateBuffer(cxGPUContext, MemFlags.ReadWrite, (IntPtr)(_sharedIdx[0] * Marshal.SizeOf(typeof(ShapeTupel))),
//              out error);
//            CheckErr(error, "Createbuffer");
//            _collisionShapeTupels = new ShapeTupel[_sharedIdx[0]];


            for (int i = 0; i < _sharedIdx[0]; i++)
            {
                _collisionShapeTupels[i] = new ShapeTupel();
                uint keytmp = (uint)(_readCellData[_tupelList[i].obj1] & 0xFFFFFFFF);
                if(!_objIdClShapeMap.ContainsKey(keytmp))
                    Debug.WriteLine("Wrong Tempkey : "+keytmp);
                _collisionShapeTupels[i].obj1 = _objIdClShapeMap[keytmp];

                keytmp = (uint)(_readCellData[_tupelList[i].obj2] & 0xFFFFFFFF);

                _collisionShapeTupels[i].obj2 = _objIdClShapeMap[keytmp];
            }

            cl_collisons = new ComputeBuffer<CollisionTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite, _sharedIdx[0]);
            cl_collisionShapes = new ComputeBuffer<ShapeTupel>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, _collisionShapeTupels);

            ckCheckCollsions.SetMemoryArgument(0, cl_cellData);
            ckCheckCollsions.SetMemoryArgument(1, cl_collisionTuples);
            ckCheckCollsions.SetMemoryArgument(2, cl_collisionShapes);
            ckCheckCollsions.SetMemoryArgument(3, cl_collisons);
            ckCheckCollsions.SetMemoryArgument(3, cl_sharedIdxMem);
            ckCheckCollsions.SetValueArgument(4, _constants);
            ckCheckCollsions.SetValueArgument(5, _sharedIdx[0]);
            cqCommandQueue.Execute(ckCheckCollsions, null, globalWorkSize, localWorkSize, eventList);
            cqCommandQueue.Finish();
            // CheckCollsions
            //            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_sharedIdxMem, Bool.True, IntPtr.Zero, (IntPtr)(4),
            //                     0, 0, null, out eve);
            //            CheckErr(error, "EnqBuffer");
            //            error = Cl.EnqueueWriteBuffer(cqCommandQueue, cl_collisionShapes, Bool.True, IntPtr.Zero, (IntPtr)(_sharedIdx[0] * Marshal.SizeOf(typeof(ShapeTupel))),
            //                                 _collisionShapeTupels, 0, null, out eve);
            //
            //            CheckErr(error, "EnqBuffer");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 0, ptrSize, cl_cellData);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 1, ptrSize, cl_collisionTuples);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 2, ptrSize, cl_collisionShapes);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 3, ptrSize, cl_collisons);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 4, ptrSize, cl_sharedIdxMem);
            //
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 5, (IntPtr)(Marshal.SizeOf(typeof(CollisionConstants))), _constants);
            //            CheckErr(error, "Cl.SetKernelArg");
            //            error = Cl.SetKernelArg(ckCheckCollsions, 6, (IntPtr)4, _sharedIdx[0]);
            //            CheckErr(error, "Cl.SetKernelArg");
            //
            //            error = Cl.EnqueueNDRangeKernel(cqCommandQueue, ckCheckCollsions, 1, null, workGroupSizePtr, localWorkGroupSizePtr, 0, null, out clevent);
            //            CheckErr(error, "Cl.EnqueueNDRangeKernel");
            //
            //            error = Cl.Finish(cqCommandQueue);
            //            CheckErr(error, "Cl.Finish");

            cqCommandQueue.ReadFromBuffer(cl_sharedIdxMem, ref _sharedIdx, true, eventList);

//            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_sharedIdxMem, Bool.True, IntPtr.Zero, (IntPtr)(4),
//                 _sharedIdx, 0, null, out eve);
//            CheckErr(error, "Cl.EnqueueReadBuffer");


            _tupelList = new CollisionTupel[_sharedIdx[0]];
            cqCommandQueue.ReadFromBuffer(cl_collisons, ref _tupelList, true, eventList);

//            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_collisons, Bool.True, IntPtr.Zero, (IntPtr)(_sharedIdx[0] * Marshal.SizeOf(typeof(CollisionTupel))),
//            _tupelList, 0, null, out eve);
//
//            CheckErr(error, "Cl.EnqueueReadBuffer");

        }
#endregion

        public COMMIT_RESULT Commit()
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
             _constants.numTotalElements = (uint)( _EnvActEnvObjs.Count + _EnvExploreObjs.Count + _EnvFreshAddedObjs.Count);

            if (_constants.numTotalElements == 0)
                return COMMIT_RESULT.OK;

            if (DEBUG)
            {
                _debugReadInput = new ulong[_constants.numTotalElements];
                _debugCollisionList = new ulong[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
            }
            _cell3DPosList = new CollisionCell3D[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
            _tupelList = new CollisionTupel[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];     
            _readCellData = new ulong[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
            _readCellIds = new uint[_constants.numTotalElements * GRID_FIELDS_PER_ELEMENT];
           
            clShapeArray = _EnvActEnvObjs.Values.ToList();
            clShapeArray.AddRange(_EnvExploreObjs.Values.ToList());
            clShapeArray.AddRange(_EnvFreshAddedObjs.Values.ToList());

            //shapes.
            _clObjArray = _EnvActEnvObjs.Keys.ToList().Select(i => (ulong)i + FLAG_MOVE).ToList();
            _clObjArray.AddRange(_EnvExploreObjs.Keys.ToList().Select(i => (ulong)i + FLAG_EXLORE));
            _clObjArray.AddRange(_EnvFreshAddedObjs.Keys.ToList().Select(i => (ulong)i + FLAG_NEW));
            var fillObjs = new ulong[_constants.numTotalElements- _clObjArray.Count];
            for (var i = 0; i < fillObjs.Length; i++) {
                fillObjs[i] = ulong.MaxValue;
            }

            _constants.numTotalElements =  (uint)_clObjArray.Count;

            try
            {
                #region CreateBuffers
                // Create the Buffers
                CreateBuffers();
                #endregion

                Debug.WriteLine("Execution time Create Buffers =" + (DateTime.Now - before).TotalMilliseconds);
                before = DateTime.Now;

                #region createCellList
                CreateCellId();
                #endregion

                Debug.WriteLine("Execution time CreateCellIdArray =" + (DateTime.Now - before).TotalMilliseconds);
                before = DateTime.Now;



                #region RadixSort

                ComputeEventList eventList = new ComputeEventList();

                //            ErrorCode error;
                //            Event eve;

                sort.sortKeysValue(cl_cellIds, cl_cellData, (int)_constants.numTotalElements * CELL_DATA_ELEMENT_SIZE);

                cqCommandQueue.ReadFromBuffer(cl_cellData, ref _readCellData, true, eventList);

                //            error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_cellData, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * CELL_DATA_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                //            _readCellData, 0, null, out eve);
                //            CheckErr(error, "Cl.EnqueueReadBuffer");
                if (DEBUG)
                {
                    cqCommandQueue.ReadFromBuffer(cl_cellIds, ref _readCellIds, true, eventList);

                    //                error = Cl.EnqueueReadBuffer(cqCommandQueue, cl_cellIds, Bool.True, IntPtr.Zero, (IntPtr)(_constants.numTotalElements * CELL_ID_ELEMENT_SIZE * GRID_FIELDS_PER_ELEMENT),
                    //                _readCellIds, 0, null, out eve);
                    //                CheckErr(error, "Cl.EnqueueReadBuffer");

                    for (int i = 0; i < _readCellIds.Count(); i++)
                    {
                        if (_cellMap.ContainsKey(_readCellIds[i]))
                        {
                            _cell3DPosList[i] = _cellMap[_readCellIds[i]];
                        }
                        else
                        {
                            _cell3DPosList[i] = new CollisionCell3D();
                        }
                    }

                    DebugHelper.PrintCellIdBufferExtended(_debugReadInput, _readCellIds, _readCellData, _cell3DPosList, "Sorted Keys", (int)_constants.numTotalElements);

                }

                #endregion

                Debug.WriteLine("Execution time RadixSort =" + (DateTime.Now - before).TotalMilliseconds);
                before = DateTime.Now;

                #region CollisionList
                CreateCollisionList();
                #endregion

                Debug.WriteLine("Execution time CreateCollisionList =" + (DateTime.Now - before).TotalMilliseconds);
                before = DateTime.Now;

                #region CreateCollisionTuples
                CreateCollsionTuples();
                #endregion
                Debug.WriteLine("Execution time CreateCollisionTuples =" + (DateTime.Now - before).TotalMilliseconds);
                before = DateTime.Now;
                #region CheckCollisions
                if (_sharedIdx[0] > 0)
                {
                    CheckCollsions();
                    Debug.WriteLine("Execution time CheckCollisions =" + (DateTime.Now - before).TotalMilliseconds);
                    before = DateTime.Now;

                    //TODO: Maybe do this threadwise to get some more performance.
                    for (int i = 0; i < _sharedIdx[0]; i++)
                    {
                        if (_collisionMap.ContainsKey(_tupelList[i].obj1))
                        {
                            _collisionMap[_tupelList[i].obj1].Add(_tupelList[i].obj2);
                        }
                        else
                        {
                            HashSet<uint> tmp = new HashSet<uint>();
                            tmp.Add(_tupelList[i].obj2);
                            _collisionMap.TryAdd(_tupelList[i].obj1, tmp);
                        }
                        if (_collisionMap.ContainsKey(_tupelList[i].obj2))
                        {
                            _collisionMap[_tupelList[i].obj2].Add(_tupelList[i].obj1);
                        }
                        else
                        {
                            HashSet<uint> tmp = new HashSet<uint>();
                            tmp.Add(_tupelList[i].obj1);
                            _collisionMap.TryAdd(_tupelList[i].obj2, tmp);
                        }
                    }

                    // Check Results
                }


                // RemoveSet
                var itemsToRemove = new HashSet<uint>();
                // Call all delegates
                foreach (var actDele in m_AddDelegates.ToList())
                {
                    uint tmpId = (uint)(actDele.Item1 & 0xFFFFFFFF);
                    ISpatialEntity actAEntity = _objIdSpatialMap[tmpId];


                    // TODO: Fresh added elements can collide.
                    /*                if (_collisionMap.ContainsKey(tmpId))
                                    {
                                        // Add collided -> dont add item to environment
                                        List<ISpatialEntity> deleList = new List<ISpatialEntity>();//collisionMap[tmpId].Select(actObj => objIdSpatialMap[actObj]).ToList();
                                        foreach (var actobjId in _collisionMap[tmpId].ToList())
                                        {
                                            deleList.Add(_objIdSpatialMap[actobjId]);
                                        }
                                        var ret = new EnvironmentResult(deleList);
                                        actDele.Item2.Invoke(ret, actAEntity);
                                       // itemsToRemove.Add(tmpId);
                                    }
                                    else*/
                    {
                        // Add successful -> Add Element to environment and invoke corresponding delegate
                        actDele.Item2.Invoke(new EnvironmentResult(), actAEntity);
                        _EnvActEnvObjs.TryAdd(tmpId, _objIdClShapeMap[tmpId]);
                    }
                }
                foreach (var actDele in m_MoveDelegates.ToList())
                {
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
                    if (_spatialObjIdMap.ContainsKey(actRemove.Item1))
                    {
                        //Debug.WriteLine("Removing at delegate agentID: {0} ", actRemove.Item1);
                        var objId = _spatialObjIdMap[actRemove.Item1];
                        var entity = _objIdSpatialMap[objId];


                        //ulong objId = _spatialObjIdMap[entity.AgentGuid];
                        clRectangleShapeObject tmp;
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
                        Debug.WriteLine("Tried to remove {0} which does not exist", actRemove.Item1);
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
                Debug.WriteLine("Execution time Call Delegates and Cleanup =" + (DateTime.Now - before).TotalMilliseconds);
                // before = DateTime.Now;

            }
            catch (ComputeException e)
            {
                Console.WriteLine(e);
                return COMMIT_RESULT.MEMORY_ERROR;
            }

          return COMMIT_RESULT.OK;

        }


        public ISpatialEntity GetSpatialEntity(Guid agentID) {
            if (_spatialObjIdMap.ContainsKey(agentID)) {
                if (_objIdSpatialMap.ContainsKey(_spatialObjIdMap[agentID]))
                    return _objIdSpatialMap[_spatialObjIdMap[agentID]];
                throw new ArgumentException("ESC - Getspatialentity: Error spatial object not in internal obj id map - ID = "+ agentID);
            }
            //Debug.WriteLine("Agent with GUID: {0} not found", agentID);
            return null;
            //throw new ArgumentException("ESC - Getspatialentity: Spatial entity not added to ESC- ID = " + agentID);
        }


        private static clRectangleShapeObject ConvertShapeToClShape(IShape shape) {
            clRectangleShapeObject retval = new clRectangleShapeObject();
            clPoint3D center;
            center.x = (float)shape.Bounds.Position.X;
            center.y = (float)shape.Bounds.Position.Y;
            center.z = (float)shape.Bounds.Position.Z;
            clPoint3D front;
            front.x = (float)shape.Bounds.LeftBottomFront.X;
            front.y = (float)shape.Bounds.LeftBottomFront.Y;
            front.z = (float)shape.Bounds.LeftBottomFront.Z;
            clPoint3D rear;
            rear.x = (float)shape.Bounds.RightTopRear.X;
            rear.y = (float)shape.Bounds.RightTopRear.Y;
            rear.z = (float)shape.Bounds.RightTopRear.Z;
            retval.center = center;
            retval.leftBottomFront = front;
            retval.rigthTopRear = rear;
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
                _constants.xMax = (float) value.X;
                _constants.yMax = (float) value.Y;
                _constants.zMax = (float) value.Z;
            }
        }

        public bool IsGrid
        {
            get { return true; }
            set { }
        }

    }
}
