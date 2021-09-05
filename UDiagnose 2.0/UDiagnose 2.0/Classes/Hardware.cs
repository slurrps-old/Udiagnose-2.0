//Author: Kenneth Lamb

namespace UDiagnose_2._0.Classes
{
    class Hardware : SystemInfo //Inherits from SystemInfo Class
    {
        //Author: Kenneth Lamb
        //Purpose:  This class queries the information from the systeminfo class so the main program can obtain the information.
        // Here we set pdms for all of the information that is to be queried and then pass those pdm's to the main program.
        // 
        // Assumptions: That the locations for the readings will be in the same place
        //
        // Exception Handling: None as all of the queries are properly made to always inquire the appropriate information from the OS
        //
        // This class includes 3 Methods

       
        //Default Constructor
        public Hardware()
        {

        }

        //Strucs for each individual hardware components
        #region Public and private PDM's for the main program to talk to
        //------------------------------------------------------------------------------------
        //Search PDM's to load up the data required for the FillTree method
        //to be searched on a separate thread than the UI
        //Here are the following structs needed for the programs hardware

        #region System
        public struct System
        {
            public static string OSInfo;
            public static string systemName;
            public static string type;
            public static string status;
        }
        #endregion

        #region Laptop Info
        public struct Laptop
        {
            public static string manufacturer;
            public static string serialNum;
        }
        #endregion

        #region CPU

        //Processor
        //Array to hold the CPU Cache levels
        private int[] Cache = new int[3];

        //Struct to hold the CPU Details
        public struct CPU
        {
            public static string processorInfo;
            public static string processorSubInfo;
            public static string processorID;
            public static string processorRevision;
            public static string numCores;
            public static string numThreads;
            public static string cpuMaxSpeed;
            public static string l1Cache;
            public static string l2Cache;
            public static string l3Cache;
        }
        #endregion

        #region Mobo
        //Mobo
        //Struct to hold the Mobo Details
        public struct MOBO
        {
            public static string manufacturerMobo;
            public static string baseboard;
            public static string serialNumber;
        }
        #endregion

        #region BIOS
        //Bios
        //Struct to hold the BIOS Details
        public struct BIOS
        {
            public static string biosMaker;
            public static string biosVersion;
            public static string biosCaption;
        }
        #endregion

        #region RAM
        //RAM
        //Struct to hold the RAM Details
        public struct RAM
        {
            public static string ramManufacturer;
            public static string ramInstalled;
            public static string ramSpeed;
            public static string slotsOnBoard;
            public static int numRAMSInstalled;
            public static string formFactor;
        }
        #endregion

        #region GPU
        //GPU
        //Struct to hold the GPU Details
        public struct GPU
        {
            public static string gpuName;
            public static string gpuDriver;
            public static string gpuInfo;
        }
        #endregion

        #endregion

