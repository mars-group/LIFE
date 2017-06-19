using System;
using System.IO;
using GpuEnvironment.Implementation;
using GpuEnvironment.Types;

namespace GpuEnvironment.Helper
{
    public static class DebugHelper {
        public static int Log_Idx = 0;
        private static string debugLog = Path.Combine(GpuESC.LOG_BASE_PATH + "OpenCLDebugLog.txt");
        private static string sortLog = Path.Combine(GpuESC.LOG_BASE_PATH + "sortLog.txt");
        private static string log = Path.Combine(GpuESC.LOG_BASE_PATH + "log.txt");

        private const long HOMECELL = (1L << 32);
        private const long PHANTOMCELL = (1L << 33);
        private const long UNUSED = (1L << 34);


        private const long NEW = (1L << 35);
        private const long MOVE = (1L << 36);
        private const long EXPLORE = (1L << 37);
        private const long NATIVE = (1L << 38);
        private const long GHOST = (1L << 39);


        public static void PrintElementBuffer(int[] printData, int count, string caption)
        {
            String output = caption;
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------";
            for (int i = 0; i < count; i++)
            {
                if (i % 20 == 0) output += "\n";
                output += String.Format("{0,5:x} ", printData[i]);

            }
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(sortLog))
            {
                sw.WriteLine(output);
            }
        }

        public static void PrintElementBuffer(uint[] printData, int count, string caption)
        {
            String output = caption;
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------";
            for (int i = 0; i < count; i++)
            {
                if (i % 20 == 0) output += "\n";
                output += String.Format("{0,5:x} ", printData[i]);

            }
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(sortLog))
            {
                sw.WriteLine(output);
            }
        }
        public static void PrintCellIdBuffer(long[] cellInput, int[] cellIds, long[] cellData, string caption, int count)
        {
            String output = caption;
            output += "\n";
            output += "Data input array\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------";
            for (int i = 0; i < count; i++)
            {
                if (i % 20 == 0) output += "\n";
                output += String.Format("{0,5:x} ", cellInput[i]&0xFFFFFFFF);

            }
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";


            output += "Generated Cell Id Array \n";
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            output += String.Format("{0,-12}|{1,-12}|{2,-12}|{3,-12}|{4,-12}|{5,-12}|{6,-12}|{7,-12}\n", " CellId", " CellObjId", " Home", " Phantom", " New", " Moved", "Explore", "Unused");
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            for (int i = 0; i < count*8; i++)
            {
                output += String.Format("{0,-12:x}|{1,-12:x}|{2,-12}|{3,-12}|{4,-12}|{5,-12}|{6,-12}|{7,-12}\n", cellIds[i], (cellData[i] & ((long)0xFFFFFFFF)),
                    ((cellData[i] & HOMECELL) > 0) ? " Yes" : " No", ((cellData[i] & PHANTOMCELL) > 0) ? " Yes" : " No",
                    ((cellData[i] & NEW) > 0) ? " Yes" : " No",((cellData[i] & MOVE)>0)?" Yes":" No",((cellData[i] & EXPLORE)>0)?" Yes":" No",((cellData[i] & UNUSED)>0)?" Yes":" No");

            } 
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }





        }

        public static void PrintCollisionList(ulong[] collisionList,uint[] cellIds, ulong[] cellData, CollisionCell3D[] debugCell3DPosList, string caption, int count)
        {
            String output = caption;
            output += "\n";
            output += "Collision List\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            for (int i = 0; i < count; i++)
            {
                int index = (int)(collisionList[i] & 0xFFFFFFFF);
                int numElements = (int)((collisionList[i] >> 32) & 0xFFFF);
                int homeCnt = (int)((collisionList[i] >> 48) & 0xFFFF);
                output += String.Format("actCellID = {0:x} x={1} y={2} z={3} startIdx= {4} homeCount = {5}\n", cellIds[index], debugCell3DPosList[index].x, debugCell3DPosList[index].y, debugCell3DPosList[index].z,index, homeCnt);
                output += String.Format("Colliding objects:\n");        

                for (int j = 0; j < numElements; j++)
                {
                    output += String.Format("objId: {0},", cellData[index+j]&0xFFFFFFFF);        
                }
                output += "\n\n";
            }

            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }

        }

        public static void PrintCollisionTuples(CollisionTupel[] collisionTupels, uint[] cellIds, ulong[] cellData, CollisionCell3D[] debugCell3DPosList, string caption, int count)
        {
            String output = caption;
            output += "\n";
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            for (int i = 0; i < count; i++)
            {
                uint index1 = collisionTupels[i].obj1;
               uint index2 = collisionTupels[i].obj2;
                output += String.Format("CollisionCheck at {0:x}:\n", cellIds[index1]);

                output += String.Format("CollisionTupelElement 1 = {0:x} x={1} y={2} z={3}\n", cellData[index1]&0xFFFFFFFF, debugCell3DPosList[index1].x, debugCell3DPosList[index1].y, debugCell3DPosList[index1].z);
                output += String.Format("CollisionTupelElement 2 = {0:x} x={1} y={2} z={3}\n", cellData[index2]&0xFFFFFFFF, debugCell3DPosList[index2].x, debugCell3DPosList[index2].y, debugCell3DPosList[index2].z);

                output += "\n";
            }

            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }

        }
