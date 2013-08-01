using System;
using System.Security.Principal;

namespace SpencerHakim
{
    /// <summary>
    /// Environment info
    /// </summary>
    public static class Environment
    {
        /// <summary>
        /// Gets the current Unix timestamp (seconds since midnight UTC 1/1/1970)
        /// </summary>
        public static Int64 UnixTimestamp
        {
            get 
            {
                return System.Convert.ToInt64( Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds) );
            }
        }

        /// <summary>
        /// Gets whether the current user and process have admin privileges
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                using( WindowsIdentity identity = WindowsIdentity.GetCurrent() )
                    return (new WindowsPrincipal(identity)).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
