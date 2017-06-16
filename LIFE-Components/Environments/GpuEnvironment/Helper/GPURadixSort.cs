using System;
using System.Collections.Generic;
using System.IO;
using Cloo;

namespace GpuEnvironment.Helper
{
    public class GPURadixSort
    {
        private static int Log_Idx = 0;
        private const bool DEBUG = true;
        private const bool DEBUG_CONSOLE_OUTPUT = false;
        private readonly string debugLog = Path.Combine(Directory.GetCurrentDirectory(), "OpenCLDebugLog.txt");
        private static string sortLog = Path.Combine(Directory.GetCurrentDirectory(), "sortLog");
//        private string programPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\GPURadixSort\RadixSort.cl");

        public struct GPUConstants
        {
            public int numRadices;
            public int numBlocks;
            public int numGroupsPerBlock;
            public int R;
            public int numThreadsPerGroup;
            public int numElementsPerGroup;
            public int numRadicesPerBlock;
            public int bitMask;
            public int L;
            public int numThreadsPerBlock;
            public int numTotalElements;
        }


        private ComputeContext cxGPUContext { get; set; }
        private ComputeCommandQueue m_cqCommandQueue { get; set; }
        private ComputeDevice _device { get; set; }

        private GPUConstants gpuConstants;


        private ComputeBuffer<uint> mInputBuff;
        private ComputeBuffer<uint> m_Counters;
        private ComputeBuffer<uint> m_RadixPrefixes;
        private ComputeBuffer<ulong> mOutputBuff;
        //private CLMemoryHandle mBlockOffsets;

        private const int numCounters = num_Radices * NumGroupsPerBlock * numBlocks;

        // Anzahl an Bits die als Buckets für jeden radix durchlauf verwendet werden
        private const int radix_BitsL = 4;
        private const int num_Radices = 1 << radix_BitsL;

        // Auch Thread Blocks unter CUDA -> Gruppe von Threads mit gemeinsamen shared memory.
        private const int numBlocks = 4;

        // Anzahl von WorkItems / Threads, die sich in einer Work-Group befinden
        private const int numThreadsPerBlock = 32;

        private const int R = 8;

        private const int NumGroupsPerBlock = numThreadsPerBlock / R;

        private const int BIT_MASK_START = 0xF;
        private const int BITS_PER_BYTE = 8;

        uint[] counters = new uint[numCounters];




        ComputeKernel ckSetupAndCount; // OpenCL kernels
        ComputeKernel ckSumIt;
        ComputeKernel m_ckReorderingKeysOnly;
        ComputeKernel ckReorderingKeyValue;
        //Kernel ckReorderDataKeysOnly;


        private int maxElements;
        private uint[] m_debugReadUint;
        private ulong[] m_debugReadValues;




    

        public GPURadixSort(
            ComputeCommandQueue cqCommandQue,
            ComputeContext context,
            ComputeDevice device
        )
        {

            gpuConstants = new GPUConstants();
            gpuConstants.L = radix_BitsL;
            gpuConstants.numGroupsPerBlock = NumGroupsPerBlock;
            gpuConstants.R = R;
            gpuConstants.numThreadsPerGroup = numThreadsPerBlock / NumGroupsPerBlock;
            gpuConstants.numThreadsPerBlock = numThreadsPerBlock;
            gpuConstants.numBlocks = numBlocks;
            gpuConstants.numRadices = num_Radices;
            gpuConstants.numRadicesPerBlock = num_Radices / numBlocks;
            gpuConstants.bitMask = BIT_MASK_START;
            counters.Initialize();

            // TODO : Add exceptionhandling
            cxGPUContext = context;
            m_cqCommandQueue = cqCommandQue;
            _device = device;

            string programSource = GpuOpenClCode.RadixSortKernel;
            IntPtr[] progSize = new IntPtr[] { (IntPtr)programSource.Length };
            string flags = "-cl-fast-relaxed-math";

            ComputeProgram prog = new ComputeProgram(context, programSource);
            try
            {
                prog.Build(new List<ComputeDevice>() { device }, flags, null, IntPtr.Zero);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            if (prog.GetBuildStatus(device) != ComputeProgramBuildStatus.Success)
            {
                Console.WriteLine(prog.GetBuildLog(device));
                throw new ArgumentException("UNABLE to build programm");
            }

            ckSetupAndCount = prog.CreateKernel("SetupAndCount");
            ckSumIt = prog.CreateKernel("SumIt");
            m_ckReorderingKeysOnly = prog.CreateKernel("ReorderingKeysOnly");
            ckReorderingKeyValue = prog.CreateKernel("ReorderingKeyValue");
        }






        public void sortKeysOnly(ComputeBuffer<uint> input, ComputeBuffer<uint> output,
            int numElements)
        {
            m_debugReadUint = new uint[Math.Max(numElements, numCounters)];

            m_Counters = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, counters);

            m_RadixPrefixes = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, gpuConstants.numRadices);

