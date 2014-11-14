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
        public static long UnixTimestamp
        {
            get 
            {
                return System.Convert.ToInt64( Math.Round((DateTime.UtcNow - UnixEpochDateTime).TotalSeconds) );
            }
        }

        /// <summary>
        /// Gets the current Unix timestamp as a DateTime
        /// </summary>
        public static DateTime UnixEpochDateTime
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets whether the current process (and, by extension, the user) have admin privileges
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                using( var identity = WindowsIdentity.GetCurrent() )
                    return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
