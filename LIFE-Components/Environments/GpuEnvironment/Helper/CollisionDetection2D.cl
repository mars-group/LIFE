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


#define DEBUG


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
