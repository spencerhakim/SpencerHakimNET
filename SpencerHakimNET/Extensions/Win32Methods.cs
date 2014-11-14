using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Linq;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods that use Windows specific APIs (such as System.Management)
    /// </summary>
    public static class Win32Methods
    {
        static class NativeMethods
        {
            [DllImport("shell32.dll", SetLastError=true)]
            private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)]string lpCmdLine, out int pNumArgs);

            public static string[] CommandLineToArgvW(string lpCmdLine)
            {
                int pNumArgs;

                var ptr = CommandLineToArgvW(lpCmdLine, out pNumArgs);
                if( ptr == IntPtr.Zero )
                    Marshal.ThrowExceptionForHR( Marshal.GetHRForLastWin32Error() );

                try
                {
                    var args = new string[pNumArgs];
                    for(var i=0; i < args.Length; i++)
                    {
                        var p = Marshal.ReadIntPtr(ptr, i*IntPtr.Size);
                        args[i] = Marshal.PtrToStringUni(p);
                    }

                    return args;
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        /// <summary>
        /// Restarts the Process, providing the same commandline arguments that were originally provided. Does not kill the original Process.
        /// </summary>
        /// <param name="process">The Process to restart</param>
        public static void Restart(this Process process)
        {
            if( process == null )
                throw new ArgumentNullException("process");

            var args = process.GetCommandLineArgs().Skip(1);
            var psi = new ProcessStartInfo(process.MainModule.FileName, String.Join(" ", args));
            Process.Start(psi);
        }

        /// <summary>
        /// Gets the command line arguments used to start a Process
        /// </summary>
        /// <param name="process">The Process to get the command line arguments for</param>
        /// <returns>An array of the command line arguments</returns>
        public static string[] GetCommandLineArgs(this Process process)
        {
            if( process == null )
                throw new ArgumentNullException("process");

            //do a bit less work for the current process
            if( process.Id == Process.GetCurrentProcess().Id )
                return System.Environment.GetCommandLineArgs();

            //have to use WMI for other processes
            string cmdLine = null;
            wmiQuery(String.Format("SELECT CommandLine FROM win32_process WHERE ProcessId={0}", process.Id), x => {
                    cmdLine = x["CommandLine"].ToString();
            });

            return NativeMethods.CommandLineToArgvW(cmdLine);
        }

        /// <summary>
        /// Get the child Process IDs of a given process
        /// </summary>
        /// <param name="process">Process to analyze</param>
        /// <returns>List of child Process IDs otherwise</returns>
        public static ReadOnlyCollection<int> GetChildProcessIds(this Process process)
        {
            if( process == null )
                throw new ArgumentNullException("process");

            List<int> childPIds = new List<int>();

            wmiQuery(String.Format("SELECT ProcessId FROM win32_process WHERE ParentProcessId={0}", process.Id), x => {
                int pid = Int32.Parse( x["ProcessId"].ToString() );
                childPIds.Add(pid);
            });

            return new ReadOnlyCollection<int>(childPIds);
        }

        private static void wmiQuery(string query, Action<ManagementObject> action)
        {
            using( var objSearcher = new ManagementObjectSearcher(query) )
            using( var result = objSearcher.Get() )
            {
                foreach( var mo in result.Cast<ManagementObject>() )
                {
                    action(mo);
                    mo.SafeDispose();
                }
            }
        }
    }
}
