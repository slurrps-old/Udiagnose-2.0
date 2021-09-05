/*Author: Kenneth Lamb
 * 
 * The purpose of this class is to load the performance metrics for the program using performance counters as well as load
 * information about the system on the appropriate labels.
 * 
 * This class includes 8 Methods
 * ------------------------------
 * InitializeGPUCounters()
 * GPUData()
 * GPU3D()
 * GPUCopy()
 * GPUVideo()
 * GPUCompute()
 * LoadPerformance()
 * LoadUI(frmMain main)
 */

using System;
using System.Diagnostics;

namespace UDiagnose_2._0.Classes
{
    class PerformanceMeasurements: SystemInfo //Inherits from SystemInfo Class
    {
        //Create Variables to hold the Performance counters values.
        //Default them all to 0
        #region Variables for Data Performance --set all to 0

        #region Processor
        //CPU
        private float fCPULoad = 0;//Holds CPU load percentage
        private int intThreads = 0;//Holds the amount of threads
        private int intHandles = 0;//Holds the amount of handles
        private float cpuClockSpeed = 0;//Holds the current clock speed of the CPU

        #endregion

        #region Memory
        //Memory
        private float fMemLoad = 0.0f;//Holds the memory load percentage

        #endregion

        #region GPU
        //GPU
        private float f3DGPU = 0.0f;//Holds 3D Instances
        private float fCopyGPU = 0.0f;//Holds Copy Instances
        private float fCompute0GPU = 0.0f;//Holds Compute_0
        private float fVDecodeGPU = 0.0f;//Holds Video Decode

        #endregion

        #region Drives
        //Drives
        private float fDrive = 0.0f;//Holds the total drive load percentage
        private float fSDrive = 0.0f;//Selected Drive
        private float fSReadSpeed = 0.0f;//Holds the read speed of the selected drive
        private float fSWriteSpeed = 0.0f;//Holds the write speed of the selected drive

        #endregion

        #region System Time
        //System Time
        private string time;//Holds the system time

        #endregion

        #endregion

        //Houses all of the performance counters of the program
        #region Performance Counter Variables

        #region Processor
        //-------------------------------------Performance Counters----------------------------------------
        //Processor------------------------
        public PerformanceCounter pCPULoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");//%Processing time total of all cores
        public PerformanceCounter pCPUFrequency = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total");//Processor Frequency _Total of all cores
        //May add more to get frequency of all cores!? not sure yet
        public PerformanceCounter pThreads = new PerformanceCounter("Process", "Thread Count", "_Total");//Processor thread count
        public PerformanceCounter pHandles = new PerformanceCounter("Process", "Handle Count", "_Total");//Processor handle count

        #endregion

        #region Memory
        //Memory---------------------------------
        public PerformanceCounter pMemoryLoad = new PerformanceCounter("Memory", "% Committed Bytes In Use");//%Committed Bytes In Use

