using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace SpencerHakim
{
    /// <summary>
    /// Manages a console window for GUI apps. Useful for debug logging.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        static class NativeMethods
        {

            #region Console interop
            [DllImport("kernel32.dll")]
            public static extern bool AllocConsole();

            [DllImport("kernel32.dll")]
            public static extern bool FreeConsole();

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("Kernel32")]
            public static extern bool SetConsoleCtrlHandler(IntPtr handlr, bool add);
            #endregion

            #region WinUI interop
            [DllImport("user32.dll")]
            public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

            [DllImport("user32.dll")]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

            [DllImport ("user32.dll" )]
            public static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);
            #endregion
        }

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_BYCOMMAND = 0x00000000;

        /// <summary>
        /// Gets the handle to the console window
        /// </summary>
        public static IntPtr Handle
        {
            get { return NativeMethods.GetConsoleWindow(); }
        }

        /// <summary>
        /// Gets whether or not a console window is open
        /// </summary>
        public static bool HasConsole
        {
            get { return Handle != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            if( !HasConsole )
            {
                NativeMethods.AllocConsole();
                resetOutAndError();
                disableConsoleClose();
            }
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            if( HasConsole )
            {
                setOutAndErrorNull();
                NativeMethods.FreeConsole();
            }
        }

        /// <summary>
        /// Toggles a console window open/close
        /// </summary>
        public static void Toggle()
        {
            if( HasConsole )
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <remarks>
        /// Forgive me for my sins
        /// </remarks>
        private static void resetOutAndError()
        {
            var type = typeof(Console);

            //get the privates
            var _out = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( _out != null );

            var _error = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( _error != null );

            var InternalSyncObject = type.GetProperty("InternalSyncObject", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( InternalSyncObject != null );

            var InitializeStdOutError = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( InitializeStdOutError != null );

            //lock and invalidate _out and _error
            lock( InternalSyncObject.GetValue(null, null) )
            {
                _out.SetValue(null, null);
                _error.SetValue(null, null);
            }

            //reinitialize _out and _error so they point to the new console
            InitializeStdOutError.Invoke(null, new object[]{ true } );
            InitializeStdOutError.Invoke(null, new object[]{ false } );
        }

        private static void setOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        private static void disableConsoleClose()
        {
            NativeMethods.SetConsoleCtrlHandler(IntPtr.Zero, true); //ignores Ctrl-C
            var hMenu = NativeMethods.GetSystemMenu(NativeMethods.GetConsoleWindow(), false);
            NativeMethods.EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED); //disables the upper-right Close (X) button in the titlebar
            NativeMethods.RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND); //removes the Close option in the Alt-Space menu
        }
    }
}