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
    public static class ProcessMethods
    {
        private static class NativeMethods
        {
            [Flags]
            public enum ThreadAccess
            {
                TERMINATE               = 0x0001,
                SUSPEND_RESUME          = 0x0002,
                GET_CONTEXT             = 0x0008,
                SET_CONTEXT             = 0x0010,
                SET_INFORMATION         = 0x0020,
                QUERY_INFORMATION       = 0x0040,
                SET_THREAD_TOKEN        = 0x0080,
                IMPERSONATE             = 0x0100,
                DIRECT_IMPERSONATION    = 0x0200
            }

            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern uint SuspendThread(IntPtr hThread);

            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern int ResumeThread(IntPtr hThread);

            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern bool CloseHandle(IntPtr hHandle);

            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern uint GetCurrentThreadId();

            [DllImport("kernel32.dll", SetLastError=true)]
            public static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

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
        /// Suspends all threads belong to the process, except for the current thread if this is the current process
        /// </summary>
        /// <param name="process"></param>
        public static void Suspend(this Process process)
        {
            if( process == null )
                throw new ArgumentNullException("process");

            foreachThread(process, p => NativeMethods.SuspendThread(p));
        }

        /// <summary>
        /// Resumes all threads belong to the process, except for the current thread if this is the current process
        /// </summary>
        /// <param name="process"></param>
        public static void Resume(this Process process)
        {
            if( process == null )
                throw new ArgumentNullException("process");

            foreachThread(process, p => NativeMethods.ResumeThread(p));
        }

        /// <summary>
        /// Determines if a process is 32- or 64-bit
        /// </summary>
        /// <param name="process">The Process to determine the architecture for</param>
        /// <returns>True if 64-bit, otherwise false</returns>
        public static bool Is64Bit(this Process process)
        {
            if( System.Environment.OSVersion.Version.Major < 6 || !System.Environment.Is64BitOperatingSystem )
                return false; //32-bit Windows

            bool result;
            if( !NativeMethods.IsWow64Process(process.Handle, out result) )
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            return result;
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

        private static int currentThreadInSameProcess(Process proc)
        {
            var currentThread = 0;
            using( var self = Process.GetCurrentProcess() )
                if( self.Id == proc.Id )
                    currentThread = (int)NativeMethods.GetCurrentThreadId();

            return currentThread;
        }

        private static void foreachThread(Process proc, Action<IntPtr> action)
        {
            var currentThread = currentThreadInSameProcess(proc);

            foreach( var pOpenThread in proc.Threads.Cast<ProcessThread>()
                .Where( pT => pT.Id != currentThread )
                .Select( pT => NativeMethods.OpenThread(NativeMethods.ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id) )
                .Where( pOpenThread => pOpenThread != IntPtr.Zero ))
            {
                try
                {
                    action(pOpenThread);
                }
                catch{}
                NativeMethods.CloseHandle(pOpenThread);
            }
        }
    }
}