        //Physical Disk---------------------------
        public PerformanceCounter pDrive = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total"); //Drive utilization
        public PerformanceCounter sDrive = new PerformanceCounter("PhysicalDisk", "% Disk Time", "0 C:"); //Drive % Disk Time for selected drive --Set to be default Disk 0 Name C: OS Drive
        public PerformanceCounter sReadSpeed = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "0 C:");//Drive Disk Read Bytes/sec --Set to be default Disk 0 Name C: OS Drive
        public PerformanceCounter sWriteSpeed = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "0 C:");//Drive Disk Write Bytes/sec --Set to be default Disk 0 Name C: OS Drive
                                                                                                                       //Further detail will need to be shown here...

        #endregion

        #region GPU
        //GPU---------------------------------------
        //In work as this is a bit tricky to get. Not sure if this is 100% accurate
        //Variable for performance counters
        public string[] GPUInstances;//Temp to hold GPU instances below
        public PerformanceCounter[] p3DGPU;//Holds GPU 3D Instances as an array ****Issue Reporting Correctly!!!
        public PerformanceCounter[] pCopyGPU;//Holds GPU Copy Instances as an array
        public PerformanceCounter[] pVDecodeGPU;//Holds GPU VideoDecode Instances as an array
        public PerformanceCounter[] pCompute0GPU;//Holds GPU Compute_0 Instances as an array

        #endregion

        #region System Time
        //System Time----------------------------------
        public PerformanceCounter pUpTime = new PerformanceCounter("System", "System Up Time"); //System up time
        #endregion

        #endregion

        //Used to gather GPU performance
        #region Load the GPU Performance Counters

        #region Initialize Counters
        //Initializes all of the GPU counters by calling GetInstances from SystemInfo class
        public void InitializeGPUCounters()
        {
            //Pre: Does not need anything to initialize
            //Purpose: To load GPU Instances into a performance counter array
            //Post: Loads up the instances from sysInfo.GetInstances() into an 
            //array of PerformanceCounters called pGPU

            //------------------GPU-------------------------
            //Get the instances from the System Info for GPU selected instances
            GPU3D();
            GPUCopy();
            GPUVideo();
            GPUCompute();
        }//End InitializeGPUCounters
        #endregion

        #region GPU Data Retrieval
        //Gets the data from all of the performance counters
        public void GPUData()
        {
            //Pre: requires no variable initialization
            //Post: Adds all of each performance counter array and sets them to the
            //appropriate variable.
            //Purpose: To reset the float variables then using for loops go through each
            //GPU performance counter array and += them to the appropriate variables

            //Reset the float variables
            f3DGPU = 0.0f;//Set fGPU to 0 on every call to get the correct info
            fCopyGPU = 0.0f;
            fCompute0GPU = 0.0f;
            fVDecodeGPU = 0.0f;

            //Add all of the instances together to get utilization using for loops
            for (int i = 0; i < p3DGPU.Length; i++)
            {
                f3DGPU += p3DGPU[i].NextValue();
            }
            for (int i = 0; i < pCopyGPU.Length; i++)
            {
                fCopyGPU += pCopyGPU[i].NextValue();
            }
            for (int i = 0; i < pCompute0GPU.Length; i++)
            {
                fCompute0GPU += pCompute0GPU[i].NextValue();
            }
            for (int i = 0; i < pVDecodeGPU.Length; i++)
            {
                fVDecodeGPU += pVDecodeGPU[i].NextValue();
            }
        }
        #endregion 

        #region GPU Instance Queries
        //Gathers 3D instances returning an array of performance counters
        private PerformanceCounter[] GPU3D()
        {
            GPUInstances = GetInstances("_phys_0_eng_0_engtype_3D");
            //Set the array of performance counters to equal the length of the instances
            p3DGPU = new PerformanceCounter[GPUInstances.Length];
            //Create an array of performance counters
            for (int i = 0; i < p3DGPU.Length; i++)
            {
                p3DGPU[i] = new PerformanceCounter("GPU Engine", "Utilization Percentage", GPUInstances[i]);
            }
            return p3DGPU;
        }

        //Gathers Copy instances returning an array of performance counters
        private PerformanceCounter[] GPUCopy()
        {
            GPUInstances = GetInstances("_engtype_Copy");
            //Set the array of performance counters to equal the length of the instances
            pCopyGPU = new PerformanceCounter[GPUInstances.Length];
            //Create an array of performance counters
            for (int i = 0; i < pCopyGPU.Length; i++)
            {
                pCopyGPU[i] = new PerformanceCounter("GPU Engine", "Utilization Percentage", GPUInstances[i]);
            }
            return pCopyGPU;
        }

        //Gathers the VideoDecode instances returning an array of performance counters
        private PerformanceCounter[] GPUVideo()
        {
            GPUInstances = GetInstances("_engtype_VideoDecode");
            //Set the array of performance counters to equal the length of the instances
            pVDecodeGPU = new PerformanceCounter[GPUInstances.Length];
            //Create an array of performance counters
            for (int i = 0; i < pVDecodeGPU.Length; i++)
            {
                pVDecodeGPU[i] = new PerformanceCounter("GPU Engine", "Utilization Percentage", GPUInstances[i]);
            }
            return pVDecodeGPU;
        }

        //Gathers Compute_0 instances returning an array of performance counters
        private PerformanceCounter[] GPUCompute()
        {
            GPUInstances = GetInstances("_engtype_Compute_0");
            //Set the array of performance counters to equal the length of the instances
            pCompute0GPU = new PerformanceCounter[GPUInstances.Length];
            //Create an array of performance counters
            for (int i = 0; i < pCompute0GPU.Length; i++)
            {
                pCompute0GPU[i] = new PerformanceCounter("GPU Engine", "Utilization Percentage", GPUInstances[i]);
            }
            return pCompute0GPU;
        }
        #endregion

        #endregion 

        //Load up the Variables with the performance counter values
        #region Load Up Variables
        //Loads up all of the Load data for the program per second Tick
        public void LoadPerformance()
        {
            //Pre: requires no variables to be initialized
            //Post: Loads all of the variables with the performance counters values in the class
            //Purpose: To load all of the variables with the performance counters next value

            //Gets System time and formats it into a string
            #region System Time
            //Sysyem up time-----
            TimeSpan ts = TimeSpan.FromSeconds(pUpTime.NextValue());//Holds the performance counter for the system up time
            time = string.Format("{0}d:{1}h:{2}m:{3}s", ts.Days, ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));//Format the system up time
            #endregion

            #region Loading Variables with Performance Counter Values
            //--------------------------------Performance Counters--------------------------------------//
            //Try catch to avoid the program from breaking due to exception errors.
            try
            {
                #region Processor
                //Holds the performance counters for CPU, RAM, GPU, and Drive load percentage on the system
                //CPU-----Load the CPU variables
                fCPULoad = pCPULoad.NextValue();//get the next %Processor Time value of the counter
                intThreads = Convert.ToInt32(pThreads.NextValue());//get the next Thread Count value of the counter
                intHandles = Convert.ToInt32(pHandles.NextValue());//get the next Handle Count value of the counter
                cpuClockSpeed = (pCPUFrequency.NextValue()/1000);//Get the next CPU Frequency value of the counter

                #endregion

                #region Memory
                //Memory-----Load the Memory variables
                fMemLoad = pMemoryLoad.NextValue();//get the next % Committed Bytes In Use value of the counter

                #endregion

                #region GPU
                //GPU-----Load the GPU variables
                //Check to make sure the array GPU performance counters are not empty or null
                if (GPUInstances == null)
                {
                    InitializeGPUCounters();//Instantiate the GPU instances
                }
                //Check to make sure the GPU performance counters are not null!!! or exception will throw
                if (p3DGPU != null && pCompute0GPU != null && pVDecodeGPU != null && pCopyGPU != null)
                {
                    GPUData();//Get next values for the GPU instances
                }

                #endregion

                #region Drives
                //Physical Disk-----Load the Disk variables
                fDrive = pDrive.NextValue();//get the next % Disk Time _Total value of the counter
                fSDrive = sDrive.NextValue();//Get the next % Disk Time of Selected Drive value of the counter
                fSReadSpeed = sReadSpeed.NextValue();//Get the next Disk Read Speed in Bytes/Sec value of the counter
                fSWriteSpeed = sWriteSpeed.NextValue();//Get the next Disk Write Speed in Bytes/Sec value of the counter

                #endregion

            }
            catch
            {
                //TODO
                //Need to put a string array here that will add all errors to a txt file on closing of the program or during ticks
                //not sure which to choose yet
                //Throw error
            }
            #endregion
        }
        #endregion 

        //Load the UI
        #region LoadUI()
        //Loads the UI of the main form with the data from the LoadPerformance section
        public void LoadUI(frmMain main)
        {
            //Pre: Requires frmMain to be initialized
            //Post: Loads up the frmMain labels with the appropriate information
            //Purpose: To load the labels on the main form with the variables values taken from
            //the hardwares performance counters values

            //Drive Utilization Reporting
            #region Drive Page
            //---------------------------Drive Info Page-----------------------------------
            //-----Chart
            main.chartDrive.Series["Drive"].Points.AddY(fSDrive);//add the points to the fRAM variable per tic

            //-----Labels
            main.lblPGDriveUtil.Text = string.Format("{0:0}%", fSDrive);//displays drive utilization
            //Check to format the Read speed as MB or KB depending on the amount shown
            //ReadSpeed
            if (fSReadSpeed > 1000000)//Check that the Bytes are not greater than 1000000
            {
                //Format to MB by dividing by 1024 then taking that total and dividing it again by 1024
                main.lblDiskReadSpeed.Text = string.Format("{0:0.0}", ((fSReadSpeed / 1024) / 1024)) + " MB/s";
            }
            else
            {
                //Format to KB by dividing by 1024
                main.lblDiskReadSpeed.Text = string.Format("{0:0.0}", (fSReadSpeed / 1024)) + " KB/s";
            }
            //Check to format the Write speed as MB or KB depending on the amount shown
            //WriteSpeed
            if (fSWriteSpeed > 1000000)//Check that the Bytes are not greater than 1000000
            {
                //Format to MB by dividing by 1024 then taking that total and dividing it again by 1024
                main.lblDiskWriteSpeed.Text = string.Format("{0:0.0}", ((fSWriteSpeed / 1024) / 1024)) + " MB/s";
            }
            else
            {
                //Format to KB by dividing by 1024
                main.lblDiskWriteSpeed.Text = string.Format("{0:0.0}", (fSWriteSpeed / 1024)) + " KB/s";
            }
            #endregion 

            //Shows the overview of the CPU, Memory, GPU, and Drive Total% usage in the system
            #region Overview Page
            //--------------------------------OVERVIEW PAGE----------------------------------
            //Utilizations
            //----------------------------Chart Overview Load
            main.chartOVERVIEW.Series["CPU"].Points.AddY(fCPULoad);//add the points to the fCPU variable per tic
            main.chartOVERVIEW.Series["RAM"].Points.AddY(fMemLoad);//add the points to the fRAM variable per tic
            main.chartOVERVIEW.Series["Drive"].Points.AddY(fDrive);//add the points to the fDrive variable per tic
            main.chartOVERVIEW.Series["GPU"].Points.AddY(f3DGPU);

            //----------------------------------Labels-------------------------------------------
            //CPU On Overview page
            main.lblCPUUtilization.Text = string.Format("{0:0}%", fCPULoad);//displays cpu utilization

            //Memory
            main.lblRAMUtilization.Text = string.Format("{0:0}%", fMemLoad);//displays ram utilization

            //Physical Disk
            main.lblDriveUtilization.Text = string.Format("{0:0}%", fDrive);//displays drive utilization

            //GPU
            main.lblGPUUtilization.Text = string.Format("{0:0}%", f3DGPU);

            //System Up Time
            main.lblSystemUpTime.Text = time;//set the label to the time variable
            #endregion

            //CPU Utilization Reporting
            #region CPU Page
            //---------------------------CPU PAGE-----------------------------------
            //CPU Information
            //-----Chart
            main.chartCPU.Series["CPU"].Points.AddY(fCPULoad);//chartCPU Utilization
            //-----Labels
            main.lblCPUPageUtil.Text = string.Format("{0:0}%", fCPULoad);//displays cpu utilization
            main.lblFrequency.Text = cpuClockSpeed.ToString("0.00") + " GHz";//Get the current frequency of the cpu
            main.lblHandles.Text = (intHandles.ToString());//Get the amount of handles on the cpu
            main.lblThreads.Text = (intThreads.ToString());//Get the amount of threads on the cpu
            
            #endregion

            //Memory Utilization Reporting
            #region Memory Page
            //---------------------------Memory PAGE-----------------------------------
            //-----Chart
            main.chartRAM.Series["RAM"].Points.AddY(fMemLoad);//add the points to the fRAM variable per tic
            //-----Labels
            main.lblMemoryPGUtilization.Text = string.Format("{0:0}%", fMemLoad);
            #endregion

            //GPU Utilization Reporting
            #region GPU Page
            //---------------------------GPU PAGE-----------------------------------
            //-----Chart
            main.chart3D.Series["GPU"].Points.AddY(f3DGPU);
            main.chartCopy.Series["GPU"].Points.AddY(fCopyGPU);
            main.chartVideoDecode.Series["GPU"].Points.AddY(fVDecodeGPU);
            main.chartCompute0.Series["GPU"].Points.AddY(fCompute0GPU);
            #endregion

            //Not available yet-----Future release
            #region Temp Page
            //---------------------------Temp PAGE-----------------------------------
            //progCPUTemp.Value = Convert.ToInt32(CPUTemperature.GetCPUTemp());
            #endregion
        }
        #endregion

    }//End class
}//End namespace