            gpuConstants.numElementsPerGroup = (numElements / (gpuConstants.numBlocks * gpuConstants.numGroupsPerBlock)) + 1;
            gpuConstants.numTotalElements = numElements;


            ComputeEventList eventList = new ComputeEventList();
            if (DEBUG)
            {
                m_cqCommandQueue.ReadFromBuffer<uint>(input, ref m_debugReadUint, true, eventList);
                PrintAsArray(m_debugReadUint, gpuConstants.numTotalElements);
            }
            int i;
            for (i = 0; i < sizeof(uint) * BITS_PER_BYTE / radix_BitsL; i++)
            {
                m_cqCommandQueue.WriteToBuffer(counters, m_Counters, true, eventList);

                if (i % 2 == 0)
                {
                    DateTime before = DateTime.Now;
                    SetupAndCount(input, radix_BitsL * i);
                    if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("Setup and Count =" + (DateTime.Now - before).TotalMilliseconds);

                    before = DateTime.Now;
                    SumIt(input, radix_BitsL * i);
                    if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("SumIt =" + (DateTime.Now - before).TotalMilliseconds);

                    before = DateTime.Now;
                    ReorderingKeysOnly(input, output, radix_BitsL * i);
                    if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("Reorder =" + (DateTime.Now - before).TotalMilliseconds);

                }
                else
                {
                    SetupAndCount(output, radix_BitsL * i);
                    SumIt(output, radix_BitsL * i);
                    ReorderingKeysOnly(output, input, radix_BitsL * i);
                }

            }
            if (i % 2 != 0)
            {
                m_cqCommandQueue.CopyBuffer(input, output, eventList);
                m_cqCommandQueue.Finish();
            }
            //  m_RadixPrefixes.;
            m_RadixPrefixes.Dispose();
            m_Counters.Dispose();
            Log_Idx++;

        }


        public void sortKeysValue(ComputeBuffer<uint> inputKey, ComputeBuffer<uint> outputKey, ComputeBuffer<ulong> inputValue, ComputeBuffer<ulong> outputValue,
            int numElements)
        {
            m_debugReadUint = new uint[Math.Max(numElements, numCounters)];
            m_debugReadValues = new ulong[Math.Max(numElements, numCounters)];

            m_Counters = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, counters);


