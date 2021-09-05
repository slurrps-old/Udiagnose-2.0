//Author: Kenneth Lamb

using System;
using System.Management;
using System.Diagnostics;
using System.Collections.Generic;

namespace UDiagnose_2._0.Classes
{
    //Author: Kenneth Lamb
    //Purpose:  This class contains the definiton of a SystemInfo class. It contains no private variables however 
    // it does contain 21 methods that pull the computer hardware information from the system the application is on
    //There may be more added on as more hardware info is requested in the program
    //This uses System.Management so that you can Query the WMI Classes on windows.
    // A summary of the methods appears below:
    // 
    // Assumptions: That the locations for the readings will be in the same place
    //
    // Exception Handling: None as all of the queries are properly made to always inquire the appropriate information from the OS
    //
    // Summary of Methods:


    class SystemInfo
    {
        //Default constructor
        //Pre: Object has been instantiated
        //Post: Class has been initialized does not need anything
        //Purpose: initialized a class object
        public SystemInfo()
        {
            //Default contructor
            //This class needs nothing to be inputted as it is just an inquiry class to search information.
        }

        #region System  and Computer Information
        readonly static ManagementClass mSE = new ManagementClass("Win32_SystemEnclosure");
        readonly ManagementObjectCollection moSE = mSE.GetInstances();

        readonly static ManagementClass mCS = new ManagementClass("Win32_ComputerSystem");
        readonly ManagementObjectCollection moCS = mCS.GetInstances();

        protected string GetName()
        {
            string name = string.Empty;
            foreach(ManagementObject mo in moCS)
            {
                name = mo.Properties["Name"].Value.ToString();
                break;
            }

            return name;
        }

        protected string GetSystemStatus()
        {
            string model = string.Empty;
            foreach(ManagementObject mo in moCS)
            {
                model = mo.Properties["Status"].Value.ToString();
            }
            return model;
        }

        #region System Type
        protected string GetSystemType()
        {
            //Int variable to hold the chassis type
            int typeNum = 0;
            //empty string to hold the switch conversion
            string type = string.Empty;
            //Foreach to go through the objects
            foreach(ManagementObject mo in moSE)
            {
                //Foreach to go through all of the 
                foreach (int i in (UInt16[])mo.Properties["ChassisTypes"].Value)
                {
                    //Assign i to typNum
                    typeNum = i;
                }
            }

            #region Switch SystemType
            //Use switch to check typNum int and assign the proper string to type
            switch (typeNum)
            {
                case 1:
                    type = "Other";
                    break;
                case 2:
                    type = "Unknown";
                    break;
                case 3:
                    type = "Desktop";
                    break;
                case 4:
                    type = "Low Profile Desktop";
                    break;
                case 5:
                    type = "Pizza Box";
                    break;
                case 6:
                    type = "Mini Tower";
                    break;
                case 7:
                    type = "Tower";
                    break;
                case 8:
                    type = "Portable";
                    break;
                case 9:
                    type = "Laptop";
                    break;
                case 10:
                    type = "Notebook";
                    break;
                case 11:
                    type = "Hand Held";
                    break;
                case 12:
                    type = "Docking Station";
                    break;
                case 13:
                    type = "All in One";
                    break;
                case 14:
                    type = "Sub Notebook";
                    break;
                case 15:
                    type = "Space-Saving";
                    break;
                case 16:
                    type = "Lunch Box";
                    break;
                case 17:
                    type = "Main System Chassis";
                    break;
                case 18:
                    type = "Expansion Chassis";
                    break;
                case 19:
                    type = "SubChassis";
                    break;
                case 20:
                    type = "Bus Expansion Chassis";
                    break;
                case 21:
                    type = "Peripheral Chassis";
                    break;
                case 22:
                    type = "Storage Chassis";
                    break;
                case 23:
                    type = "Rack Mount Chassis";
                    break;
                case 24:
                    type = "Sealed-Case PC";
                    break;
                case 30:
                    type = "Tablet";
                    break;
                case 31:
                    type = "Convertible";
                    break;
                case 32:
                    type = "Detachable";
                    break;
              
            }
            #endregion
            //Return string type
            return type;
        }
        #endregion

