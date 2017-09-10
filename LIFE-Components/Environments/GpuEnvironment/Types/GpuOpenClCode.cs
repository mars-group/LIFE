namespace GpuEnvironment.Types
{
    public class GpuOpenClCode
    {
        public static string RadixSortKernel = @"

        typedef struct GPUConstants
        {
             int numRadices;
             int numBlocks;
             int numGroupsPerBlock;
             int R;
             int numThreadsPerGroup;
             int numElementsPerGroup;
             int numRadicesPerBlock;
             int bitMask;
             int L;
             int numThreadsPerBlock;
             int numTotalElements;
        }Constants;

        inline void PrefixLocal(__local uint* inout, int p_length, int numThreads){
            __private uint glocalID = get_local_id(0);
            __private int inc = 2;

            // reduce
            while(inc <= p_length){
        
                for(int i = ((inc>>1) - 1) + (glocalID * inc)  ; (i + inc) < p_length ; i+= numThreads*inc){
                    inout[i + (inc>>1)] = inout[i] + inout[i + (inc>>1)];
                }
                inc = inc <<1;
                barrier(CLK_LOCAL_MEM_FENCE);
            }
            // Downsweep
            inout[p_length-1] = 0;
            barrier(CLK_LOCAL_MEM_FENCE);

            while(inc >=2){
                for (int i = ((inc>>1) - 1) + (glocalID * inc)  ; (i + (inc>>1)) <= p_length ; i+= numThreads*inc)
                {
                    uint tmp = inout[i + (inc >>1)];
                    inout[i + (inc >>1)] = inout[i] + inout[i + (inc >>1 )];
                    inout[i] = tmp;
                }
                inc = inc>>1;
                barrier(CLK_LOCAL_MEM_FENCE);
            }
        }

        inline void PrefixGlobal(__global uint* inout, int p_length, int numThreads){
            __private uint glocalID = get_local_id(0);
            __private int inc = 2;

            // reduce
            while(inc <= p_length){
        
                for(int i = ((inc>>1) - 1) + (glocalID * inc)  ; (i + inc) < p_length ; i+= numThreads*inc){
                    inout[i + (inc>>1)] = inout[i] + inout[i + (inc>>1)];
                }
                inc = inc <<1;
                barrier(CLK_LOCAL_MEM_FENCE);
            }
            // Downsweep
            inout[p_length-1] = 0;
            barrier(CLK_LOCAL_MEM_FENCE);

            while(inc >=2){
                for (int i = ((inc>>1) - 1) + (glocalID * inc)  ; (i + (inc>>1)) <= p_length ; i+= numThreads*inc)
                {
                    uint tmp = inout[i + (inc >>1)];
                    inout[i + (inc >>1)] = inout[i] + inout[i + (inc >>1 )];
                    inout[i] = tmp;
                }
                inc = inc>>1;
                barrier(CLK_LOCAL_MEM_FENCE);
            }
        }



        __kernel void SetupAndCount(    __global  uint* cellIdIn, 
                                    __global volatile uint* counters,
                                    Constants dConst, 
                                    uint bitOffset)
        {
            __private uint gLocalId = get_local_id(0);
            __private uint gBlockId = get_group_id(0);
    
            // Current threadGroup -> is based of the localId(Block internal)
            __private uint threadGroup = gLocalId / dConst.R;

            // Startindex of the datablock that corresponds to the Threadblock
            __private int actBlock = gBlockId * dConst.numGroupsPerBlock * dConst.numElementsPerGroup ;
    
            // Offset inside the block for the threadgroup of the current thread
            __private int actGroup = (gLocalId / dConst.R ) * dConst.numElementsPerGroup;

            // Startindex for the current thread
            __private uint idx = actBlock + actGroup + gLocalId % dConst.R;
    
            // Set the boarder
            __private int boarder = actBlock +actGroup + dConst.numElementsPerGroup;
            boarder = (boarder > dConst.numTotalElements)? dConst.numTotalElements : boarder;
    
            // Number of counters for each radix
            __private uint countersPerRadix = dConst.numBlocks * dConst.numGroupsPerBlock;
            // Each Threadgroup has its own counter for each radix -> Calculating offset based on current block
            __private uint counterGroupOffset = gBlockId * dConst.numGroupsPerBlock;

            for(;idx < boarder; idx += dConst.numThreadsPerGroup){
                __private uint actRadix = (cellIdIn[idx] >> bitOffset) & dConst.bitMask;
                // The following code ensures that the counters of each Threadgroup are sequentially incremented
                 for(uint tmpIdx = 0 ; tmpIdx < dConst.R; tmpIdx++){
                    if(gLocalId % dConst.R == tmpIdx){
                        counters[ (actRadix * countersPerRadix)  +counterGroupOffset+ threadGroup ]++;
                    }
                    barrier(CLK_GLOBAL_MEM_FENCE);
                }
            }
        }

        __kernel void SumIt(    __global uint* cellIdIn, 
                                    __global volatile uint* counters,
                                    __global uint* radixPrefixes,
                                     Constants dConst, 
                                    uint bitOffset,
                                    __local uint* groupcnt)
        {
            __private uint globalId = get_global_id(0);
            __private uint gLocalId = get_local_id(0);
            __private uint gBlockId = get_group_id(0);
    

            __private uint countersPerRadix = dConst.numBlocks * dConst.numGroupsPerBlock;


            __private uint actRadix = dConst.numRadicesPerBlock * gBlockId;

            for(int i = 0 ; i< dConst.numRadicesPerBlock; i++){
                // The Num_Groups counters of the radix are read from global memory to shared memory.
                // Jeder Thread liest die Counter basierend auf der localid aus
                int numIter = 0;
                uint boarder = ((actRadix+1) * countersPerRadix);
               // boarder = (boarder > dConst.numBlocks * dConst.numGroupsPerBlock)? dConst.numBlocks * dConst.numGroupsPerBlock : boarder;
                for(int j = (actRadix * countersPerRadix) + gLocalId ; j < boarder; j+= dConst.numThreadsPerBlock){
                    groupcnt[gLocalId + dConst.numThreadsPerBlock * numIter++] = counters[j];
                    //numIter += dConst.numThreadsPerBlock;
                }

                barrier(CLK_LOCAL_MEM_FENCE);

                // Die einzelnen RadixCounter sind nun in dem groupcnt local Memory
                //prefixSum(&counters[actRadix * dConst.numBlocks * dConst.numGroupsPerBlock], groupcnt, tmpPrefix,dConst.numBlocks * dConst.numGroupsPerBlock);
                PrefixLocal(groupcnt, countersPerRadix ,dConst.numThreadsPerBlock );
        
                // PrefixSum wurde gebildet..
                barrier(CLK_LOCAL_MEM_FENCE);

                // Gesamtprefix für den aktuellen radix berechnen
                if(gLocalId == 1 ){
                    radixPrefixes[actRadix] = groupcnt[(countersPerRadix) -1] + counters[((actRadix+1) * countersPerRadix)-1];
                }

                // Errechnete Prefixsumme zurück in den global memory schreiben
                barrier(CLK_GLOBAL_MEM_FENCE);
                numIter = 0;
                for(int j = (actRadix * countersPerRadix) + gLocalId ; j < ((actRadix+1) * countersPerRadix); j+= dConst.numThreadsPerBlock){
                    counters[j] = groupcnt[gLocalId + dConst.numThreadsPerBlock * numIter++];
                    //numIter += dConst.numThreadsPerBlock;
                }
                barrier(CLK_GLOBAL_MEM_FENCE);
                actRadix++;
            }
        }




        __kernel void ReorderingKeysOnly(    __global uint* cellIdIn, 
                                    __global uint* cellIdOut,
                                    __global uint* counters,
                                    __global uint* radixPrefixes,
                                    __local uint* localCounters,
                                    __local uint* localPrefix,
                                     Constants dConst, 
                                    uint bitOffset)
        {
            int globalId = get_global_id(0);
            __private uint gLocalId = get_local_id(0);
            const uint gBlockId = get_group_id(0);
            const uint threadGroup= gLocalId / dConst.R;
            const uint threadGroupId= gLocalId % dConst.R;
            const uint actRadix = dConst.numRadicesPerBlock * gBlockId;
            const uint countersPerRadix = dConst.numGroupsPerBlock * dConst.numBlocks;


            __private int radixCounterOffset = actRadix * countersPerRadix;

            // erst abschließen der radix summierung
            __private  uint blockidx ; 


            // Read radix prefixes to localMemory
            for(int i = gLocalId ; i< dConst.numRadices ; i+= dConst.numThreadsPerBlock){
                localPrefix[i] = radixPrefixes[i];
            }
            // Präfixsumme über die RadixCounter bilden.
            barrier(CLK_LOCAL_MEM_FENCE);
            PrefixLocal(localPrefix, dConst.numRadices, dConst.numThreadsPerBlock);
            barrier(CLK_LOCAL_MEM_FENCE);



            // Load (groups per block * radices) counters, i.e., the block column
          //for (uint i = threadGroupId; i < dConst.numRadices; i+= dConst.numThreadsPerGroup) {
           // localCounters[threadGroup+ dConst.numGroupsPerBlock * i] = counters[countersPerRadix * i + gBlockId * dConst.numGroupsPerBlock + threadGroup] + localPrefix[i];
    
         // }

          for (uint i = 0; i < dConst.numGroupsPerBlock; ++i) {
            if (gLocalId < dConst.numRadices) {
              localCounters[gLocalId *  dConst.numGroupsPerBlock + i]  = counters[gLocalId * countersPerRadix + gBlockId * dConst.numGroupsPerBlock + i] + localPrefix[gLocalId];
            }
          }

            barrier(CLK_LOCAL_MEM_FENCE);

            const uint group = gLocalId / dConst.numThreadsPerGroup;
            const uint group_offset = gBlockId * dConst.numGroupsPerBlock;
            const uint group_thread = gLocalId % dConst.numThreadsPerGroup;
            const uint elem_offset = (group_offset + group) * dConst.numElementsPerGroup;
            const uint start = elem_offset + group_thread;
            const uint end = min((uint) elem_offset +dConst.numElementsPerGroup,(uint)  dConst.numTotalElements);

            for (uint i = start; i < end; i += dConst.numThreadsPerGroup) {
                const uint bits = (cellIdIn[i] >> bitOffset) & dConst.bitMask;
                const uint index = (bits * dConst.numGroupsPerBlock) + group;
                for (uint j = 0; j < dConst.numThreadsPerGroup; ++j) {
                  if (group_thread == j) {
                    cellIdOut[localCounters[index]] = cellIdIn[i];
                    ++localCounters[index];
                  }
                  barrier(CLK_LOCAL_MEM_FENCE);
                }
            }

        }








        __kernel void ReorderingKeyValue(    __global uint* cellIdIn, 
                                    __global uint* cellIdOut,
                                    __global ulong* valueIn,
                                    __global ulong* valueOut,
                                    __global uint* counters,
                                    __global uint* radixPrefixes,
                                    __local uint* localCounters,
                                    __local uint* localPrefix,
                                     Constants dConst, 
                                    uint bitOffset)
        {

            int globalId = get_global_id(0);
            __private uint gLocalId = get_local_id(0);
            const uint gBlockId = get_group_id(0);
            const uint threadGroup= gLocalId / dConst.R;
            const uint threadGroupId= gLocalId % dConst.R;
            const uint actRadix = dConst.numRadicesPerBlock * gBlockId;
            const uint countersPerRadix = dConst.numGroupsPerBlock * dConst.numBlocks;


            __private int radixCounterOffset = actRadix * countersPerRadix;

            // erst abschließen der radix summierung
            __private  uint blockidx ; 


            // Read radix prefixes to localMemory
            for(int i = gLocalId ; i< dConst.numRadices ; i+= dConst.numThreadsPerBlock){
                localPrefix[i] = radixPrefixes[i];
            }
            // Präfixsumme über die RadixCounter bilden.
            barrier(CLK_LOCAL_MEM_FENCE);
            PrefixLocal(localPrefix, dConst.numRadices, dConst.numThreadsPerBlock);
            barrier(CLK_LOCAL_MEM_FENCE);



            // TODO: 
            // Load (groups per block * radices) counters, i.e., the block column
          //for (uint i = threadGroupId; i < dConst.numRadices; i+= dConst.numThreadsPerGroup) {
           // localCounters[threadGroup+ dConst.numGroupsPerBlock * i] = counters[countersPerRadix * i + gBlockId * dConst.numGroupsPerBlock + threadGroup] + localPrefix[i];
    
         // }

          for (uint i = 0; i < dConst.numGroupsPerBlock; ++i) {
            if (gLocalId < dConst.numRadices) {
              localCounters[gLocalId *  dConst.numGroupsPerBlock + i]  = counters[gLocalId * countersPerRadix + gBlockId * dConst.numGroupsPerBlock + i] + localPrefix[gLocalId];
            }
          }



            barrier(CLK_LOCAL_MEM_FENCE);

            const uint group = gLocalId / dConst.numThreadsPerGroup;
            const uint group_offset = gBlockId * dConst.numGroupsPerBlock;
            const uint group_thread = gLocalId % dConst.numThreadsPerGroup;
            const uint elem_offset = (group_offset + group) * dConst.numElementsPerGroup;
            const uint start = elem_offset + group_thread;
            const uint end = min((uint) elem_offset +dConst.numElementsPerGroup,(uint)  dConst.numTotalElements);

            for (uint i = start; i < end; i += dConst.numThreadsPerGroup) {
                const uint bits = (cellIdIn[i] >> bitOffset) & dConst.bitMask;
                const uint index = (bits * dConst.numGroupsPerBlock) + group;
                for (uint j = 0; j < dConst.numThreadsPerGroup; ++j) {
                  if (group_thread == j) {
                    cellIdOut[localCounters[index]] = cellIdIn[i];
                    valueOut[localCounters[index]] = valueIn[i];
                    ++localCounters[index];
                  }
                  barrier(CLK_LOCAL_MEM_FENCE);
                }
            }
        }
        ";

        public static string CollisionDetection3DKernel = @"
        #define NUM_PHANTOMCELLS 7
#define HOMECELL (((long)1)<<32)
#define PHANTOMCELL (((long)1)<<33)
#define UNUSED (((long)1)<<34)


#define NEW (((long)1)<<35)
#define MOVE (((long)1)<<36)
#define EXPLORE (((long)1)<<37)


//#define DEBUG


typedef struct CollisionConstants
{
	int numThreadsPerBlock;
	int numTotalElements;
	float xMax;
	float yMax;
	float zMax;
	float cellSizeX;
	float cellSizeY;
	float cellSizeZ;
	int xGridBoundary;
	int yGridBoundary;
	int zGridBoundary;
	int numBlocks;
	}CollisionConstants;

typedef struct ObjectData
{
int objectId;
char cellType;

}ObjectData;


typedef struct clPoint
{
float x;
float y;
float z;
}clPoint;

typedef struct CollisionTupel
{
int obj1;
int obj2;
}CollisionTupel;


typedef struct clShapeObject
{
clPoint center;
clPoint leftBottomFront;
clPoint rightTopRear;
}clShapeObject;

typedef struct ShapeTupel
{
clShapeObject obj1;
clShapeObject obj2;
}ShapeTupel;

typedef struct CollisionCell
{
	int x;
	int y;
	int z;
	}CollisionCell;

inline int calculateCellHash(CollisionCell cell, CollisionConstants gConst){

	return (int) (cell.z*gConst.yMax*gConst.xMax + cell.y*gConst.xMax + cell.x);
}

inline int calculateHash(int x , int y ,int z, CollisionConstants gConst){

	return (int) (z*gConst.yMax*gConst.xMax + y*gConst.xMax + x);
}

// Input ist die Absolute Position des Elements
// Output ist CollisionCell in dem 
inline CollisionCell getCell(clPoint p_Point, float cellSizeX, float cellSizeY, float cellSizeZ){
	__private CollisionCell cell;
	cell.x = (int) p_Point.x /cellSizeX;
	cell.y = (int) p_Point.y /cellSizeY;
	cell.z = (int) p_Point.z /cellSizeZ;
	return cell;
}

inline CollisionCell getDebugCell(float x,float y,float z,CollisionConstants gConst){
	__private CollisionCell cell;
	cell.x = (int) x;
	cell.y = (int) y;
	cell.z = (int) z;
	return cell;
}

inline void setCellFull(CollisionCell cell, __global bool* freeCells, CollisionConstants gConst ){
	freeCells[(int)(cell.z*gConst.yGridBoundary*gConst.xGridBoundary + cell.y*gConst.xGridBoundary + cell.x)] = false;
}
inline bool Equals(CollisionCell cell1, CollisionCell cell2){

	return cell1.x == cell2.x && cell1.y == cell2.y && cell1.z == cell2.z;
}



#ifdef DEBUG
	inline void fillCell(	int x, int y, int z,
							CollisionConstants dConst,
							__global  CollisionCell* debugCells,
							  ulong objId,
							__global  uint* cellIdArray,
							__global  ulong* cellObjArray,
							__global  bool* freeCells,
							int phantomCellIdx
							
		){
		debugCells[phantomCellIdx] = getDebugCell(x , y,z, dConst );
		cellObjArray[phantomCellIdx] = objId| PHANTOMCELL;
		//setCellFull(getDebugCell(actCell.x -1, actCell.y,actCell.z,dConst), freeCells, dConst);
		cellIdArray[phantomCellIdx] = calculateHash(x,y,z,dConst);
	}
#else
	inline void fillCell(	int x, int y, int z,
							CollisionConstants dConst,
							 ulong objId,
							__global  uint* cellIdArray,
							__global  ulong* cellObjArray,
							__global  bool* freeCells,
							int phantomCellIdx

		){
		cellObjArray[phantomCellIdx] = objId | PHANTOMCELL;
		//setCellFull(getDebugCell(actCell.x -1, actCell.y,actCell.z,dConst), freeCells, dConst);
		cellIdArray[phantomCellIdx] = calculateHash(x , y, z,dConst);
	}
#endif


__kernel void CreateCellId(	__global  ulong* objectIds, 
	__global  clShapeObject* shapes,
	__global  uint* cellIdArray,
	__global  ulong* cellObjArray,
	__global  bool* freeCells,
	CollisionConstants dConst,
	uint numElements 
	#ifdef DEBUG
	, __global  CollisionCell* debugCells
	#endif
	)
{
	__private uint globalId = get_global_id(0);
	__private uint gLocalId = get_local_id(0);
	__private uint gGroupId = get_group_id(0);
	__private uint elementsPerBlock = (numElements / dConst.numBlocks) +1 ;

	__private uint end = (gGroupId) * elementsPerBlock+elementsPerBlock;
	if(end > numElements) end = numElements;
	
	__private uint beginPhantomCells ;
	__private uint phantomCellCount = 0;

	

	// Jeder Thread bearbeitet ein objekt alleine
	for(int i = gLocalId + gGroupId * elementsPerBlock; i < end; i+= dConst.numThreadsPerBlock){
		// Caluculate home hash
		__private CollisionCell actCell = getCell(shapes[i].center, dConst.cellSizeX, dConst.cellSizeY, dConst.cellSizeZ);
		cellIdArray[i] = calculateCellHash(actCell, dConst);
		cellObjArray[i] = objectIds[i];
		cellObjArray[i] |= HOMECELL;

		#ifdef DEBUG
		debugCells[i] = actCell;
		#endif
		setCellFull(actCell,freeCells,dConst);
		private int phantomCellIdx = dConst.numTotalElements + i * NUM_PHANTOMCELLS;
		phantomCellCount = 0;


		// Check Collison with neighbour cells

		// first go for the X-Axis
		if(((int) (shapes[i].leftBottomFront.x / dConst.cellSizeX)) != actCell.x){
			// Überlappung bei x < Xmin
			#ifdef DEBUG
			fillCell(actCell.x -1, actCell.y ,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
			#else
			fillCell(actCell.x -1, actCell.y ,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
			#endif
			phantomCellCount++;

			// hinzufügen von x-1 als phantomcell


			// Check < ymin
			if(((int) (shapes[i].leftBottomFront.y / dConst.cellSizeY)) != actCell.y){
				// x < xmin & y < ymin

				// hinzufügen von x-1, y-1 als phantomcell
				// hinzufügen von y-1 als phantomcell
				
				#ifdef DEBUG
				fillCell(actCell.x -1, actCell.y -1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x -1, actCell.y -1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;
				
				#ifdef DEBUG
				fillCell(actCell.x, actCell.y -1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x, actCell.y -1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;


				//  check < zmin
				if(((int) (shapes[i].leftBottomFront.z / dConst.cellSizeZ)) != actCell.z){
					// x < xmin & y < ymin & z < zmin

					// hinzufügen von x-1, y-1, z-1 als phantomcell
					// hinzufügen von x-1, z-1 als phantomcell
					// hinzufügen von y-1, z-1 als phantomcell
					// hinzufügen von z-1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y -1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y -1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y -1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y -1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;

					// check < zmax
					}else if(((int) (shapes[i].rightTopRear.z / dConst.cellSizeZ)) != actCell.z){
					// x < xmin & y < ymin & z > zmax

					// hinzufügen von x-1, y-1, z+1 als phantomcell
					// hinzufügen von x-1, z+1 als phantomcell
					// hinzufügen von y-1, z+1 als phantomcell
					// hinzufügen von z+1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y -1,actCell.z + 1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y -1,actCell.z + 1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y -1,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y -1,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;


					// end
				}

				}else if(((int) (shapes[i].rightTopRear.y / dConst.cellSizeY)) != actCell.y){
				// x < xmin & y > ymax

				// hinzufügen von x-1, y+1 als phantomcell
				// hinzufügen von y+1 als phantomcell
				
				#ifdef DEBUG
				fillCell(actCell.x -1, actCell.y +1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x -1, actCell.y +1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;
				
				#ifdef DEBUG
				fillCell(actCell.x, actCell.y + 1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x, actCell.y + 1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;

				//  check < zmin
				if(((int) (shapes[i].leftBottomFront.z / dConst.cellSizeZ)) != actCell.z){
					// x < xmin & y > ymax & z < zmin

					// hinzufügen von x-1, y+1, z-1 als phantomcell
					// hinzufügen von x-1, z-1 als phantomcell
					// hinzufügen von y+1, z-1 als phantomcell
					// hinzufügen von z-1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y +1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y +1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y +1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y +1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;

					// check < zmax
					}else if(((int) (shapes[i].rightTopRear.z / dConst.cellSizeZ)) != actCell.z){
					// x < xmin & y < ymin & z > zmax

					// hinzufügen von x-1, y+1, z+1 als phantomcell
					// hinzufügen von x-1, z+1 als phantomcell
					// hinzufügen von y+1, z+1 als phantomcell
					// hinzufügen von z+1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y +1,actCell.z + 1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y +1,actCell.z + 1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x -1, actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x -1, actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y +1,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y +1,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;

					// end
				}

			}

			}else if(((int) (shapes[i].rightTopRear.x / dConst.cellSizeX)) != actCell.x){
			// x > xmax 

			// hinzufügen von x+1 als phantomcell
			
			#ifdef DEBUG
			fillCell(actCell.x +1, actCell.y,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
			#else
			fillCell(actCell.x +1, actCell.y,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
			#endif
			phantomCellCount++;

			// Check < ymin
			if(((int) (shapes[i].leftBottomFront.y / dConst.cellSizeY)) != actCell.y){
				// x > xmax  & y < ymin

				// hinzufügen von x+1, y-1 als phantomcell
				// hinzufügen von y-1 als phantomcell
				
				#ifdef DEBUG
				fillCell(actCell.x +1, actCell.y -1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x +1, actCell.y -1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;
				
				#ifdef DEBUG
				fillCell(actCell.x, actCell.y -1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x, actCell.y -1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;


				//  check < zmin
				if(((int) (shapes[i].leftBottomFront.z / dConst.cellSizeZ)) != actCell.z){
					// x > xmax  & y < ymin & z < zmin

					// hinzufügen von x+1, y-1, z-1 als phantomcell
					// hinzufügen von x+1, z-1 als phantomcell
					// hinzufügen von y-1, z-1 als phantomcell
					// hinzufügen von z-1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y -1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y -1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y -1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y -1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;


					// check < zmax
					}else if(((int) (shapes[i].rightTopRear.z / dConst.cellSizeZ)) != actCell.z){
					// x > xmax  & y < ymin & z > zmax

					// hinzufügen von x+1, y-1, z+1 als phantomcell
					// hinzufügen von x+1, z+1 als phantomcell
					// hinzufügen von y-1, z+1 als phantomcell
					// hinzufügen von z+1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y -1,actCell.z + 1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y -1,actCell.z + 1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y -1,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y -1,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;


					// end
				}

				}else if(((int) (shapes[i].rightTopRear.y / dConst.cellSizeY)) != actCell.y){
				// x > xmax  & y > ymax

				// hinzufügen von x+1, y+1 als phantomcell
				// hinzufügen von y+1 als phantomcell
				
				#ifdef DEBUG
				fillCell(actCell.x +1, actCell.y +1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x +1, actCell.y +1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;
				
				#ifdef DEBUG
				fillCell(actCell.x, actCell.y + 1,actCell.z,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#else
				fillCell(actCell.x, actCell.y + 1,actCell.z,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
				#endif
				phantomCellCount++;

				//  check < zmin
				if(((int) (shapes[i].leftBottomFront.z / dConst.cellSizeZ)) != actCell.z){
					// x > xmax  & y > ymax & z < zmin

					// hinzufügen von x+1, y+1, z-1 als phantomcell
					// hinzufügen von x+1, z-1 als phantomcell
					// hinzufügen von y+1, z-1 als phantomcell
					// hinzufügen von z-1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y +1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y +1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y +1,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y +1,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z -1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;


					// check < zmax
					}else if(((int) (shapes[i].rightTopRear.z / dConst.cellSizeZ)) != actCell.z){
					// x > xmax  & y < ymin & z > zmax

					// hinzufügen von x+1, y+1, z+1 als phantomcell
					// hinzufügen von x+1, z+1 als phantomcell
					// hinzufügen von y+1, z+1 als phantomcell
					// hinzufügen von z+1 als phantomcell
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y +1,actCell.z + 1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y +1,actCell.z + 1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x +1, actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x +1, actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x, actCell.y +1,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x, actCell.y +1,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;
					
					#ifdef DEBUG
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst,debugCells, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#else
					fillCell(actCell.x , actCell.y ,actCell.z +1,dConst, objectIds[i], cellIdArray, cellObjArray,freeCells, phantomCellIdx++);
					#endif
					phantomCellCount++;


					// end
				}

			}

			
		}
					// Alle existierenden Phantomcells wurde hinzugefügt 
			// Auffüllen mit dummywerten bis es jeweils 7 Phantomcells gibt
			while(phantomCellCount < 7){

				cellObjArray[phantomCellIdx] = objectIds[i] | UNUSED;

				cellIdArray[phantomCellIdx++] = 0xFFFFFFFF;
				phantomCellCount++;
			}
		}
	}


__kernel void CreateCollisionList(__global  uint* cellIdArray, __global  ulong* cellObjArray, __global  ulong* collisionList,
	                              __global volatile uint* collisionListIdx, CollisionConstants dConst, uint numElements){
	__private uint gGroupId = get_group_id(0);
	__private uint globalId = get_global_id(0);
	__private uint gLocalId = get_local_id(0);
	__private uint elementsPerBlock = numElements / dConst.numBlocks +1 ;
	// Calculate the number of Elements each thread has to deal with -> min 1
	__private uint numElementsPerThread = (elementsPerBlock < dConst.numThreadsPerBlock)? 1 : elementsPerBlock / dConst.numThreadsPerBlock +1;
	// Its possible that there are more threads than elements so we need to skip these threads 
	__private bool goForIt = elementsPerBlock > gLocalId * numElementsPerThread; 
	if(!goForIt)
		return;
	__private int idx = elementsPerBlock*gGroupId +gLocalId*numElementsPerThread ;
	// Previous thread run over the boarder if the series does not end -> increase idx accordingly
	if(idx > 0){
		while(idx < numElements && cellIdArray[idx-1] == cellIdArray[idx])
		idx++;
	}
	__private ulong startIdx;
	__private ulong objCount;
	__private ulong hCount=0;
	while( idx+1 < numElements && idx < (elementsPerBlock*gGroupId +(gLocalId+1)*numElementsPerThread)){
		if(cellIdArray[idx] == 0xFFFFFFFF)
			break;
		if(cellIdArray[idx] == cellIdArray[idx+1]){
			startIdx = idx;
			objCount = 1;
			hCount = 0;
			if(cellObjArray[idx] & HOMECELL)
				hCount++;
			while(cellIdArray[idx] == cellIdArray[idx+1] && idx+1 < numElements){
				objCount++;
				if(cellObjArray[idx+1] & HOMECELL)
					hCount++;
				idx++;
			}
			collisionList[atomic_inc(collisionListIdx)] = startIdx | (objCount<<32) | (hCount<<48);
		}
		idx++;
	}
}


__kernel void CreateCollsionTuples(	 
	__global  ulong* cellObjArray,
	__global  ulong* collisionList,
	__global  CollisionTupel* collisionTuples,
	__global volatile uint* tupleIdx,
	CollisionConstants dConst,
	uint numElements
	)
{
	__private uint globalId = get_global_id(0);
	__private uint gLocalId = get_local_id(0);
	__private uint gGroupId = get_group_id(0);

	__private uint elementsPerBlock = (numElements / dConst.numBlocks) +1 ;
	
	__private uint boundary = elementsPerBlock +  elementsPerBlock * (gGroupId) ;
	
	boundary = (boundary < numElements)? boundary : numElements;

	__private uint actIdx;
	for(int i = gLocalId + elementsPerBlock * gGroupId; i< boundary; i+= dConst.numThreadsPerBlock ){

		__private uint objCnt = (uint) ((long)(collisionList[i]>>32) & 0xFFFF);
		__private uint homeCnt = (uint) ((long)(collisionList[i]>>48) & 0xFFFF);
		__private uint begin = (uint) ((long)collisionList[i] & 0xFFFFFFFF);
		if(begin + homeCnt > dConst.numTotalElements*8 || begin + objCnt > dConst.numTotalElements *8 )
			continue;
		// home collisions hinzufügen
		for(int j = begin;  j < begin + homeCnt; j++){
			for(int k = j+1; k < begin + objCnt; k++){
				actIdx =atomic_inc(tupleIdx);
				collisionTuples[actIdx].obj1 = j;
				collisionTuples[actIdx].obj2 = k;
			}
		}

		/*	
		//phantomcollisions hinzufügen
		for(int j = begin+ homeCnt;  j < begin + objCnt; j++){
			for(int k = j+1; k < begin + objCnt; k++){
				actIdx =atomic_inc(tupleIdx);
				collisionTuples[actIdx].obj1 = j;
				collisionTuples[actIdx].obj2 = k;			
			}
		}	
		*/	
		// TODO: Phantom Phantom collision


	}
}


__kernel void CheckCollsions(	 
	__global  ulong* cellObjArray,
	__global  CollisionTupel* collisionTuples,
	__global  ShapeTupel* collidingObjs,
	__global  CollisionTupel* collisons,
	__global volatile uint* collidingObjIdx,
	CollisionConstants dConst,
	uint numElements)
{

	__private uint gGroupId = get_group_id(0);
	__private uint globalId = get_global_id(0);
	__private uint gLocalId = get_local_id(0);

	__private uint elementsPerBlock = (numElements / dConst.numBlocks)+1 ;
	__private int end = elementsPerBlock* (gGroupId + 1) ;
	end = (end>numElements)? numElements: end;
	
	// TODO -> filter explore elements


	for(int idx = elementsPerBlock*gGroupId +gLocalId ; idx < end; idx+=dConst.numThreadsPerBlock){


		if(		collidingObjs[idx].obj1.leftBottomFront.x  <= collidingObjs[idx].obj2.rightTopRear.x 

			&& 	collidingObjs[idx].obj2.leftBottomFront.x  <= collidingObjs[idx].obj1.rightTopRear.x 

			&& 	collidingObjs[idx].obj1.leftBottomFront.y  <= collidingObjs[idx].obj2.rightTopRear.y 

			&& 	collidingObjs[idx].obj2.leftBottomFront.y  <= collidingObjs[idx].obj1.rightTopRear.y 

			&& 	collidingObjs[idx].obj1.leftBottomFront.z  <= collidingObjs[idx].obj2.rightTopRear.z 

			&& 	collidingObjs[idx].obj2.leftBottomFront.z  <= collidingObjs[idx].obj1.rightTopRear.z 

		){
			// no collision
			__private int actIdx =atomic_inc(collidingObjIdx);
			collisons[actIdx].obj1 = cellObjArray[collisionTuples[idx].obj1] & 0xFFFFFFFF;
			collisons[actIdx].obj2 = cellObjArray[collisionTuples[idx].obj2] & 0xFFFFFFFF;		
			//collisons[atomic_inc(collidingObjIdx)] = collisionTuples[idx];
		}else{
		//colliding
			
		}
	}
}
";

        public static string CollisionDetection2DHierarchicalKernel = @"
#define NUM_PHANTOMCELLS 3
#define NUM_OBJ_CELLS 4
#define HOMECELL (((long)1)<<32)
#define PHANTOMCELL (((long)1)<<33)
#define UNUSED (((long)1)<<34)
#define NATIVE (((long)1)<<38)
#define NEW (((long)1)<<35)
#define MOVE (((long)1)<<36)

// Other do not collide with it, but itself will collide with everything thats in the area
// So it return every Element in the given Shape, but not other elements of this type
#define EXPLORE (((long)1)<<37)
#define GHOST (((long)1)<<39)


//#define DEBUG


typedef struct CollisionConstants
{
	uint numThreadsPerBlock;
	uint numTotalElements;
	uint numLvls;
	uint cellArraySize;
	float xMax;
	float yMax;
	float maxCellSizeX;
	float maxCellSizeY;
	uint xGridBoundary;
	uint yGridBoundary;
	uint numBlocks;
	}CollisionConstants;


typedef struct CollisionSublist
{
	uint startIdx;
	ushort nHome;
	ushort nPhant;
	ushort iHome;
	ushort objCnt;
}CollisionSublist;

typedef struct ObjectData
{
int objectId;
char cellType;

}ObjectData;


typedef struct clPoint2D
{
float x;
float y;
}clPoint2D;

typedef struct ObjectTupel
{
int obj1Idx;
int obj2Idx;
}ObjectTupel;

typedef struct clCircleShapeObject
{
clPoint2D center;
float radius;
}clCircleShapeObject;

// TODO: Maybe add lvl
typedef struct clShapeIdTupel
{
	ulong objId;
	clCircleShapeObject objShape;
}clShapeIdTupel;



typedef struct ShapeTupel
{
clCircleShapeObject obj1;
clCircleShapeObject obj2;
}ShapeTupel;

typedef struct CollisionCell
{
	int x;
	int y;
	}CollisionCell;

inline int calculateCellHash(CollisionCell cell, CollisionConstants dConst){

	return (int) (cell.y*dConst.xMax + cell.x);
}

inline int calculateHash(int x , int y , int xMax, int yMax){

	return (int) ( y*xMax + x);
}

// Input ist die Absolute Position des Elements
// Output ist CollisionCell in dem 
inline CollisionCell getCell(float x, float y, CollisionConstants gConst, int currLvl){
	float cellSize = gConst.maxCellSizeX/(pow((float)2,(float)currLvl));
	__private CollisionCell cell;
	cell.x = (int) (x /cellSize);
	cell.y = (int) (y /cellSize);
	return cell;
}

inline CollisionCell getDebugCell(float x,float y,CollisionConstants gConst){
	__private CollisionCell cell;
	cell.x = (int) x;
	cell.y = (int) y;
	return cell;
}

/*
inline void setCellFull(CollisionCell cell, __global bool* freeCells, CollisionConstants gConst ){
	freeCells[(int)(cell.z*gConst.yGridBoundary*gConst.xGridBoundary + cell.y*gConst.xGridBoundary + cell.x)] = false;
}*/
inline bool Equals(CollisionCell cell1, CollisionCell cell2){

	return cell1.x == cell2.x && cell1.y == cell2.y ;
}



#ifdef DEBUG
//actEle.objShape.center.x -1 ,actEle.objShape.center.y, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  cellIdx++
	inline void fillCell(	int cellX,
							int cellY,
							CollisionConstants dConst,
							  ulong objId,
							__global  uint* cellIdArray,
							__global  ulong* cellObjArray,
							__global  CollisionCell* debugCells,
							int cellIdx
		){
		objId = objId & (~HOMECELL);
		objId |= PHANTOMCELL;
		debugCells[cellIdx] = getDebugCell(cellX , cellY, dConst);
		// objId must already have the right flags added
		cellObjArray[cellIdx] = objId;
		//setCellFull(getDebugCell(actCell.x -1, actCell.y,actCell.z,dConst), freeCells, dConst);
		cellIdArray[cellIdx] = calculateHash(cellX,cellY,dConst.xMax,dConst.yMax);
	}
#else
	inline void fillCell(	int cellX,
							int cellY,
							CollisionConstants dConst,
							  ulong objId,
							__global  uint* cellIdArray,
							__global  ulong* cellObjArray,
							__global  CollisionCell* debugCells,
							int cellIdx
		){
		objId = objId & (~HOMECELL);
		objId |= PHANTOMCELL;
		cellObjArray[cellIdx] = objId;
		//setCellFull(getDebugCell(actCell.x -1, actCell.y,actCell.z,dConst), freeCells, dConst);
		cellIdArray[cellIdx] = calculateHash(cellX , cellY,dConst.xMax,dConst.yMax);
	}
#endif



__kernel void ReorderElements(	
	__global  clShapeIdTupel* input, 
	__global  clShapeIdTupel* lvls, 
	__global volatile int* lvlIdxs,
	CollisionConstants dConst
){

	__private uint gLocalId = get_local_id(0);
	__private uint gGroupId = get_group_id(0);
	__private uint elementsPerBlock = (dConst.numTotalElements / dConst.numBlocks) +1 ;

	__private uint end = (gGroupId) * elementsPerBlock+elementsPerBlock;
	if(end > dConst.numTotalElements) end = dConst.numTotalElements;
	
	// Each thread computes one element
	for(int i = gLocalId + gGroupId * elementsPerBlock; i < end; i+= dConst.numThreadsPerBlock){
		bool found = false;
		for(int j = 0 ; j < dConst.numLvls ; j++){
			if((float)input[i].objShape.radius * 2 > (float) (((float)dConst.maxCellSizeX)/ (pow((float)2,(float)j+1)) )){
				lvls[j*dConst.numTotalElements + atomic_inc(&lvlIdxs[j])] = input[i];
				found = true;
				break;
			}
		}
		if(!found){
			lvls[(dConst.numLvls -1) * dConst.numTotalElements  + atomic_inc(&lvlIdxs[dConst.numLvls -1])] = input[i];

		}
		
	}
}


/*
	OKAY unfortunatly its much easyer to split the elements into different buffers at this point.. So ill reimplement it....
 * InputLevels -> 2D Array with the native elements for each lvl
 * lvlElements -> Total number of elements for each lvl -> Each upper lvl also contains all elements from the lower lvls
 * lvlCellIdArray -> 2D  Array with the Cell ids -> ORDER: 
 - Native Homecells
 - Native Phantomcells
 - Imported Homecells 
 	-> Homecells sublvl 1
 	-> Homecells sublvl 2
 	-> -------
 	-> Phantomcells sublvl 1
 	-> Phantomcells sublvl 2
 	-> -------
 	
 - Imported Phantomcells
 - Debugcells will only be filled in debug mode, elsewise its a null ptr -> Debug mode uses an other fillcell method
 ** TODO: Needed checks -> native homecells vs all other,  native Phantom vs imported homecells
*/
__kernel void CreateCellId(	
	__global  clShapeIdTupel* inputLvls, 
	__global volatile int* lvlElements,
	//__global  int* lvlTotalElements,	// = lvl elements + lvl elements of every lower lvl
	__global  uint* lvlCellIdArray,
	__global  ulong* lvlCellObjArray,
	CollisionConstants dConst,
	uint activeLvl, // Index of the kernel lvl
	uint numElements, 
	__global  CollisionCell* lvlDebugCells

	)
{
		__private uint gLocalId = get_local_id(0);
	__private uint gGroupId = get_group_id(0);
	uint startIdx= numElements * activeLvl; // INPUT LVL START
//	for(int i = 0; i < activeLvl; i++){
//		startIdx += lvlElements[i];
//	}


	__private uint elementsPerBlock = (numElements / dConst.numBlocks) +1 ;
	__private uint end = (gGroupId) * elementsPerBlock+elementsPerBlock;
	if(end >  startIdx + numElements) end = startIdx + numElements;

	__private uint beginPhantomCells ;
	__private uint phantomCellCount = 0;


	// Start at native lvl and then go down to the lower lvls
	for(int i = gLocalId + gGroupId * elementsPerBlock; i < end; i+= dConst.numThreadsPerBlock){

		int idx = i;
		// Now insert the objects into the native and every upper layer.
		int lvlOffset;
		int nativeLvl;
		// check in which lvl our element is
	//	currLvl
		

		for(nativeLvl = activeLvl ; nativeLvl < dConst.numLvls ; nativeLvl++){
			// if the lvl amout of elements in the current lvl is bigger than our remaining idx for that nativeLvl, we got an element inside that lvl
			if(idx - (lvlElements[nativeLvl] -1) <= 0)
				break;
			//lvlOffset += dConst.numTotalElements; 
			idx -=  lvlElements[nativeLvl];
			// if the current idx smaller than the actual lvl offset its inside that lvl
		}




/*
		for(nativeLvl = 0 ; nativeLvl < dConst.numLvls ; nativeLvl++){
			// if the lvl amout of elements in the current lvl is bigger than our remaining idx for that nativeLvl, we got an element inside that lvl
			if(idx - lvlElements[nativeLvl] < 0)
				break;
			lvlOffset += lvlElements[nativeLvl]; 
			idx -=  lvlElements[nativeLvl];
			// if the current idx smaller than the actual lvl offset its inside that lvl
		}
*/
		// Get the current tupel
		clShapeIdTupel actEle = inputLvls[nativeLvl * dConst.numTotalElements + idx];
		
		// Create the home and phantomcells for the upper layers (non native)
//		for(int currLvl = nativeLvl ; currLvl >= 0; currLvl--){
		int currLvl = activeLvl;
		int lvlStart = 0;
		int phantomCellIdx = 0;
		// Get the offset for the current lvl -> TODO: Maybe do this on CPU side
		//for(int tmpLvl = 0 ; tmpLvl < currLvl ; tmpLvl++)
		//	lvlStart += lvlTotalElements[tmpLvl];
		//lvlStart *= NUM_OBJ_CELLS;


		CollisionCell actCell = getCell(actEle.objShape.center.x,actEle.objShape.center.y, dConst, currLvl);

		// home and phandom idx differ if the zell isnt native
		/*
		The order for each lvl is 
		Native home  -> Phantom 
		
		*/
		if(currLvl == nativeLvl) {
			// Its the native lvl for the current element
			actEle.objId |= NATIVE;
			lvlCellIdArray[lvlStart + idx] = calculateCellHash(actCell, dConst);
			lvlCellObjArray[lvlStart + idx] = actEle.objId |  HOMECELL;
			#ifdef DEBUG
				lvlDebugCells[lvlStart + idx] = actCell;
			#endif
			phantomCellIdx = lvlStart + lvlElements[currLvl] + idx * NUM_PHANTOMCELLS;
		}else{
			// The current element isnt native
			// So the native home and phantomcells are before us,
			// after native cells, the homecells sublvls follow beginning with the 
			// nearest lvl
			// N-Home | N-Phant | Sub1-Home | Sub2-Home... | Sub1-Phant | Sub2-Phant
			// HomeOffset is numNativeElements * NUM_OBJ_CELLS + foreach(sublvl before the own native) numElements + actIdx
			// PhantomOffset is numNativeElements * NUM_OBJ_CELLS + foreach(sublvl) + numElements + foreach(sublvl before the own native) numElements * NUM_PHANTOMCELLS + actIdx* NUM_PHANTOMCELLS

			int tmpIdx = lvlStart;
			// Inrement for all native cells
			tmpIdx += lvlElements[currLvl] * NUM_OBJ_CELLS;
			phantomCellIdx = tmpIdx;
			// Increment for the home elements of every lvl closer to the current than out own
			for(int tmpLvl = currLvl+1 ; tmpLvl < nativeLvl ; tmpLvl++ ){
				tmpIdx += lvlElements[tmpLvl]; 
			}
			tmpIdx += idx;
			lvlCellIdArray[tmpIdx] = calculateCellHash(actCell,dConst);
			lvlCellObjArray[tmpIdx] = actEle.objId |= HOMECELL;
			#ifdef DEBUG
				lvlDebugCells[tmpIdx] = actCell;
			#endif
			
			for(int tmpLvl = currLvl+1 ; tmpLvl < dConst.numLvls ; tmpLvl++ ){
				phantomCellIdx += lvlElements[tmpLvl]; 
			}
			for(int tmpLvl = currLvl+1 ; tmpLvl < nativeLvl ; tmpLvl++ ){
				phantomCellIdx += lvlElements[tmpLvl] * NUM_PHANTOMCELLS; 
			}
			phantomCellIdx += idx * NUM_PHANTOMCELLS;
		}
		


		phantomCellCount = 0;
		// Get lvl cellsize
		float actCellSize = dConst.maxCellSizeX / (int)(pow((float)2,(float)currLvl));
		// set this cell as home
		if((int)((actEle.objShape.center.x -actEle.objShape.radius)/ actCellSize ) != actCell.x ){
			// add x -1,y 
			fillCell(actCell.x -1 ,actCell.y, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
			phantomCellCount++;
			if((int)((actEle.objShape.center.y -actEle.objShape.radius)/ actCellSize ) != actCell.y ){
				// add x-1,y-1 and x,y-1
				fillCell(actCell.x -1 ,actCell.y-1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				fillCell(actCell.x ,actCell.y-1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				phantomCellCount+=2;

			}else if((int)((actEle.objShape.center.y +actEle.objShape.radius)/ actCellSize ) != actCell.y ){
				// add x-1,y+1 and x,y+1
				fillCell(actCell.x -1 ,actCell.y+1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				fillCell(actCell.x ,actCell.y+1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				phantomCellCount+=2;
			}
		}else if((int)((actEle.objShape.center.x +actEle.objShape.radius)/ actCellSize ) != actCell.x ){
			// add x+1,y 
			fillCell(actCell.x +1 ,actCell.y, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
			phantomCellCount++;
			if((int)((actEle.objShape.center.y -actEle.objShape.radius)/ actCellSize ) != actCell.y ){
				// add x+1,y-1 and x,y-1
				fillCell(actCell.x +1 ,actCell.y-1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				fillCell(actCell.x ,actCell.y-1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				phantomCellCount+=2;

			}else if((int)((actEle.objShape.center.y +actEle.objShape.radius)/ actCellSize ) != actCell.y ){
				// add x+1,y+1 and x,y+1

				fillCell(actCell.x +1 ,actCell.y+1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				fillCell(actCell.x ,actCell.y+1, dConst ,actEle.objId, lvlCellIdArray, lvlCellObjArray, lvlDebugCells,  phantomCellIdx++);
				phantomCellCount+=2;
			}
		}
		// fill up with dummycells until NUM_PHANTOMCELLS
		while(phantomCellCount < NUM_PHANTOMCELLS){
		 	actEle.objId  =  actEle.objId  & (~HOMECELL);

			lvlCellObjArray[phantomCellIdx] = actEle.objId | UNUSED;

			lvlCellIdArray[phantomCellIdx++] = 0xFFFFFFFF;
			phantomCellCount++;
		}
		
	}
}



// In the first version, we start one kernel per lvlArray
// In this case the algorithm will be the same as before.
// Maybe start one kernel for all lvls for optimization
// Also we should detect to big Subarrays here and start an separate kernel for each
// Since building permutations for all elements can take quite some time on big counts
__kernel void CreateNarrowCheckList(__global  uint* cellIdArray,
								 __global  ulong* cellObjArray, 
								 __global  CollisionSublist* narrowCheckSublist,
	                             __global volatile uint* narrowCheckSublistIdx, 
	                             CollisionConstants dConst, 
	                             uint numElements){
	__private uint gGroupId = get_group_id(0);
	__private uint gLocalId = get_local_id(0);
	__private uint elementsPerBlock = numElements / dConst.numBlocks +1 ;
	// Calculate the number of Elements each thread has to deal with -> min 1
	__private uint numElementsPerThread = (elementsPerBlock < dConst.numThreadsPerBlock)? 1 : elementsPerBlock / dConst.numThreadsPerBlock +1;
	// Its possible that there are more threads than elements so we need to skip these threads 
	__private bool goForIt = elementsPerBlock > gLocalId * numElementsPerThread; 
	if(!goForIt)
		return;
	__private int idx = elementsPerBlock*gGroupId +gLocalId*numElementsPerThread ;
	// Previous thread run over the boarder if the series does not end -> increase idx accordingly
	if(idx > 0){
		while(idx < numElements && cellIdArray[idx-1] == cellIdArray[idx])
		idx++;
	}
	__private ulong startIdx;
	__private ulong objCount;
	__private ulong nCount=0;
	while( idx+1 < numElements && idx < (elementsPerBlock*gGroupId +(gLocalId+1)*numElementsPerThread)){
		if(cellIdArray[idx] == 0xFFFFFFFF)
			break;
		if(!(cellObjArray[idx] & NATIVE)){
			idx++;
			continue;
		} 
		if(cellIdArray[idx] == cellIdArray[idx+1]  ){
			uint startIdx = idx;
			ushort objCnt = 1;
			ushort nHome = 0;
			ushort nPhant = 0;
			ushort iHome = 0;

			// Native homecell -> 
			// values:
			// - startIdx
			// - Native home cnt
			// - Total number of elements im the same cell
			// - Amount of native cells......
			if(cellObjArray[idx] & HOMECELL)
				nHome++;
			else
				nPhant++;
			

			while(cellIdArray[idx] == cellIdArray[idx+1] && idx+1 < numElements){
				objCnt++;
				if(cellObjArray[idx+1] & NATIVE){
					// Native cell
					// Native homecell -> 
					// values:
					// - startIdx
					// - Native home cnt
					// - Total number of elements im the same cell
					// - Amount of native cells......
					if(cellObjArray[idx+1] & HOMECELL)
						nHome++;
					else
						nPhant++;
				}else{
					// Imported cell
					if(cellObjArray[idx+1] & HOMECELL)
						iHome++;

				} 
				idx++;
			}

			if(nHome == 0 && iHome == 0){
              idx++;                            
              continue;
            }
			CollisionSublist tmpLst = {startIdx,nHome,nPhant,iHome,objCnt};
			// TODO : check objcount -> maybe shard the cartesian product calculation
			narrowCheckSublist[atomic_inc(narrowCheckSublistIdx)] = tmpLst;
		}
		idx++;
	}
}


__kernel void CreateCollsionTuples(	 
	__global  ulong* cellObjArray,
	__global  CollisionSublist* narrowCheckSublist,
	__global  ObjectTupel* collisionCheckTuples,
	__global volatile uint* tupleIdx,
	CollisionConstants dConst,
	uint numElements
	)
{
	__private uint globalId = get_global_id(0);
	__private uint gLocalId = get_local_id(0);
	__private uint gGroupId = get_group_id(0);

	__private uint elementsPerBlock = (numElements / dConst.numBlocks) +1 ;
	
	__private uint boundary = elementsPerBlock +  elementsPerBlock * (gGroupId) ;
	
	boundary = (boundary < numElements)? boundary : numElements;

	__private uint actIdx;
	for(int i = gLocalId + elementsPerBlock * gGroupId; i< boundary; i+= dConst.numThreadsPerBlock ){
		// TODO: For safety reasons we may need to check boundarys here. 
		// If this method gets valid data, an overflow is not possible, but there could be memory errors


		CollisionSublist colLst= narrowCheckSublist[i];
		// Order in cellIdArray :
		//    | nHome | nPhant | iHome | iPhant |

		int nHomeEnd = colLst.startIdx + colLst.nHome;
		int nPhantEnd = colLst.startIdx + colLst.nHome + colLst.nPhant;
		int iHomeEnd = colLst.startIdx + colLst.nHome + colLst.nPhant + colLst.iHome;
	//	if(begin + homeCnt > dConst.numTotalElements*8 || begin + objCnt > dConst.numTotalElements *8 )
	//		continue;
		// home collisions hinzufügen
		for(int j = colLst.startIdx ;  j < nHomeEnd; j++){
			for(int k = j+1; k < colLst.startIdx + colLst.objCnt; k++){
				actIdx =atomic_inc(tupleIdx);
				collisionCheckTuples[actIdx].obj1Idx = j;
				collisionCheckTuples[actIdx].obj2Idx = k;
			}
		}

		// Add Phantom - Import collisions
		for(int j = nHomeEnd ;  j < nPhantEnd; j++){
			for(int k = nPhantEnd ; k < iHomeEnd; k++){
				actIdx =atomic_inc(tupleIdx);
				collisionCheckTuples[actIdx].obj1Idx = j;
				collisionCheckTuples[actIdx].obj2Idx = k;
			}
		}
	}
}


// Since we broke everything up to collisiontupels the last step is almost the same as it was for the 3D variant 
// Well its 2D circles instead of 3D Boxes, but the base check is the same
__kernel void CheckCollsions(	 
	__global  ulong* cellObjArray,
	__global  ObjectTupel* collisionCheckTuples,
	__global  ShapeTupel* collidingObjs,
	__global  ObjectTupel* collisons,
	__global  ObjectTupel* explores,
	__global volatile uint* collidingObjIdx,
	__global volatile uint* exploreObjIdx,
	CollisionConstants dConst,
	uint numElements)
{

	__private uint gGroupId = get_group_id(0);
	__private uint globalId = get_global_id(0);
	__private uint gLocalId = get_local_id(0);

	__private uint elementsPerBlock = (numElements / dConst.numBlocks)+1 ;
	__private int end = elementsPerBlock* (gGroupId + 1) ;
	end = (end>numElements)? numElements: end;
	



	for(int idx = elementsPerBlock*gGroupId +gLocalId ; idx < end; idx+=dConst.numThreadsPerBlock){
		// TODO	 -> filter explore elements
		if ( cellObjArray[collisionCheckTuples[idx].obj1Idx] & EXPLORE){
			if(cellObjArray[collisionCheckTuples[idx].obj2Idx] & EXPLORE)
				continue;	
		} 
		if( (cellObjArray[collisionCheckTuples[idx].obj1Idx] & GHOST) || (cellObjArray[collisionCheckTuples[idx].obj2Idx] & GHOST)){
			if(!((cellObjArray[collisionCheckTuples[idx].obj1Idx] & EXPLORE) || (cellObjArray[collisionCheckTuples[idx].obj2Idx] & EXPLORE)))
				continue;
		}
		float dx = fabs((float)(collidingObjs[idx].obj1.center.x -collidingObjs[idx].obj2.center.x));  
		float dy = fabs((float)(collidingObjs[idx].obj1.center.y -collidingObjs[idx].obj2.center.y));  
		float dist = sqrt(pown(dx,2) + pown(dy,2));  
		if(	dist <	 collidingObjs[idx].obj1.radius +collidingObjs[idx].obj2.radius){
		    if ( (cellObjArray[collisionCheckTuples[idx].obj1Idx] & EXPLORE) || (cellObjArray[collisionCheckTuples[idx].obj2Idx] & EXPLORE)){
			    // add to explores
 			    __private int actIdx2 =atomic_inc(collidingObjIdx);

			    explores[actIdx2].obj1Idx = cellObjArray[collisionCheckTuples[idx].obj1Idx] & 0xFFFFFFFF;
			    explores[actIdx2].obj2Idx = cellObjArray[collisionCheckTuples[idx].obj2Idx] & 0xFFFFFFFF;
			    continue;
		    }
		
			__private int actIdx =atomic_inc(collidingObjIdx);
			collisons[actIdx].obj1Idx = cellObjArray[collisionCheckTuples[idx].obj1Idx] & 0xFFFFFFFF;
			collisons[actIdx].obj2Idx = cellObjArray[collisionCheckTuples[idx].obj2Idx] & 0xFFFFFFFF;		
			//collisons[atomic_inc(collidingObjIdx)] = collisionCheckTuples[idx];
		}
	}
}

";
    }
}
