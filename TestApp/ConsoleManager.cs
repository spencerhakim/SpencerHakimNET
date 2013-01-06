using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace TestApp
{
    [SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        #region Console interop
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        private static extern int GetConsoleOutputCP();
        #endregion

        #region WinUI interop
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport ("user32.dll" )]
        private static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_GRAYED = 0x00000001;
        private const uint MF_BYCOMMAND = 0x00000000;
        #endregion

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            if( !HasConsole )
            {
                AllocConsole();
                InvalidateOutAndError();
                DisableConsoleClose();
            }
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            if( HasConsole )
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

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

        private static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            FieldInfo _out = type.GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( _out != null );

            FieldInfo _error = type.GetField("_error", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( _error != null );

            MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError", BindingFlags.Static | BindingFlags.NonPublic);
            Debug.Assert( _InitializeStdOutError != null );

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[]{ true } );
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        private static void DisableConsoleClose()
        {
            IntPtr hMenu = GetSystemMenu(GetConsoleWindow(), false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
            RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
        }
    }
}