using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods that use System.Windows.Forms or System.Management
    /// </summary>
    public static class WindowsMethods
    {
        /// <summary>
        /// Non-broken replacement for Control.DesignMode
        /// </summary>
        /// <param name="ctrl">Control to check if in design mode</param>
        /// <returns>True if in design mode, false otherwise</returns>
        /// <remarks>See: http://stackoverflow.com/questions/34664/designmode-with-controls </remarks>
        public static bool InDesignMode(this Control ctrl)
        {
            //only reliable check for inside constructor
            if( LicenseManager.UsageMode == LicenseUsageMode.Designtime )
                return true;

            //only reliable check for outside constructor; similar to official implementation
            while( ctrl != null )
            {
                if( (ctrl.Site != null) && ctrl.Site.DesignMode )
                    return true;
                ctrl = ctrl.Parent;
            }
            return false;
        }

        /// <summary>
        /// Determines if a Rectangle intersects a screen area
        /// </summary>
        /// <param name="rect">The rectangle that may intersect a screen area</param>
        /// <returns>True if the rectangle intersects a screen area, false otherwise</returns>
        public static bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach(Screen screen in Screen.AllScreens)
            {
                if( screen.WorkingArea.IntersectsWith(rect) )
                    return true;
            }

            return false;
        }

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
