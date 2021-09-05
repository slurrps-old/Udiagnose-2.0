using System;
using MetroFramework.Forms;
using System.Windows.Forms;
using System.Diagnostics;
using UDiagnose_2._0.Classes;
using System.Timers;
using System.Threading;
using UDiagnose_2._0.Forms;
namespace UDiagnose_2._0
{
    public partial class frmMain : MetroForm
    {
        public frmMain()
        {
            InitializeComponent();
        }

        #region Public variables and constants for the program
        //Public Timer for the system to gather load and temperatures live on a separate thread
        //so that they do not iterfier with the UI controls
        public System.Timers.Timer systemLoadTimer;

        //Initialize the appropriate classes
        readonly PerformanceMeasurements performance = new PerformanceMeasurements();
        readonly DriveInfoClass drives = new DriveInfoClass();
        readonly SaveHWInfo saveHW = new SaveHWInfo();
        #endregion

        #region Form Load Details
        //Form Load
        private void frmMain_Load(object sender, EventArgs e)
        {
            //Load the menuStrip colors
            mainMenuStrip.LoadColors();

            //Set all default pages to the first page in each tabbed control
            ComputerHardwareNav.SelectedPage = ComputerHardwareNav.Pages[0];
            HardwareNav.SelectedPage = HardwareNav.Pages[0];
            SystemLoadNav.SelectedPage = SystemLoadNav.Pages[0];

            ////Setup some parameters to the controls
            //DrivePage
            rtbDriveInfo.SelectionIndent = 10;
            //performance.InitializeGPUCounters();//Initialize the GPU
        }
        #endregion

        #region Drive Information
        //Drive Selection Information
        private void lstDrives_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Load selected drive information
            drives.GetPhysicalDriveInformation(this, lstDrives.SelectedItem.ToString());

            //clear the chart
            chartDrive.Series["Drive"].Points.Clear();
            //Set new performance counter as per the drive selected
            //Update PerformanceMeasurements class PerformanceCounter sDrive to have new instance
            performance.sDrive = new PerformanceCounter("PhysicalDisk", "% Disk Time", drives.physicalDriveNumber + " " + lstDrives.SelectedItem.ToString());
            performance.sReadSpeed = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", drives.physicalDriveNumber + " " + lstDrives.SelectedItem.ToString());
            performance.sWriteSpeed = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", drives.physicalDriveNumber + " " + lstDrives.SelectedItem.ToString());

            if (rtbDriveInfo.Text == "")
            {
                drives.GetNetworkDriveInformation(this, lstDrives.SelectedItem.ToString());
            }
        }
        #endregion

        #region Timer Events
        //This event will load up the live data that will continue to read as long as the program is up and running. This is on a separate thread from the GUI

        //This is being called in the form load event which causes it to lag in starting by about 3 to 5 seconds will need to sort that out
        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Load the current performance 
            performance.LoadPerformance();
            
            //Important!!!------------------------------UI Updating----------------------------------------------------
            //Invoke the UI elements from a different thread!! Important or the UI will not work for the system if not invoked from a different thread
            //as the system timer is not on the main thread
            try
            {
                BeginInvoke((ThreadStart)delegate ()
                {
                    performance.LoadUI(this);
                });//End Invoke--------------------------------------------------------------------------------
            }
            catch
            {

            }
        }//End OnTimedEvent event
        #endregion

        #region Menu Strip Items

        #region File Strip
        //Save Button
        private void btnSaveInfo_Click(object sender, EventArgs e)
        {
            //Call the SaveInfo method from the SaveHWInfo class
            saveHW.SaveInfo(this);
        }
        //Exit Button
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exit application
            Application.Exit();
        }
        //X Button
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Tools Strip
        //Launches disk management
        private void diskPartitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //We can use system.diagnostics.process.start to launch programs on the computer. 
            //diskmgmt.msc is alwasy located on the same directory in windows
            //we will use this to launch to take care of formatting for the most part.
            Process diskPart = System.Diagnostics.Process.Start(@"C:\Windows\System32\diskmgmt.msc");

            diskPart.Close();
        }//End diskPartitionToolStripMenuItem_Click

        //Cmd launcher
        private void cMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //We can use system.diagnostics.process.start to launch programs on the computer. 
            //Opens the command prompt
            Process CMD = System.Diagnostics.Process.Start(@"C:\Windows\System32\cmd.exe");

            CMD.Close();
        }//End cMDToolStripMenuItem_Click

        //Disk Part launcher 
        private void diskPartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //We can use system.diagnostics.process.start to launch programs on the computer. 
            //Opens the disk partition command prompt
            Process CMDDisk = System.Diagnostics.Process.Start(@"C:\Windows\System32\diskpart.exe");

            CMDDisk.Close();
        }//End diskPartToolStripMenuItem_Click

        //Launches registry edit tool
        private void regEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show message alerting that altering anything in registry may harm their computer
            DialogResult result = MessageBox.Show("Warning editing the registry can harm your computer. " +
                "Are you sure you want to open this?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            switch (result)
            {
                case DialogResult.Yes:
                    //We can use system.diagnostics.process.start to launch programs on the computer. 
                    //Opens the registry editor
                    Process regEdit = System.Diagnostics.Process.Start(@"C:\WINDOWS\regedit.exe");

                    regEdit.Close();
                    break;

                case DialogResult.No:

                    break;
            }
        }//End regEditToolStripMenuItem_Click

        //This will launch the event viewer
        private void eventViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //We can use system.diagnostics.process.start to launch programs on the computer. 
            //Opens the event viewer
            Process eventViewer = System.Diagnostics.Process.Start(@"C:\Windows\System32\eventvwr.exe");

            eventViewer.Close();
        }//End eventViewerToolStripMenuItem_Click

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Closes the application
            Application.Exit();
        }//End btnExit_Click

        //Closed event
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Closes the application
            Application.Exit();
        }//End frmMain_FormClosed
        #endregion

        #region Help Strip
        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTOS TOS = new frmTOS();
            TOS.Show();
        }
        #endregion

        #region Context Menu
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            drives.RefreshDrives(this);
        }
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            systemLoadTimer.Stop();
        }

        private void resumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            systemLoadTimer.Start();
        }



        #endregion

        #endregion

        #region Events

        #endregion

    }//End class
}//End namespace
