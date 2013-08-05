using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods that use Windows specific APIs (such as System.Management)
    /// </summary>
    public static class Win32Methods
    {
        /// <summary>
        /// Get the child Process IDs of a given process
        /// </summary>
        /// <param name="process">Process to analyze</param>
        /// <returns>Null if failed to connect to WMI, List of child Process IDs otherwise</returns>
        public static ReadOnlyCollection<int> GetChildProcessIds(this Process process)
        {
            if( process == null )
                throw new ArgumentNullException("process");

            String machineName = "localhost";
            String myQuery = string.Format("SELECT ProcessId FROM win32_process WHERE ParentProcessId={0}", process.Id);
            List<int> childPIds = null;

            ManagementScope mScope = new ManagementScope(String.Format(@"\\{0}\root\cimv2", machineName), null);
            mScope.Connect();
            if( mScope.IsConnected )
            {
                ObjectQuery objQuery = new ObjectQuery(myQuery);
                using( ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(mScope, objQuery) )
                using( ManagementObjectCollection result = objSearcher.Get() )
                {
                    childPIds = new List<int>();
                    foreach( ManagementObject item in result )
                    {
                        int pid = Int32.Parse( item["ProcessId"].ToString() );
                        childPIds.Add(pid);
                    }
                }
            }

            return new ReadOnlyCollection<int>(childPIds);
        }
    }
}
