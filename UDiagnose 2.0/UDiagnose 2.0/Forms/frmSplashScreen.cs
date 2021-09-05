//Author: Kenneth Lamb

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDiagnose_2._0.Classes;

namespace UDiagnose_2._0.Forms
{
    public partial class frmSplashScreen : Form
    {
        //Classes called here to load into the main form
        public frmMain mainForm = new frmMain();
        readonly Hardware hwInfo = new Hardware(); //Hardware info class called
        readonly DriveInfoClass drives = new DriveInfoClass();

        public frmSplashScreen()
        {
            InitializeComponent();
        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
            //Start the timer for the live data on the main form
            //properties
            mainForm.systemLoadTimer = new System.Timers.Timer(1000); //set a new timer at 1 second per tick
            mainForm.systemLoadTimer.Elapsed += mainForm.OnTimedEvent;//set it to the on timed event
            mainForm.systemLoadTimer.Enabled = true;//enable the timer
            
            //Load the Drives Information
            this.BeginInvoke((ThreadStart)delegate ()
            {
                lblLoading.Text = "Loading Drive Information";
            });
            //Call RefreshDrives to load the ComboBox
            drives.RefreshDrives(mainForm);
            //Call Load selected drive information
            drives.GetPhysicalDriveInformation(mainForm, mainForm.lstDrives.SelectedItem.ToString());
            Thread.Sleep(500);

            //Load up the HardwareInformation then load up the System Page information
            this.BeginInvoke((ThreadStart)delegate ()
            {
                lblLoading.Text = "Loading Hardware Information";
            });
            //Grab the TreeViewHardware queries
            hwInfo.LoadHardwareInfo(); //Search up the hardware on the computer
            //Load up the details for the individual pages on the form. Needs to have LoadHardwareInfo() called first!!!
            hwInfo.LoadUpPagesDetails(mainForm);
            //load the treeview with all of the information from LoadHardwareInfo()
            hwInfo.FillTreeView(mainForm);
            Thread.Sleep(500);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Dispose of the worker
            backgroundWorker1.Dispose();
            //Hide this form
            this.Hide();
            //Show the main form frmMain
            mainForm.Show();
        }

    }//End class
}//End namespace