            m_RadixPrefixes = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, gpuConstants.numRadices);


            ComputeEventList eventList = new ComputeEventList();
            if (DEBUG)
            {
                m_cqCommandQueue.ReadFromBuffer<uint>(inputKey, ref m_debugReadUint, true, eventList);
                PrintAsArray(m_debugReadUint, gpuConstants.numTotalElements);
            }
            gpuConstants.numElementsPerGroup = (numElements / (gpuConstants.numBlocks * gpuConstants.numGroupsPerBlock)) + 1;
            gpuConstants.numTotalElements = numElements;
            int i;
            for (i = 0; i < sizeof(uint) * BITS_PER_BYTE / radix_BitsL; i++)
            {
                m_cqCommandQueue.WriteToBuffer(counters, m_Counters, true, eventList);

                if (i % 2 == 0)
                {
                    SetupAndCount(inputKey, radix_BitsL * i);
                    SumIt(inputKey, radix_BitsL * i);
                    ReorderingKeyValue(inputKey, outputKey, inputValue, outputValue, radix_BitsL * i);
                }
                else
                {
                    SetupAndCount(outputKey, radix_BitsL * i);
                    SumIt(outputKey, radix_BitsL * i);
                    ReorderingKeyValue(outputKey, inputKey, outputValue, inputValue, radix_BitsL * i);
                }

            }
            if (i % 2 != 0)
            {
                m_cqCommandQueue.CopyBuffer(inputKey, outputKey, eventList);
                m_cqCommandQueue.CopyBuffer(inputValue, outputValue, eventList);
                m_cqCommandQueue.Finish();

            }
            m_RadixPrefixes.Dispose();
            m_Counters.Dispose();

            Log_Idx++;

        }


        public void sortKeysValue(ComputeBuffer<uint> key, ComputeBuffer<ulong> value,
            int numElements)
        {
            m_debugReadUint = new uint[Math.Max(numElements, numCounters)];
            m_debugReadValues = new ulong[Math.Max(numElements, numCounters)];


            m_Counters = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite | ComputeMemoryFlags.CopyHostPointer, counters);

            m_RadixPrefixes = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, gpuConstants.numRadices);

            ComputeBuffer<uint> outputKey = new ComputeBuffer<uint>(cxGPUContext, ComputeMemoryFlags.ReadWrite, numElements);
            ComputeBuffer<ulong> outputValue = new ComputeBuffer<ulong>(cxGPUContext, ComputeMemoryFlags.ReadWrite, numElements);



            ComputeEventList eventList = new ComputeEventList();

            gpuConstants.numElementsPerGroup = (numElements / (gpuConstants.numBlocks * gpuConstants.numGroupsPerBlock)) + 1;
            gpuConstants.numTotalElements = numElements;
            int i;
            for (i = 0; i < sizeof(uint) * BITS_PER_BYTE / radix_BitsL; i++)
            {
                m_cqCommandQueue.WriteToBuffer(counters, m_Counters, true, eventList);
                if (i % 2 == 0)
                {
                    SetupAndCount(key, radix_BitsL * i);
                    SumIt(key, radix_BitsL * i);
                    ReorderingKeyValue(key, outputKey, value, outputValue, radix_BitsL * i);
                }
                else
                {
                    SetupAndCount(outputKey, radix_BitsL * i);
                    SumIt(outputKey, radix_BitsL * i);
                    ReorderingKeyValue(outputKey, key, outputValue, value, radix_BitsL * i);
                }

            }
            if (i % 2 == 0)
            {
                m_cqCommandQueue.CopyBuffer(outputKey, key, eventList);
                m_cqCommandQueue.CopyBuffer(outputValue, value, eventList);
                m_cqCommandQueue.Finish();

            }


            outputKey.Dispose();
            outputValue.Dispose();
            m_RadixPrefixes.Dispose();
            m_Counters.Dispose();
            Log_Idx++;

        }




        private void ReorderingKeysOnly(ComputeBuffer<uint> input, ComputeBuffer<uint> output, int bitOffset)
        {


            long[] globalWorkSize = new long[] { gpuConstants.numThreadsPerBlock * gpuConstants.numBlocks };
            long[] localWorkSize = new long[] { gpuConstants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            m_ckReorderingKeysOnly.SetMemoryArgument(0, input);
            m_ckReorderingKeysOnly.SetMemoryArgument(1, output);
            m_ckReorderingKeysOnly.SetMemoryArgument(2, m_Counters);
            m_ckReorderingKeysOnly.SetMemoryArgument(3, m_RadixPrefixes);
            m_ckReorderingKeysOnly.SetLocalArgument(4, gpuConstants.numGroupsPerBlock * gpuConstants.numBlocks * gpuConstants.numRadices * 4);
            m_ckReorderingKeysOnly.SetLocalArgument(5, gpuConstants.numRadices * 4);
            m_ckReorderingKeysOnly.SetValueArgument(6, gpuConstants);
            m_ckReorderingKeysOnly.SetValueArgument(7, bitOffset);
            m_cqCommandQueue.Execute(m_ckReorderingKeysOnly, null, globalWorkSize, localWorkSize, eventList);
            m_cqCommandQueue.Finish();


            if (DEBUG)
            {

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Input                ");
                m_cqCommandQueue.ReadFromBuffer(input, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numTotalElements, "Reordering -> Input -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Counters                ");
                m_cqCommandQueue.ReadFromBuffer(m_Counters, ref m_debugReadUint, true, eventList);
                PrintCounterBuffer(m_debugReadUint, "Reordering -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Counters                ");
                m_cqCommandQueue.ReadFromBuffer(m_RadixPrefixes, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numRadices, "Reordering -> RadixPrefixe -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Output                ");
                m_cqCommandQueue.ReadFromBuffer(output, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numTotalElements, "Reordering -> Output -> bitoffset = " + bitOffset);


                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("Reordering -> bitoffset = " + bitOffset);
                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine();
            };

        }

        private void ReorderingKeyValue(ComputeBuffer<uint> inputKey, ComputeBuffer<uint> outputKey, ComputeBuffer<ulong> inputValue, ComputeBuffer<ulong> outputValue, int bitOffset)
        {


            long[] globalWorkSize = new long[] { gpuConstants.numThreadsPerBlock * gpuConstants.numBlocks };
            long[] localWorkSize = new long[] { gpuConstants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            ckReorderingKeyValue.SetMemoryArgument(0, inputKey);
            ckReorderingKeyValue.SetMemoryArgument(1, outputKey);
            ckReorderingKeyValue.SetMemoryArgument(2, inputValue);
            ckReorderingKeyValue.SetMemoryArgument(3, outputValue);
            ckReorderingKeyValue.SetMemoryArgument(4, m_Counters);
            ckReorderingKeyValue.SetMemoryArgument(5, m_RadixPrefixes);
            ckReorderingKeyValue.SetLocalArgument(6, gpuConstants.numGroupsPerBlock * gpuConstants.numBlocks * gpuConstants.numRadicesPerBlock * 4);
            ckReorderingKeyValue.SetLocalArgument(7, gpuConstants.numRadices * 4);
            ckReorderingKeyValue.SetValueArgument(8, gpuConstants);
            ckReorderingKeyValue.SetValueArgument(9, bitOffset);
            m_cqCommandQueue.Execute(ckReorderingKeyValue, null, globalWorkSize, localWorkSize, eventList);
            m_cqCommandQueue.Finish();

            if (DEBUG)
            {
                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("-------------------------------Reordering-------------------------------------------------");


                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Input                ");
                m_cqCommandQueue.ReadFromBuffer(inputKey, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numTotalElements, "Reordering -> Inputkey -> bitoffset = " + bitOffset);

                m_cqCommandQueue.ReadFromBuffer(inputValue, ref m_debugReadValues, true, eventList);
                PrintElementBuffer(m_debugReadValues, gpuConstants.numTotalElements, "Reordering -> InputValues -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Counters                ");
                m_cqCommandQueue.ReadFromBuffer(m_Counters, ref m_debugReadUint, true, eventList);
                PrintCounterBuffer(m_debugReadUint, "Reordering -> bitoffset = " + bitOffset);


                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Counters                ");
                m_cqCommandQueue.ReadFromBuffer(m_RadixPrefixes, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numRadices, "Reordering -> RadixPrefixe -> bitoffset = " + bitOffset);



                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Output                ");
                m_cqCommandQueue.ReadFromBuffer(outputKey, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numTotalElements, "Reordering -> Output -> bitoffset = " + bitOffset);
                m_cqCommandQueue.ReadFromBuffer(outputValue, ref m_debugReadValues, true, eventList);
                PrintElementBuffer(m_debugReadValues, gpuConstants.numTotalElements, "Reordering -> Output -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("Reordering -> bitoffset = " + bitOffset);
                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine();
            };

        }
        private void SumIt(ComputeBuffer<uint> input, int bitOffset)
        {


            long[] globalWorkSize = new long[] { gpuConstants.numThreadsPerBlock * gpuConstants.numBlocks };
            long[] localWorkSize = new long[] { gpuConstants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            ckSumIt.SetMemoryArgument(0, input);
            ckSumIt.SetMemoryArgument(1, m_Counters);
            ckSumIt.SetMemoryArgument(2, m_RadixPrefixes);
            ckSumIt.SetValueArgument(3, gpuConstants);
            ckSumIt.SetValueArgument(4, bitOffset);
            ckSumIt.SetLocalArgument(5, 4 * gpuConstants.numBlocks * gpuConstants.numGroupsPerBlock);
            m_cqCommandQueue.Execute(ckSumIt, null, globalWorkSize, localWorkSize, eventList);
            m_cqCommandQueue.Finish();

            if (DEBUG)
            {


                m_cqCommandQueue.ReadFromBuffer(input, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numTotalElements, "SumIt -> Input -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("              Counters                ");
                m_cqCommandQueue.ReadFromBuffer(m_Counters, ref m_debugReadUint, true, eventList);
                PrintCounterBuffer(m_debugReadUint, "SumIt -> bitoffset = " + bitOffset);


                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("SumIt -> bitoffset = " + bitOffset);
                m_cqCommandQueue.ReadFromBuffer(m_RadixPrefixes, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numRadices, "SumIt -> RadixPrefixe -> bitoffset = " + bitOffset);
                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine();
            };
        }

        private void SetupAndCount(ComputeBuffer<uint> input, int bitOffset)
        {


            long[] globalWorkSize = new long[] { gpuConstants.numThreadsPerBlock * gpuConstants.numBlocks };
            long[] localWorkSize = new long[] { gpuConstants.numThreadsPerBlock };

            ComputeEventList eventList = new ComputeEventList();
            ckSetupAndCount.SetMemoryArgument(0, input);
            ckSetupAndCount.SetMemoryArgument(1, m_Counters);
            ckSetupAndCount.SetValueArgument(2, gpuConstants);
            ckSetupAndCount.SetValueArgument(3, bitOffset);
            m_cqCommandQueue.Execute(ckSetupAndCount, null, globalWorkSize, localWorkSize, eventList);
            m_cqCommandQueue.Finish();


            if (DEBUG)
            {
                

                m_cqCommandQueue.ReadFromBuffer(input, ref m_debugReadUint, true, eventList);
                PrintElementBuffer(m_debugReadUint, gpuConstants.numTotalElements, "Setup and Count -> Input  -> bitoffset = " + bitOffset);
                //
                m_cqCommandQueue.ReadFromBuffer(m_Counters, ref m_debugReadUint, true, eventList);
                PrintCounterBuffer(m_debugReadUint, "Setup and Count -> bitoffset = " + bitOffset);
                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine("Setup and Count -> bitoffset = " + bitOffset);

                if (DEBUG_CONSOLE_OUTPUT) Console.WriteLine();
            }

        }

        private void PrintElementBuffer(uint[] printData, int count, string caption)
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
            using (StreamWriter sw = File.AppendText(sortLog + Log_Idx + ".txt"))
            {
                sw.WriteLine(output);
            }
        }

        private void PrintElementBuffer(ulong[] printData, int count, string caption)
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
            using (StreamWriter sw = File.AppendText(sortLog + Log_Idx + ".txt"))
            {
                sw.WriteLine(output);
            }
        }
        private void PrintCounterBuffer(uint[] printData, string caption)
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
            using (StreamWriter sw = File.AppendText(sortLog + Log_Idx + ".txt"))
            {
                sw.WriteLine(output);
            }
        }

        public static void PrintAsArray(uint[] values, int count)
        {
            string output = "static ulong[] data= new ulong[]{ ";
            for (int i = 0; i < count - 1; i++)
            {
                output += string.Format("0x{0:x} ,", values[i]);
            }
            output += string.Format("0x{0:x} }};", values[values.Length - 1]);

            using (StreamWriter sw = File.AppendText(sortLog + Log_Idx + ".txt"))
            {
                sw.WriteLine(output);
            }
        }


    }
}