        //Loads the system information on the hardware installed
        #region LoadHardwareInfo() Fill Information of the Struct's PDMs 
        //--------------------------------------------------------------------------------------------------------
        //This function will load up the data required for FillTree method
        public void LoadHardwareInfo()
        {
            //In this method we will be querying all of the data that we would like to 
            //show in the treeview and setting it to the appropriate variable
            #region System
            //System Enclosure
            System.systemName = GetName();
            System.type = GetSystemType();
            System.status = GetSystemStatus();
            System.OSInfo = (GetOSInformation());

            #region Laptop Info
            if (System.type != "Desktop")
            {
                Laptop.manufacturer = GetSystemManufacturer();
                Laptop.serialNum = GetSerialNum();
            }
            #endregion
            #endregion

            #region CPU
            //CPU info-----
            CPU.processorInfo = (GetProcessorInformation());
            CPU.processorSubInfo = (GetProcessorSubInfo());
            CPU.processorID = (GetProcessorId());
            CPU.processorRevision = (GetProcessorRevision().ToString());
            CPU.numCores = (GetNumberCores().ToString());
            CPU.numThreads = (GetNumberThreads().ToString());
            CPU.cpuMaxSpeed = (GetCpuMaxSpeedInGHz().ToString("0.00") + " GHz");

            //Cache Sizes---
            //Load up the data to the appropriate variables
            Cache = GetCache();//Fill the array
            //Set the levels of cache to the appropriate array levels
            //level1
            if (Cache[0] > 1000)
            {
                CPU.l1Cache = (((float)(Cache[0] / 1000)).ToString("0.0") + " MB");
            }
            else
            {
                CPU.l1Cache = (Cache[0].ToString("0.0") + " KB");
            }
            //level 2
            if(Cache[1] > 1000)
            {
                CPU.l2Cache = (((float)(Cache[1] / 1000)).ToString("0.0") + " MB");
            }
            else
            {
                CPU.l2Cache = (Cache[1].ToString("0.0") + " KB");
            }
            //Level 3
            if (Cache[2] > 1000)
            {
                CPU.l3Cache = (((float)(Cache[2] / 1000)).ToString("0.0") + " MB");
            }
            else
            {
                CPU.l3Cache = (Cache[2].ToString("0.0") + " MB");
            }//End Cache sizes---
            #endregion

            #region Mobo
            //Mobo info-----
            MOBO.manufacturerMobo = GetBoardMaker();
            MOBO.baseboard = GetBoardProductId();
            MOBO.serialNumber = GetBoardSerialNumber();
            #endregion

            #region BIOS
            //BIOS info-----
            BIOS.biosMaker = GetBIOSmaker();
            BIOS.biosVersion = GetBIOSVersion();
            BIOS.biosCaption = GetBIOScaption();
            #endregion

            #region RAM
            //RAM info-----
            RAM.ramManufacturer = GetRAMManufact(); //RAM Maker
            RAM.ramInstalled = (GetPhysicalMemory()/ 1024).ToString(); //Amount installed
            RAM.ramSpeed = GetRAMSpeed().ToString(); //Clock speed
            RAM.slotsOnBoard = GetNoRamSlots(); //Dimm slots on board
            RAM.numRAMSInstalled = GetRAMInstalled();
            RAM.formFactor = GetFormFactor();
            #endregion

            #region GPU
            //GPU info-----
            GPU.gpuName = GetGPUName();
            GPU.gpuDriver = GetGPUDriver();
            GPU.gpuInfo = GetGPUInformation();
            #endregion
        }//End TreeView
         //------------------------------------------------------------------------------------------------
        #endregion

