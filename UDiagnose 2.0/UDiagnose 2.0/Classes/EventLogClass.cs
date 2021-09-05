//Author: Kenneth Lamb

using System.Diagnostics;

namespace UDiagnose_2._0.Classes
{
    class EventLogClass
    {
       
        //This will be selected by the user from the list of Events to view on the computer
        private string eventLogName = "System";
        //Create the event log
        EventLog eventLog = new EventLog();
        //Create string array to hold the logs
        public string[,] logs;
        //Set the row number
        private int row = 0;

        //Private Method to load the events
        public void LoadEvents()
        {
            //Gets the specific logs by the log name.
            eventLog.Log = eventLogName;
            //Set the array to equal the eventLog entry count
            logs = new string[eventLog.Entries.Count,4];

            //Loop through the array backwards to get the most recent timeWritten error
            for(int i = (eventLog.Entries.Count - 1); i > 0; i--)
            {
                var log = eventLog.Entries[i];
                //Add the time, entry type, source, and instance ID
                logs[row, 0] += log.TimeWritten;
                logs[row, 1] += log.EntryType;
                logs[row, 2] += log.Source;
                logs[row, 3] += log.InstanceId;
                //Increase the row
                row++;
            }
        }

        
        //Public Method to load the rich text box
        public void LoadInfo(frmMain main)
        {
            LoadEvents();

            for (int i = 0; i < 60; i++)
            {
                //Change to the rich text box you are using
                //main.dgEvents.Rows.Add(logs[i, 0], logs[i, 1], logs[i, 2], logs[i, 3]);
            }
        }
    }//End class
}//End namespace
