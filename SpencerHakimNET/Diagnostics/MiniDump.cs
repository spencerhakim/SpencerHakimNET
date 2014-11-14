using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SpencerHakim.Diagnostics
{
    /// <summary>
    /// Options for creating minidumps
    /// </summary>
    [Flags]
    public enum MiniDumpOption : uint
    {
        // From dbghelp.h:
        Normal                          = 0x00000000,
        WithDataSegs                    = 0x00000001,
        WithFullMemory                  = 0x00000002,
        WithHandleData                  = 0x00000004,
        FilterMemory                    = 0x00000008,
        ScanMemory                      = 0x00000010,
        WithUnloadedModules             = 0x00000020,
        WithIndirectlyReferencedMemory  = 0x00000040,
        FilterModulePaths               = 0x00000080,
        WithProcessThreadData           = 0x00000100,
        WithPrivateReadWriteMemory      = 0x00000200,
        WithoutOptionalData             = 0x00000400,
        WithFullMemoryInfo              = 0x00000800,
        WithThreadInfo                  = 0x00001000,
        WithCodeSegs                    = 0x00002000,
        WithoutAuxiliaryState           = 0x00004000,
        WithFullAuxiliaryState          = 0x00008000,
        WithPrivateWriteCopyMemory      = 0x00010000,
        IgnoreInaccessibleMemory        = 0x00020000,

        ValidTypeFlags                  = 0x0003ffff,
    };

    //typedef struct _MINIDUMP_EXCEPTION_INFORMATION {
    //    DWORD ThreadId;
    //    PEXCEPTION_POINTERS ExceptionPointers;
    //    BOOL ClientPointers;
    //} MINIDUMP_EXCEPTION_INFORMATION, *PMINIDUMP_EXCEPTION_INFORMATION;
    /// <summary>
    /// Struct for packaging information regarding an exception that caused the minidump
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=4)]  // Pack=4 is important! So it works also for x64!
    public struct MiniDumpExceptionInformation
    {
        /// <summary>
        /// The identifier of the thread throwing the exception.
        /// </summary>
        public uint ThreadId;

        /// <summary>
        /// A pointer to an EXCEPTION_POINTERS structure specifying a computer-independent description
        /// of the exception and the processor context at the time of the exception.
        /// </summary>
        public IntPtr ExceptionPointers;

        /// <summary>
        /// Determines where to get the memory regions pointed to by the ExceptionPointers member.
        /// Set to TRUE if the memory resides in the process being debugged (the target process of the debugger).
        /// Otherwise, set to FALSE if the memory resides in the address space of the calling program (the debugger process).
        /// If you are accessing local memory (in the calling process) you should not set this member to TRUE.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool ClientPointers;
    }

    /// <summary>
    /// Functions for generating minidumps
    /// </summary>
    public static class MiniDump
    {
        /// <summary>
        /// Can be used when creating MiniDumpExceptionInformation
        /// </summary>
        /// <returns>The current thread's ID</returns>
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern uint GetCurrentThreadId();

        //BOOL
        //WINAPI
        //MiniDumpWriteDump(
        //    __in HANDLE hProcess,
        //    __in DWORD ProcessId,
        //    __in HANDLE hFile,
        //    __in MINIDUMP_TYPE DumpType,
        //    __in_opt PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
        //    __in_opt PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
        //    __in_opt PMINIDUMP_CALLBACK_INFORMATION CallbackParam
        //    );
        [DllImport("dbghelp.dll", CharSet=CharSet.Unicode, ExactSpelling=true, SetLastError=true)]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr pExceptionInfo, IntPtr pUserStreamInfo, IntPtr pCallbackInfo);

        /// <summary>
        /// Creates a minidump of the specified process with the specified options and exception information,
        /// and writes it to the specified location
        /// </summary>
        /// <param name="process">Process to dump</param>
        /// <param name="filepath">Location to write dump to</param>
        /// <param name="options">Dump options</param>
        /// <param name="mei">Exception information to include with the dump</param>
        public static void WriteDump(Process process, string filepath, MiniDumpOption options, MiniDumpExceptionInformation mei)
        {
            var pMei = Marshal.AllocHGlobal(Marshal.SizeOf(mei));

            try
            {
                using( var fs = new FileStream(filepath, FileMode.Create) )
                {
                    Marshal.StructureToPtr(mei, pMei, false);

                    var result = MiniDumpWriteDump(
                        process.Handle,
                        (uint)process.Id,
                        fs.SafeFileHandle,
                        (uint)options,
                        (mei.ExceptionPointers != IntPtr.Zero ? pMei : IntPtr.Zero),
                        IntPtr.Zero,
                        IntPtr.Zero
                    );

                    if( !result )
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pMei);
            }
        }
    }
}