        #region Laptop Info
        protected string GetSystemManufacturer()
        {
            string model = string.Empty;
            foreach (ManagementObject mo in moSE)
            {
                try
                {
                    model = mo.Properties["Manufacturer"].Value.ToString();
                }
                catch
                {

                }
                
            }
            return model;
        }
        protected string GetSerialNum()
        {
            string model = string.Empty;
            foreach (ManagementObject mo in moSE)
            {
                try
                {
                    model = mo.Properties["SerialNumber"].Value.ToString();
                }
                catch
                {

                }
                
            }
            return model;
        }
        #endregion
        #endregion

        #region OS Region
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //OS Information
        //Retrieves OS information
        protected string GetOSInformation()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return ((string)wmi["Caption"]).Trim() + ", " + (string)wmi["Version"] + ", " + (string)wmi["OSArchitecture"];
                }
                catch { }
            }
            return "BIOS Maker: Unknown";
        }
        #endregion

        #region CPU Information
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CPU Information ------------------------------------------------------------
        //Retrieves processorID
        readonly static ManagementClass mcCpu = new ManagementClass("win32_processor");
        readonly ManagementObjectCollection mocCpu = mcCpu.GetInstances();

        #region CPU Name Details
        protected String GetProcessorId()
        {
            String Id = String.Empty;
            foreach (ManagementObject mo in mocCpu)
            {

                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return Id;

        }

        //Retrieves processor information
        protected String GetProcessorInformation()
        {
            String info = String.Empty;
            foreach (ManagementObject mo in mocCpu)
            {
                string name = (string)mo["Name"];
                name = name.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®").Replace("(r)", "®").Replace("(C)", "©").Replace("(c)", "©").Replace("    ", " ").Replace("  ", " ");

                info = name;
                //mo.Properties["Name"].Value.ToString();
                //break;
            }
            return info;
        }

        //Processor sub information
        protected string GetProcessorSubInfo()
        {
            String info = String.Empty;
            foreach (ManagementObject mo in mocCpu)
            {
                info = (string)mo["Caption"] + ", " + (string)mo["SocketDesignation"];
            }

            return info;
        }
        #endregion

        #region CPU Speeds Info
        //Retrieves processor speed
        protected double GetCpuMaxSpeedInGHz()
        {
            double GHz = 0.0;
            using (mcCpu)
            {
                foreach (ManagementObject mo in mocCpu)
                {
                    GHz = 0.001 * (UInt32)mo.Properties["MaxClockSpeed"].Value;
                    break;
                }
            }
            return GHz;
        }

        ////////////////////////////////////////////////////////////////////////
        //Retrieves current clock speed of cpu
        public double GetCPUCurrentClockSpeed()
        {
            double cpuClockSpeed = 0;

            using (ManagementObject mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
            {
                cpuClockSpeed = 0.001 * (UInt32)mo.Properties["CurrentClockSpeed"].Value;
            }

            return cpuClockSpeed;
        }
        #endregion

        #region CPU Detail Info
        //Retrieves Processor all levels of cache
        protected int[] GetCache()
        {
            int[] cache = new int[3];

            using (ManagementClass mc = new ManagementClass("Win32_CacheMemory"))
            {
                int i = 0;
                foreach (ManagementObject mo in mc.GetInstances())
                {
                    cache[i] += (Convert.ToInt32(mo.Properties["MaxCacheSize"].Value));

                    i++;
                    //break;
                }
            }
            return cache;
        }

        //Get Revision of CPU
        protected Int16 GetProcessorRevision()
        {
            Int16 revision = 0;
            using (mcCpu)
            {
                foreach (ManagementObject mo in mocCpu)
                {
                    revision = (Convert.ToInt16(mo.Properties["Revision"].Value));
                    break;
                }
            }
            return revision;

        }

        //Get Virtualization Boolean
        protected bool GetVirtualization()
        {
            bool isVirtualized = false;
            using (mcCpu)
            {
                foreach (ManagementObject mo in mocCpu)
                {
                    isVirtualized = (Convert.ToBoolean(mo.Properties["VirtualizationFirmwareEnabled"].Value));
                    break;
                }
            }
            return isVirtualized;
        }

        //Retrieves the number of cores on a CPU
        protected int GetNumberCores()
        {
            int cores = 0;

            foreach (ManagementObject obj in mocCpu)
            {
                cores = Convert.ToInt32(obj.Properties["NumberOfCores"].Value.ToString());
            }

            return cores;
        }

        //Retrieves the number of threads on a CPU
        protected int GetNumberThreads()
        {
            int threads = 0;

            foreach (ManagementObject obj in mocCpu)
            {
                threads = Convert.ToInt32(obj.Properties["ThreadCount"].Value.ToString());
            }

            return threads;
        }
        #endregion

        #endregion

        #region RAM Information
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //RAM Information---------------------------------------------------------------------------
        //Gets the physical memory amount installed into the computer

        readonly static ManagementClass mcRAM = new ManagementClass("Win32_PhysicalMemory");
        readonly ManagementObjectCollection mocRAM = mcRAM.GetInstances();
        protected long GetPhysicalMemory()
        {
            
            long MemSize = 0;
            long mCap;

            // In case more than one Memory sticks are installed
            foreach (ManagementObject mo in mocRAM)
            {
                mCap = Convert.ToInt64(mo["Capacity"]);
                MemSize += mCap;
            }
            MemSize = (MemSize / 1024) / 1024;
            return MemSize;
        }

        //Gets the number of ram slots on the computer
        protected string GetNoRamSlots()
        {

            int MemSlots = 0;
            ManagementScope oMs = new ManagementScope();
            ObjectQuery oQuery2 = new ObjectQuery("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
            ManagementObjectSearcher oSearcher2 = new ManagementObjectSearcher(oMs, oQuery2);
            ManagementObjectCollection oCollection2 = oSearcher2.Get();
            foreach (ManagementObject obj in oCollection2)
            {
                MemSlots = Convert.ToInt32(obj["MemoryDevices"]);

            }
            return MemSlots.ToString();
        }

        //Ram Speed
        protected int GetRAMSpeed()
        {
            int clockSpeed = 0;

            foreach (ManagementObject mo in mocRAM)
            {
                clockSpeed = Convert.ToInt32(mo["ConfiguredClockSpeed"]);

            }
            return clockSpeed;
        }

        //Ram Manufacturer
        protected string GetRAMManufact()
        {
            foreach (ManagementObject mo in mocRAM)
            {
                try
                {
                    return mo.GetPropertyValue("Manufacturer").ToString();
                }
                catch { }
            }

            return "RAM Maker: Unknown";
        }

        protected int GetRAMInstalled()
        {
            int numRAMSlotsUsed = 0;
            string bankLabel;
            foreach(ManagementObject mo in mocRAM)
            {
                bankLabel = mo.GetPropertyValue("BankLabel").ToString();
                numRAMSlotsUsed++;
            }
            return numRAMSlotsUsed;
        }

        protected string GetFormFactor()
        {
            string formFactor = "";
            int formNumber = 0;
           
            foreach (ManagementObject mo in mocRAM)
            {
                formNumber = Convert.ToInt32(mcRAM["FormFactor"]);
            }

            switch (formNumber)
            {
                case 0: formFactor = "Unknown";
                    break;

                case 1:formFactor = "Other";
                    break;

                case 8:formFactor = "DIMM";
                    break;

                case 12:formFactor = "SODIMM";
                    break;
            }


            return formFactor;
        }
        #endregion

        #region Motherboard Information
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Motherboard Information
        readonly static ManagementClass mcMobo = new ManagementClass("win32_BaseBoard");
        readonly ManagementObjectCollection mocMobo = mcMobo.GetInstances();
        //Mobo Maker
        protected string GetBoardMaker()
        {

            
            String Id = String.Empty;
            foreach (ManagementObject mo in mocMobo)
            {

                Id = mo.Properties["Manufacturer"].Value.ToString();
                break;
            }
            return Id;

        }

        //Mobo Product ID
        protected string GetBoardProductId()
        {
                        
            String Id = String.Empty;
            foreach (ManagementObject mo in mocMobo)
            {

                Id = mo.Properties["Product"].Value.ToString();
                break;
            }
            return Id;

        }

        //Get Motherboard Serial number
        protected string GetBoardSerialNumber()
        {
            String Id = String.Empty;
            foreach (ManagementObject mo in mocMobo)
            {

                Id = mo.Properties["SerialNumber"].Value.ToString();
                break;
            }
            return Id;

        }
        #endregion

        #region BIOS Information
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Bios Information
        readonly static ManagementClass mcBIOS = new ManagementClass("Win32_BIOS");
        readonly ManagementObjectCollection mocBIOS = mcBIOS.GetInstances();
        //Retrieves BIOS Maker
        protected string GetBIOSmaker()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString();
                }
                catch { }
            }
            return "BIOS Maker: Unknown";
        }

        //BIOS Serial Number
        protected string GetBIOSVersion()
        {
            
            String Id = String.Empty;
            foreach (ManagementObject mo in mocBIOS)
            {

                Id = mo.Properties["Version"].Value.ToString();
                break;
            }
            return Id;

        }

        //BIOS Caption
        protected string GetBIOScaption()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BIOS");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Caption").ToString();
                }
                catch { }
            }
            return "BIOS Caption: Unknown";
        }
        #endregion

        #region GPU Information
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //GPU Information
        readonly static ManagementClass mcGPU = new ManagementClass("Win32_VideoController");
        readonly ManagementObjectCollection mocGPU = mcGPU.GetInstances();

        //public object Diagnostics { get; internal set; }

        //In process of being worked on

        protected string GetGPUInformation() //Gets the processor information
        {
            string strGPU = "";

            

            foreach (ManagementObject mo in mocGPU)
            {
                strGPU = mo.Properties["VideoProcessor"].Value.ToString();
            }

            return strGPU;
        }

        //GPU Name
        protected string GetGPUName() //Gets the GPU name
        {
            string strGPUName = "";

            foreach (ManagementObject obj in mocGPU)
            {
                strGPUName = obj.Properties["Name"].Value.ToString();
            }

            return strGPUName;

        }

        //GPU Driver
        protected string GetGPUDriver() //Gets the driver installed
        {
            string strGPUDriver = "";

            foreach (ManagementObject obj in mocGPU)
            {
                strGPUDriver = obj.Properties["DriverVersion"].Value.ToString();
            }

            return strGPUDriver;

        }

        //Get Driver Date
        protected string GetDriverDate()
        {
            string driverDate = "";
            ManagementObjectSearcher gpuDriver = new ManagementObjectSearcher("select * from Win32_VideoController");

            foreach (ManagementObject obj in gpuDriver.Get())
            {
                driverDate = (obj.Properties["DriverDate"].Value.ToString());
            }

            int pos;
            pos = driverDate.IndexOf(".");
            driverDate = driverDate.Substring(0, pos);
            driverDate = driverDate.Remove(8, 6);

            return driverDate;
        }

        #region GPUInstances
        //Gets GPU Utilization
        private readonly PerformanceCounterCategory pcc = new PerformanceCounterCategory("GPU Engine");//Create a PerformanceCounterCategory
        //This will get the instances from the GPU engine on the windows computer providing the instance name ending.
        public string[] GetInstances(string instanceEnding)
        {
            //Create a list called instances
            List<string> instances = new List<string>();

            // Get the instances associated with this category and go through each string.
            foreach(string instance in pcc.GetInstanceNames())
            {
                //If the instance string ends with the provided instanceEnding in the method add it to instances list
                if (instance.Substring((instance.Length - instanceEnding.Length), instanceEnding.Length) == instanceEnding)
                {
                    instances.Add(instance);
                }
            }
            //Return the list as an array
            return instances.ToArray();
        }
        #endregion

        #endregion

    }//End SystemInfo Class

}//End Namespace