/*        public static void PrintCounterBuffer(int[] printData, string caption)
        {
            String output = caption;
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            output += String.Format("{0,10}", "");
            for (int j = 0; j < numBlocks; j++)
            {
                output += String.Format("{0,29}", "Block " + j);
            }
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            for (int i = 0; i < num_Radices; i++)
            {
                output += String.Format(" {0,15}    ", "Radix " + i);
                for (int j = 0; j < numBlocks; j++)
                {
                    output += String.Format("{0,5}", "");
                    for (int k = 0; k < NumGroupsPerBlock; k++)
                    {
                        output += String.Format("{0,5:x} ", printData[i * numBlocks * NumGroupsPerBlock + j * NumGroupsPerBlock + k]);

                    }
                }
                output += "\n";
            }
            using (StreamWriter sw = File.AppendText(@"D:\Projects\gpu\GPGPUGpgpuESC\GPGPUGpgpuESC\log.txt"))
            {
                sw.WriteLine(output);
            }
        }*/

        public static void PrintCellIdBufferExtended(ulong[] cellInput,uint[] cellIds, ulong[] cellData,  CollisionCell3D[] debugCell3DPosList,string caption,int count)
        {
            String output = caption;
            output += "\n";
            output += "Data input array\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------";
            for (int i = 0; i < count; i++)
            {
                if (i % 20 == 0) output += "\n";
                output += String.Format("{0,5:x} ", cellInput[i] & 0xFFFFFFFF);

            }
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";


            output += "Generated Cell Id Array \n";
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            output += String.Format("{0,-12}|{1,-12}|{2,-12}|{3,-12}|{4,-12}|{5,-12}|{6,-12}|{7,-12}|{8,-12}|{9,-12}|{10,-12}\n", " CellId", " CellObjId", "CellPos X", "CellPos Y", "CellPos Z", " Home", " Phantom", " New", " Moved", "Explore", "Unused");
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            for (int i = 0; i < count * 8; i++)
            {
                output += String.Format("{0,-12:x}|{1,-12:x}|{2,-12}|{3,-12}|{4,-12}|{5,-12}|{6,-12}|{7,-12}|{8,-12}|{9,-12}|{10,-12}\n", cellIds[i], (cellData[i] & ((long)0xFFFFFFFF)), debugCell3DPosList[i].x, debugCell3DPosList[i].y, debugCell3DPosList[i].z,
                    ((cellData[i] & HOMECELL) > 0) ? " Yes" : " No", ((cellData[i] & PHANTOMCELL) > 0) ? " Yes" : " No",
                    ((cellData[i] & NEW) > 0) ? " Yes" : " No", ((cellData[i] & MOVE) > 0) ? " Yes" : " No", ((cellData[i] & EXPLORE) > 0) ? " Yes" : " No", ((cellData[i] & UNUSED) > 0) ? " Yes" : " No");

            }
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }



        }


        public static void PrintAsArray(ulong[] values) {
            string output = "static ulong[] = new ulong[]{ ";
            for (int i = 0; i < values.Length-1; i++) {
                output += string.Format("0x{0:x} ,",values[i]);
            }
            output += string.Format("0x{0:x} );", values[values.Length-1]);

            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }
        }

        public static void PrintAsArray(uint[] values)
        {
            string output = "static ulong[] = new ulong[]{ ";
            for (int i = 0; i < values.Length - 1; i++)
            {
                output += string.Format("0x{0:x} ,", values[i]);
            }
            output += string.Format("0x{0:x} );", values[values.Length - 1]);

            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }
        }

        public static void PrintLevelShapeElements(HierarchicalGpuESC.clShapeIdTupel[] readLvlShapeElements, uint[] lvlElements, HierarchicalGpuESC.cl2DCollisionConstants constants, string caption) {
            String output = caption;
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            for (uint i = 0; i < constants.numLvls; i++) {
                uint lvlStartIdx = i*constants.numTotalElements;
                output += string.Format("\n level{0} elements:\n",i);
                for (int j = 0; j < lvlElements[i]; j++) {
                    output += string.Format("ID: {0} | x:{1:0.00} ,y:{2:0.00}, radius:{3:0.00}\n", readLvlShapeElements[lvlStartIdx+j].objId & 0xFFFFFFFF, readLvlShapeElements[lvlStartIdx + j].objShape.center.x, readLvlShapeElements[lvlStartIdx + j].objShape.center.y, readLvlShapeElements[lvlStartIdx + j].objShape.radius);
                }
            }
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }
        }


        // 2D Debug print
        public static void PrintCellIdBufferExtended(uint[][] cellIds, ulong[][] cellData, CollisionCell2D[][] debugCell3DPosList, string caption, HierarchicalGpuESC.cl2DCollisionConstants constants, uint[] lvlTotalElements) {
            String output = caption;
            uint startIdx = 0;

            output += "Generated Cell Id Array \n";
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            output += String.Format("{0,-12}|{1,-12}|{2,-12}|{3,-12}|{4,-12}|{5,-12}|{6,-12}|{7,-12}|{8,-12}|{9,-12}|{10,-12}|{11,-12}|{12,-12}\n", " CellId", " CellObjId", "CellPos X", "CellPos Y", "CellPos Z", " Home", " Phantom", " New", " Moved", "Explore", "Unused", "Native", "Ghost");
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            for (int i = 0; i < constants.numLvls; i++) {
                output += String.Format("-------------- Level {0} elements -------------- \n\n\n",i);

                for (uint k = 0; k <  lvlTotalElements[i] * 4; k++) {
                    output += String.Format("{0,-12:x}|{1,-12:x}|{2,-12}|{3,-12}|{4,-12}|{5,-12}|{6,-12}|{7,-12}|{8,-12}|{9,-12}|{10,-12}|{11,-12}|{12,-12}\n", cellIds[i][k], (cellData[i][k] & ((long)0xFFFFFFFF)), debugCell3DPosList[i][k].x, debugCell3DPosList[i][k].y, 0,
                      ((cellData[i][k] & HOMECELL) > 0) ? " Yes" : " No", ((cellData[i][k] & PHANTOMCELL) > 0) ? " Yes" : " No",
                      ((cellData[i][k] & NEW) > 0) ? " Yes" : " No", ((cellData[i][k] & MOVE) > 0) ? " Yes" : " No", ((cellData[i][k] & EXPLORE) > 0) ? " Yes" : " No", ((cellData[i][k] & UNUSED) > 0) ? " Yes" : " No", ((cellData[i][k] & NATIVE) > 0) ? " Yes" : " No", ((cellData[i][k] & GHOST) > 0) ? " Yes" : " No");

                }

            }
   
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }
        }

        public static void PrintCollisionList(HierarchicalGpuESC.clCollisionSublist[][] sublists, uint[][] readCellIds, ulong[][] cellData, CollisionCell2D[][] debugCell3DPosList, string caption, int[][] sharedIdx, uint numLvls) {
            String output = caption;
            output += "\n";
            output += "Collision List\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            
            for (int i = 0; i < numLvls; i++)
            {
                for (int j = 0; j < sharedIdx[i][0]; j++) {
                    HierarchicalGpuESC.clCollisionSublist act = sublists[i][j];
                    output += String.Format("actCellID = {0:x} x={1} y={2} z={3} startIdx= {4} NativeHome = {5}NativePhant = {6} ImportHome = {7} TotalCnt = {8}\n", readCellIds[i][act.startIdx], debugCell3DPosList[i][act.startIdx].x, debugCell3DPosList[i][act.startIdx].y,0, act.startIdx, act.nHome,act.nPhant,act.iHome, act.objCnt);
                    output += String.Format("Colliding objects:\n");
                    for (int k = 0; k < act.objCnt; k++) {
                        output += string.Format("[{0}{1}]:{2}, ", ((cellData[i][k] & NATIVE) > 0) ? " N" : " I", ((cellData[i][k] & HOMECELL) > 0) ? "H" : " P", cellData[i][k] & ((long) 0xFFFFFFFF));
                    }
                    //                    }
                    output += "\n\n";
                }

            }

            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }
        }

        public static void PrintCollisionTuples(CollisionTupel[][] collisionTupels, uint[][] cellIds, ulong[][] cellData, CollisionCell2D[][] readLvlCell2DPosList, string caption, int[][] clSharedIdxMem, uint numLvls) {
            String output = caption;
            output += "\n";
            output += "\n";
            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            for (int i = 0; i < numLvls; i++)
            {
                for (int j = 0; j < clSharedIdxMem[i][0]; j++) {

                    uint index1 = collisionTupels[i][j].obj1;
                    uint index2 = collisionTupels[i][j].obj2;
                    output += String.Format("CollisionCheck at {0:x}:\n", cellIds[i][index1]);

                    output += String.Format("CollisionTupelElement 2 = {0:x} x={1} y={2} currentLvl {3}\n", cellData[i][index2] & 0xFFFFFFFF, readLvlCell2DPosList[i][index2].x, readLvlCell2DPosList[i][index2].y, i);

                    output += "\n";
                }
                
            }

            output += "----------------------------------------------------------------------------------------------------------------------------------------------------------\n";
            using (StreamWriter sw = File.AppendText(log))
            {
                sw.WriteLine(output);
            }
        }
    } 
}
