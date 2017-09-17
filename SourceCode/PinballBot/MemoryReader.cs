using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace PinballBot
{
    class MemoryReader
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
        Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);
        public byte[] ReadMemory(Process process, Int64 address, int numOfBytes, int bytesRead)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, process.Id);
            byte[] buffer = new byte[numOfBytes];
            bytesRead = 0;
            ReadProcessMemory((int)hProc, address, buffer, numOfBytes, ref bytesRead);
            return buffer;
        }
        public float getValue(String name, int Base, int[] offsets, String type)
        {
            int Health = 0;
            int bytesRead = 0;
            byte[] valueOut;
            float heelt = 0;
            if (name != "harald")
            {
                long BaseAddress = 0;
                Process myProcess = Process.GetProcessesByName(name).FirstOrDefault();

                Console.WriteLine(myProcess.ProcessName);
                Console.WriteLine("Base" + Base);
                ProcessModule myProcessModule = null;
                ProcessModuleCollection myProcessModuleCollection;

                myProcessModuleCollection = myProcess.Modules;

                //int bytesRead = 0;
                for (int i = 0; i < myProcessModuleCollection.Count; i++)
                {
                    myProcessModule = myProcessModuleCollection[i];
                    if (myProcessModule.ModuleName.Contains(name))
                    {
                        BaseAddress = (Int64)myProcessModule.BaseAddress;
                        break;
                    }
                }
                valueOut = ReadMemory(myProcess, Base + BaseAddress, 4, bytesRead); ///read address 1

                int value = BitConverter.ToInt32(valueOut, 0); /// convert to Int32
                string newAddr = value.ToString("X");  // convert to hex
                IntPtr mpAddr = (IntPtr)Convert.ToInt32(newAddr, 16); // convert to decimal as IntPtr
                Health = BitConverter.ToInt32(valueOut, 0);
                Console.WriteLine("Value Found it was" + Health);

                for (int i = 0; i < offsets.Length; i++)
                {

                    if (i != offsets.Length - 1)
                    {

                        mpAddr = IntPtr.Add(mpAddr, offsets[offsets.Length - 1 - i]); // add offset
                        value = mpAddr.ToInt32(); // convert to int for address 2
                        Console.WriteLine("Added" + offsets[offsets.Length - 1 - i]);
                        valueOut = ReadMemory(myProcess, value, 4, bytesRead); // read address 2
                        value = BitConverter.ToInt32(valueOut, 0); // convert to int32
                        newAddr = value.ToString("X"); //convert to hex
                        mpAddr = (IntPtr)Convert.ToInt32(newAddr, 16); // convert to decimal as IntPtr
                        Console.WriteLine("Value Found it was" + value);



                    }
                    else
                    {
                        mpAddr = IntPtr.Add(mpAddr, offsets[offsets.Length - 1 - i]); // add offset
                        value = mpAddr.ToInt32(); // convert to int for address 2
                        if (type == "Float")
                        {
                            valueOut = ReadMemory(myProcess, value, 8, bytesRead);
                            heelt = BitConverter.ToSingle(valueOut, 0);
                            return heelt;
                        }
                        else if (type == "Int")
                        {
                            valueOut = ReadMemory(myProcess, value, 4, bytesRead); // read address 2
                            Health = BitConverter.ToInt32(valueOut, 0);
                            Console.WriteLine("Value Found it was" + Health);
                            return Health;
                        }


                        i = 99;
                    }


                }
            }
            else
            {
                Health = 9999;
            }




            Console.WriteLine("Value Found it was" + Health + " amount of offsets was" + offsets.Length);
            return heelt;
        }
        public float getsingleValue(String name, int Base, String type)
        {
            int Health = 0;
            int bytesRead = 0;
            byte[] valueOut;
            float heelt = 0;

            Process myProcess = Process.GetProcessesByName(name).FirstOrDefault();


            //Console.WriteLine("Base" + Base);

            if (type == "Float")
            {
                valueOut = ReadMemory(myProcess, Base, 8, bytesRead);
                heelt = BitConverter.ToSingle(valueOut, 0);
                return heelt;
            }
            else if (type == "Int")
            {
                valueOut = ReadMemory(myProcess, Base, 4, bytesRead); ///read address 1
                Health = BitConverter.ToInt32(valueOut, 0);
                Console.WriteLine("Value Found it was " + Health);
                return Health;
            }
            else if (type == "Byte")
            {
                valueOut = ReadMemory(myProcess, Base, 1, bytesRead); ///read address 1
                Health = (valueOut[0]);
                Console.WriteLine("Value Found it was " + Health);
                return Health;
            }

            return 0;





        }
    }
}
