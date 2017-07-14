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

