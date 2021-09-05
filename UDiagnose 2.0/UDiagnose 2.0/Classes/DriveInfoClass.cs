/*Author: Kenneth Lamb
 * 
 * This class is to gather information on the drives connected to the computer
 * 
 * This class includes 7 Methods
 * ----------------------------------------
 * ConversionToGig(float conversionNum)
 * ConversionToTer(float conversionNum)
 * RefreshDrives(frmMain main)
 * GetNetworkDriveInformation(frmMain main, string driveLetter)
 * ISOSDrive(string driveLetter)
 * 
 * Not working Yet
 * DriveType()
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO; //Used to list the drives
using System.Management;

namespace UDiagnose_2._0.Classes
{
   class DriveInfoClass
    {
        #region Variables
        //Constants for conveersions of different byt sizes
        const float FLOAT_GIG_CONVERSION = 1073741824f; //Holds the float conversion number of GB per bit
        const float FLOAT_TERA_CONVERSION = 0.0009765625F;//Holds the float conversion number for TB per bit
        //-------------------------------------------------------------------------------------------------
        //Global Variables to use DriveInfo and variable to hold all of the systems rive information.
        private string driveInfo; //This is the variable that will hold all of the drive information
        private bool isOSDrive; //This will hold the boolean whether the drive is the OS drive or not.
        private float fltPercent = 0; //Hold the percent of the drive that is filled.
        //-------------------------------------------------------------------------------------------------
        //Variables for the Drive
        //LogicalDisk
        string driveName;
        bool driveCompressed;
        uint driveType;
        string fileSystem;
        //Calculate Space
        float freeSpace;
        float totalSpace;
        float usedSpace;
        uint driveMediaType;
        string volumeName;
        string volumeSerial;
        //DiiskDrive
        string driveStatus;
        string physicalName;
        public int physicalDriveNumber;
        string diskManufacturer;
        string diskModel;
        string diskInterface;
        bool mediaLoaded;
        string mediaType;
        uint mediaSignature;
        string mediaStatus;
        //------------------------------------------------------------------------------------------------
        #endregion

        public DriveInfoClass()
        {

        }

        #region Functions
        #region Conversion Functions
        //Convert bytes to Gigabytes used to display correct drive information
        public float ConversionToGig(float conversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Pose: Returns gigConversion number to the program
            //Purpose: To convert the bytes number that is incoming to gigabytes

            //Set the gigConversion to 0
            float gigConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_GIG_CONVERSION Constant
            gigConversion = conversionNum / FLOAT_GIG_CONVERSION;

            return gigConversion; //Returns the variable gigConversion
        }//End ConversionToGig

        //Convert bytes to TeraBytes used to display correct drive information
        public float ConversionToTer(float ConversionNum)
        {
            //Pre: Needs conversionNum to be initialized
            //Pose: Returns teraConversion number to the program
            //Purpose: To convert the bytes number that is incoming to terabytes

            //Set the teraConversion to 0
            float teraConversion;
            //Grabs the conversionNum from the one passed into the function then 
            //divides by the Float_TERA_CONVERSION Constant
            teraConversion = ConversionNum / FLOAT_TERA_CONVERSION;

            return teraConversion;
        }//End ConversionToTer
        #endregion

        #endregion

        //Gets a list of drives attached to the computer and adds them to the ComboBox
        #region Refresh Drive list
        public void RefreshDrives(frmMain main)
        {
            //Refresh the list and clear it out
            main.lstDrives.Items.Clear();

            var logicalDriveQuery = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
            foreach (ManagementObject ld in logicalDriveQuery.Get())
            {
                //Fill out the combo box with all of the drives attached to the computer
                //for each drives detected get the Name from the Win32_LogicalDisk class
                main.lstDrives.Items.Add((ld.Properties["Name"].Value).ToString());
            }

            //Set the selected item to the first item
            main.lstDrives.SelectedIndex = 0;
        }
        #endregion

        #region Load Drive Info
        //Loads Network Drive Information attached to the network and display them to the user
        public void GetNetworkDriveInformation(frmMain main, string driveLetter)
        {
            //Set driveInfo to nothing
            driveInfo = "";
            //Set fltPercent to 0
            fltPercent = 0;

            //Set defaults for progress bar and label to nothing
            main.lblDrivePGPercentage.Text = "";
            main.progHardDrive.Maximum = 0;
            main.progHardDrive.Value = 0;

            //Make sure drive letter is formatted correctly as ex: "C:"
            driveLetter = driveLetter.Substring(0, 2);

            driveInfo += "Non Physical Drive" + Environment.NewLine;
            //Need to query logical drives separately in case of network drives
            var logicalDriveQuery = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
            //loop to search through management objects in the Object Searcher
            foreach (ManagementObject ld in logicalDriveQuery.Get())
            {
                //Get information from the Logical Disk
                //LogicalDisk
                driveName = Convert.ToString(ld.Properties["Name"].Value); // C:
                driveCompressed = Convert.ToBoolean(ld.Properties["Compressed"].Value);
                driveType = Convert.ToUInt32(ld.Properties["DriveType"].Value); // C: - 3
                fileSystem = Convert.ToString(ld.Properties["FileSystem"].Value); // NTFS
                freeSpace = ConversionToGig(Convert.ToUInt64(ld.Properties["FreeSpace"].Value)); //converted to gigs
                totalSpace = ConversionToGig(Convert.ToUInt64(ld.Properties["Size"].Value)); //converted to gigs
                usedSpace = totalSpace - freeSpace; //Calculate usedSpace
                driveMediaType = Convert.ToUInt32(ld.Properties["MediaType"].Value); // c: 12
                volumeName = Convert.ToString(ld.Properties["VolumeName"].Value); // System
                volumeSerial = Convert.ToString(ld.Properties["VolumeSerialNumber"].Value); // 12345678

                //Check to be sure the drive letter is equal to the driveName
                if (driveName == driveLetter)
                {
                    //Check for if the drive has an OS
                    isOSDrive = ISOSDrive(driveLetter);
                    if (isOSDrive == true)
                    {
                        driveInfo += "OS Drive: True" + Environment.NewLine;
                    }
                    else
                    {
                        driveInfo += "OS Drive: False" + Environment.NewLine;
                    }

                    //Logical Disk
                    driveInfo += "DriveName: " + driveName + Environment.NewLine;
                    driveInfo += "VolumeName: " + volumeName + Environment.NewLine;
                    driveInfo += "DriveCompressed: " + driveCompressed + Environment.NewLine;
                    //Get Device Type
                    switch(driveType)
                    {
                        case 0:
                            driveInfo += "DriveType: Unknown" + Environment.NewLine;
                            break;

                        case 1:
                            driveInfo += "DriveType: No Root Directory" + Environment.NewLine;
                            break;

                        case 2:
                            driveInfo += "DriveType: Removable Disk" + Environment.NewLine;
                            break;

                        case 3:
                            driveInfo += "DriveType: Local Disk" + Environment.NewLine;
                            break;

                        case 4:
                            driveInfo += "DriveType: Network Drive" + Environment.NewLine;
                            break;

                        case 5:
                            driveInfo += "DriveType: Compact Disk" + Environment.NewLine;
                            break;

                        case 6:
                            driveInfo += "DriveType: RAM Disk" + Environment.NewLine;
                            break;
                    }
                    driveInfo += "FileSystem: " + fileSystem + Environment.NewLine;
                    driveInfo += "Used Space: " + usedSpace.ToString("0.00") + " GB" + Environment.NewLine;
                    driveInfo += "Free Space: " + freeSpace.ToString("0.00") + " GB" + Environment.NewLine;
                    driveInfo += "Total Space: " + totalSpace.ToString("0.00") + " GB" + Environment.NewLine;
                    driveInfo += "DriveMediaType: " + driveMediaType + Environment.NewLine;
                    driveInfo += "VolumeSerial: " + volumeSerial + Environment.NewLine;
                    driveInfo += Environment.NewLine;

                    //Get Percent used
                    fltPercent = ((totalSpace - freeSpace) / totalSpace) * 100.0f;
                    //Input to user how much of the drive they have used
                    //This will show the percentage of the drive being used
                    main.lblDrivePGPercentage.Text = "You have used " + fltPercent.ToString("0.00") + "% of your drive";

                    //Progress bar This needs to be inside the if statement as they need to be initialized by the drive being ready
                    //Get drive max value
                    //This will set the maximun space on the drive to the progress bar
                    main.progHardDrive.Maximum = Convert.ToInt32(totalSpace);
                    //CurrentValue of drive usage
                    //This will get how much is used on the drive and set the value to the progress bar
                    main.progHardDrive.Value = Convert.ToInt32(totalSpace - freeSpace);

                    //Change color of progress bar depending on how much of drive is used
                    if (fltPercent >= 80.0f)
                    {
                        main.lblDrivePGPercentage.ForeColor = Color.Red; //Sets the label color to Crimson
                        main.progHardDrive.Style = MetroFramework.MetroColorStyle.Red;
                    }
                    else if (fltPercent >= 70.0f)
                    {
                        main.lblDrivePGPercentage.ForeColor = Color.Yellow; //Sets the label color to Crimson
                        main.progHardDrive.Style = MetroFramework.MetroColorStyle.Yellow;
                    }
                    else
                    {
                        main.lblDrivePGPercentage.ForeColor = Color.White; //Sets the label color to White
                        main.progHardDrive.Style = MetroFramework.MetroColorStyle.Blue;
                    }

                }
                else
                {
                    //Do nothing
                }
                //Set the driveInfo to the rtb text box rtbDriveInfo
                main.rtbDriveInfo.Text = driveInfo;
                main.rtbDriveInfo.Refresh();
            }
        }

        //Used to gather information on drives Physically attached to the computer and display them to the user
        public void GetPhysicalDriveInformation(frmMain main, string driveLetter)
        {
            //Set driveInfo to nothing
            driveInfo = "";
            //Set fltPercent to 0
            fltPercent = 0;

            //Set defaults to nothing
            main.lblDrivePGPercentage.Text = "";
            main.progHardDrive.Maximum = 0;
            main.progHardDrive.Value = 0;

            //Make sure drive letter is formatted as ex: "C:"
            driveLetter = driveLetter.Substring(0, 2);

            //New driveQuery to new ManagementObjectSearcher("Select * from Win32_DiskDrive")
            var driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            //Foreach loop to go through the managementObjects in driveQuery
            foreach (ManagementObject d in driveQuery.Get())
            {
                //New Query to associate Win32_DiskDrive to DiskPartition
                var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
                //Create a new managementObjectSearcher giving it the query
                var partitionQuery = new ManagementObjectSearcher(partitionQueryText);
                //Loop to Search the ManagementObjects in the query
                foreach (ManagementObject p in partitionQuery.Get())
                {
                    //New Query to associate Win32_LogicalDisk to Partition
                    var logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                    //Create a new managementObjectSearcher giving it the query
                    var logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
                    //Loop to Search the ManagementObjects in the query
                    foreach (ManagementObject ld in logicalDriveQuery.Get())
                    {
                        //Get the information from the LogicalDisk and DiskDrive classes on Windows
                        //LogicalDisk
                        driveName = Convert.ToString(ld.Properties["Name"].Value); // C:
                        driveCompressed = Convert.ToBoolean(ld.Properties["Compressed"].Value);
                        driveType = Convert.ToUInt32(ld.Properties["DriveType"].Value); // C: - 3
                        fileSystem = Convert.ToString(ld.Properties["FileSystem"].Value); // NTFS
                        //Calculate Space
                        freeSpace = ConversionToGig(Convert.ToUInt64(ld.Properties["FreeSpace"].Value)); //converted to gigs
                        totalSpace = ConversionToGig(Convert.ToUInt64(ld.Properties["Size"].Value)); //converted to gigs
                        usedSpace = totalSpace - freeSpace; //Calculate usedSpace
                        //End Calculate space-----
                        driveMediaType = Convert.ToUInt32(ld.Properties["MediaType"].Value); // c: 12
                        volumeName = Convert.ToString(ld.Properties["VolumeName"].Value); // System
                        volumeSerial = Convert.ToString(ld.Properties["VolumeSerialNumber"].Value); // 12345678

                        //DiiskDrive
                        driveStatus = Convert.ToString(d.Properties["Status"].Value);//Disk Status
                        physicalName = Convert.ToString(d.Properties["Name"].Value); // \\.\PHYSICALDRIVE2
                        //diskName = Convert.ToString(d.Properties["Caption"].Value); // WDC WD5001AALS-xxxxxx
                        diskManufacturer = Convert.ToString(d.Properties["Manufacturer"].Value);
                        diskModel = Convert.ToString(d.Properties["Model"].Value); // WDC WD5001AALS-xxxxxx
                        diskInterface = Convert.ToString(d.Properties["InterfaceType"].Value); // IDE
                        mediaLoaded = Convert.ToBoolean(d.Properties["MediaLoaded"].Value); // bool
                        mediaType = Convert.ToString(d.Properties["MediaType"].Value); // Fixed hard disk media
                        mediaSignature = Convert.ToUInt32(d.Properties["Signature"].Value); // int32
                        mediaStatus = Convert.ToString(d.Properties["Status"].Value); // OK

                        //Check to be sure the drive letter is equal to the driveName
                        if (driveName == driveLetter)
                        {
                            //Get the Physical Drive Number from the system EX: 0, 1, 2...
                            //Can only be called once the drive name = the incoming selected drive Letter
                            physicalDriveNumber = Convert.ToInt32(physicalName.Substring((physicalName.Length - 1), 1));
                            
                            //Write out the info to the string driveInfo
                            driveInfo = Environment.NewLine;
                            //Fill out the Information for the Drive
                            //------------------------------------------
                            //Check for if the drive has an OS
                            isOSDrive = ISOSDrive(driveLetter);
                            if (isOSDrive == true)
                            {
                                driveInfo += "OS Drive: True" + Environment.NewLine;
                            }
                            else
                            {
                                driveInfo += "OS Drive: False" + Environment.NewLine;
                            }

                            //Get Drive Type
                            driveInfo += "DriveType: " + DriveType(physicalDriveNumber) + Environment.NewLine;

                            //Logical Disk
                            driveInfo += "DriveName: " + driveName + Environment.NewLine;
                            driveInfo += "VolumeName: " + volumeName + Environment.NewLine;
                            driveInfo += "Status: " + driveStatus + Environment.NewLine;
                            driveInfo += "DriveCompressed: " + driveCompressed + Environment.NewLine; 
                            //Get Device Type
                            switch (driveType)
                            {
                                case 0:
                                    driveInfo += "DriveType: Unknown" + Environment.NewLine;
                                    break;

                                case 1:
                                    driveInfo += "DriveType: No Root Directory" + Environment.NewLine;
                                    break;

                                case 2:
                                    driveInfo += "DriveType: Removable Disk" + Environment.NewLine;
                                    break;

                                case 3:
                                    driveInfo += "DriveType: Local Disk" + Environment.NewLine;
                                    break;

                                case 4:
                                    driveInfo += "DriveType: Network Drive" + Environment.NewLine;
                                    break;

                                case 5:
                                    driveInfo += "DriveType: Compact Disk" + Environment.NewLine;
                                    break;

                                case 6:
                                    driveInfo += "DriveType: RAM Disk" + Environment.NewLine;
                                    break;
                            }
                            driveInfo += "FileSystem: " + fileSystem + Environment.NewLine;
                            //Setup checking if any of the space is over 1000 GB
                            //
                            driveInfo += "Used Space: " + usedSpace.ToString("0.00") + " GB" + Environment.NewLine;
                            driveInfo += "Free Space: " + freeSpace.ToString("0.00") + " GB" + Environment.NewLine;
                            driveInfo += "Total Space: " + totalSpace.ToString("0.00") + " GB" + Environment.NewLine;
                            driveInfo += "DriveMediaType: " + driveMediaType + Environment.NewLine;
                            driveInfo += "VolumeSerial: " + volumeSerial + Environment.NewLine;
                            driveInfo += Environment.NewLine;

                            //DiskDrive
                            driveInfo += "PhysicalName: " + physicalName + Environment.NewLine;
                            driveInfo += "DiskManufacturer: " + diskManufacturer + Environment.NewLine;
                            driveInfo += "DiskModel: " + diskModel + Environment.NewLine;
                            driveInfo += "DiskInterface: " + diskInterface + Environment.NewLine;
                            driveInfo += "MediaLoaded: " + mediaLoaded + Environment.NewLine;
                            driveInfo += "MediaType: " + mediaType + Environment.NewLine;
                            driveInfo += "MediaSignature: " + mediaSignature + Environment.NewLine;
                            driveInfo += "MediaStatus: " + mediaStatus + Environment.NewLine;
                            driveInfo += Environment.NewLine;

                            //Get Percent used
                            fltPercent = ((totalSpace - freeSpace) / totalSpace) * 100.0f;

                            //Input to user how much of the drive they have used
                            //This will show the percentage of the drive being used
                            main.lblDrivePGPercentage.Text = "You have used " + fltPercent.ToString("0.00") + "% of your drive";

                            //Progress bar This needs to be inside the if statement as they need to be initialized by the drive being ready
                            //Get drive max value
                            //This will set the maximun space on the drive to the progress bar
                            main.progHardDrive.Maximum = Convert.ToInt32(totalSpace);

                            //CurrentValue of drive usage
                            //This will get how much is used on the drive and set the value to the progress bar
                            main.progHardDrive.Value = Convert.ToInt32(totalSpace - freeSpace);

                            //Change color of progress bar depending on how much of drive is used
                            if (fltPercent >= 80.0f)
                            {
                                main.lblDrivePGPercentage.ForeColor = Color.Red; //Sets the label color to Crimson
                                main.progHardDrive.Style = MetroFramework.MetroColorStyle.Red;
                            }
                            else if (fltPercent >= 70.0f)
                            {
                                main.lblDrivePGPercentage.ForeColor = Color.Yellow; //Sets the label color to Crimson
                                main.progHardDrive.Style = MetroFramework.MetroColorStyle.Yellow;
                            }
                            else
                            {
                                main.lblDrivePGPercentage.ForeColor = Color.White; //Sets the label color to White
                                main.progHardDrive.Style = MetroFramework.MetroColorStyle.Blue;
                            }

                        }
                        else
                        {
                            //Do nothing
                        }
                        //Set the driveInfo to the rtb text box rtbDriveInfo
                        main.rtbDriveInfo.Text = driveInfo;
                        main.rtbDriveInfo.Refresh();
                    }
                }

            }
        }//End GetInformation

        #endregion

        //Detects if the Drive has an OS on it
        #region Detect OS Drive
        public bool ISOSDrive(string driveLetter)
        {
            //Pre: requires driveLetter to be initialized
            //Post: returns true or false
            //Purpose: To see if the drive selected is contains an OS

            //Create string driveLetter/Windows
            driveLetter += @"\Windows";
            //Check if the drive has a folder called Windows 
            if (Directory.Exists(driveLetter))
            {
                //Return true if it exists
                return true;
            }
            else
            {
                //Else returns false
                return false;
            }
        }//End Detect OS Drive
        #endregion


        //Some test code to get the drive type SSD HDD ect...
        private string DriveType(int driveNumber)
        {
            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM MSFT_PhysicalDisk where DeviceId = " + driveNumber.ToString());
            string type = "";
            scope.Connect();
            searcher.Scope = scope;

            try
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    switch (Convert.ToInt16(queryObj["MediaType"]))
                    {
                        case 1:
                            type = "Unspecified";
                            return type;

                        case 3:
                            type = "HDD";
                            return type;

                        case 4:
                            type = "SSD";
                            return type;

                        case 5:
                            type = "SCM";
                            return type;

                        default:
                            type = "Unspecified";
                            return type;
                    }
                }
            }
            catch
            {

            }
            
            searcher.Dispose();

            return type;
        }

    }//End class
}//End namespace