        //Used to fill the Hardware TreeView
        #region FillTreeView()
        //This method will take all of the PDM's of the application and load them into the treeview from the main form
        //Needs to have the main form initialized to access the elements
        public void FillTreeView(frmMain mainForm)
        {
            #region System
            //OS Info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[1].Nodes.Add(System.OSInfo);
            //System Enclosure
            mainForm.treeHardwareInfo.Nodes[0].Nodes[0].Nodes.Add(System.systemName);
            mainForm.treeHardwareInfo.Nodes[0].Nodes[0].Nodes.Add("Computer Type: " + System.type);
            mainForm.treeHardwareInfo.Nodes[0].Nodes[0].Nodes.Add("Status: " + System.status);

            #region Laptop
            if (System.type != "Desktop")
            {
                mainForm.treeHardwareInfo.Nodes[0].Nodes[0].Nodes.Add("Manufacturer: " + Laptop.manufacturer);
                mainForm.treeHardwareInfo.Nodes[0].Nodes[0].Nodes.Add("Serial " + Laptop.serialNum);
            }
            #endregion
            #endregion

            #region CPU
            //CPU info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add(CPU.processorInfo); //Processor information like the name
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes[0].Nodes.Add(CPU.processorSubInfo); //Sub info 
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("Processor ID: " + CPU.processorID);// the ID of the processor
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("Processor Revision: " + CPU.processorRevision);// the revision of the processor
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("Number of Cores: " + CPU.numCores); //Amount of cores
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("Number of Threads: " + CPU.numThreads); //Number of threads
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("Max Speed: " + CPU.cpuMaxSpeed); //Current clock speed of the CPU
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("L1 Cache: " + CPU.l1Cache); //Size of the l1 Cache
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("L2 Cache: " + CPU.l2Cache); //Size of the l2 Cache
            mainForm.treeHardwareInfo.Nodes[0].Nodes[2].Nodes.Add("L3 Cache: " + CPU.l3Cache); //Size of the l3 Cache
            #endregion

            #region Mobo
            //Mobo info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[3].Nodes.Add("Manufacturer: " + MOBO.manufacturerMobo); //Mobo manufacturer
            mainForm.treeHardwareInfo.Nodes[0].Nodes[3].Nodes.Add("Baseboard Product: " + MOBO.baseboard); //baseboard info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[3].Nodes.Add("Serial Number: " + MOBO.serialNumber); //serial number info
            #endregion

            #region BIOS
            //BIOS info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[4].Nodes.Add("Maker: " + BIOS.biosMaker);//Bios maker
            mainForm.treeHardwareInfo.Nodes[0].Nodes[4].Nodes.Add("Version: " + BIOS.biosVersion);//Serial of the bios
            mainForm.treeHardwareInfo.Nodes[0].Nodes[4].Nodes.Add("Caption: " + BIOS.biosCaption);//Bios caption
            #endregion

            #region RAM
            //RAM info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[5].Nodes.Add("Manufacturer: " + RAM.ramManufacturer); //RAM Maker
            mainForm.treeHardwareInfo.Nodes[0].Nodes[5].Nodes.Add("Form Factor: " + RAM.formFactor); //RAM form factor
            mainForm.treeHardwareInfo.Nodes[0].Nodes[5].Nodes.Add("RAM Installed: " + RAM.ramInstalled + " GB"); //Amount installed
            mainForm.treeHardwareInfo.Nodes[0].Nodes[5].Nodes.Add("Speed: " + RAM.ramSpeed + " GHz"); //Clock speed
            mainForm.treeHardwareInfo.Nodes[0].Nodes[5].Nodes.Add("Dimm slots on board: " + RAM.slotsOnBoard); //Dimm slots on board
            mainForm.treeHardwareInfo.Nodes[0].Nodes[5].Nodes.Add("Dimm slots used " + RAM.numRAMSInstalled.ToString() + " Out of " + RAM.slotsOnBoard);
            #endregion

            #region GPU
            //GPU info
            mainForm.treeHardwareInfo.Nodes[0].Nodes[6].Nodes.Add(GPU.gpuName);//Name of the GPU
            mainForm.treeHardwareInfo.Nodes[0].Nodes[6].Nodes.Add("Driver version: " + GPU.gpuDriver);//Driver information
            mainForm.treeHardwareInfo.Nodes[0].Nodes[6].Nodes.Add(GPU.gpuInfo);//More information on the GPU
            #endregion

            //Open all nodes
            mainForm.treeHardwareInfo.ExpandAll(); //Expand the treeview 
        }
        #endregion

        //Loads the SystemLoads pages 
        #region LoadUpPagesDetails()
        public void LoadUpPagesDetails(frmMain main)
        {
            #region Overview
            //Overview Page
            main.lblPGOverviewCPUName.Text = "CPU - " + CPU.processorInfo;//Grab the processor name
            main.lblPGOverviewMEMName.Text = "RAM - " + RAM.ramManufacturer + " " + (GetPhysicalMemory() / 1024).ToString() + " GB";//Grab the ram amount and manufacturer
            main.lblPGOverviewGPUName.Text = "GPU - " + GPU.gpuName;//Grab the gpu name
            #endregion

            #region CPU
            //CPU Page Details
            main.lblPGCPUName.Text = CPU.processorInfo; //Processor name
            main.lblMaxCPUSpeed.Text = CPU.cpuMaxSpeed.ToString(); //Max Processor speed
            main.lblCPUCores.Text = CPU.numCores.ToString(); //Number of cores on processor
            main.lblCPUThreads.Text = CPU.numThreads.ToString(); //Number of threads on processor
            main.lblL1Cahce.Text = CPU.l1Cache.ToString(); //L1 Cache size
            main.lblL2Cache.Text = CPU.l2Cache.ToString(); //L2 Cache size
            main.lblL3Cache.Text = CPU.l3Cache.ToString(); //L3 Cache size
            #endregion

            #region Memory
            //Memory Page Details
            main.lblPGMemName.Text = RAM.ramManufacturer + " " + (GetPhysicalMemory() / 1024).ToString() + " GB";//Grab the ram amount and manufacturer
            #endregion

            #region GPU
            //GPU Page Details
            main.lblPGGPUName.Text = GPU.gpuName;//Grab the gpu name
            #endregion

            #region Components
            //Hardware Components page

            #endregion
        }
        #endregion

    }//End Class

}//End Namespace
