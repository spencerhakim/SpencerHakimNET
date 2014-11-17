using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using WinFormsApp = System.Windows.Forms.Application;
using WPFApp = System.Windows.Application;

namespace SpencerHakim.Diagnostics
{
    /// <summary>
    /// Enumeration of unhandled exception origins
    /// </summary>
    public enum ExceptionOrigin
    {
        /// <summary>
        /// Triggered by AppDomain.UnhandledException
        /// </summary>
        AppDomain,

        /// <summary>
        /// Triggered by System.Windows.Forms.Application.ThreadException
        /// </summary>
        WinFormsThread,

        /// <summary>
        /// Triggered by System.Windows.Application.DispatcherUnhandledException
        /// </summary>
        Dispatcher,

        /// <summary>
        /// Triggered by using System.Windows.Threading.TaskScheduler.UnobservedTaskException
        /// </summary>
        TaskScheduler
    }

    /// <summary>
    /// Helper for catching unhandled exceptions, including Corrupted State Exceptions
    /// </summary>
    public static class UnhandledExceptions
    {
        private static bool _initialized;

        /// <summary>
        /// Callback for unhandled exceptions
        /// </summary>
        public static Action<ExceptionOrigin, Exception> UnhandledException { get; set; }

        static UnhandledExceptions()
        {
            UnhandledException = (o,e)=>{};
        }

        /// <summary>
        /// Initializes the unhandled exception handler
        /// </summary>
        /// <param name="handleCSE">Whether or not to catch Corrupted State Exceptions</param>
        /// <param name="disableWinForms">Throw WinForms thread errors in AppDomain</param>
        public static void InitHandler(bool handleCSE, bool disableWinForms)
        {
            if( _initialized )
                return;
            _initialized = true;

            //hook handlers
            if( handleCSE )
            {
                AppDomain.CurrentDomain.UnhandledException += cseAppDomainUnhandledException;
                WinFormsApp.ThreadException += cseWinFormsThreadException;
                WPFApp.Current.DispatcherUnhandledException += cseDispatcherUnhandledException;
                TaskScheduler.UnobservedTaskException += cseUnobservedTaskException;
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException += appDomainUnhandledException;
                WinFormsApp.ThreadException += winFormsThreadException;
                WPFApp.Current.DispatcherUnhandledException += dispatcherUnhandledException;
                TaskScheduler.UnobservedTaskException += unobservedTaskException;
            }

            if( disableWinForms )
                WinFormsApp.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.ThrowException);
        }

        #region CSE handlers
        [HandleProcessCorruptedStateExceptions]
        private static void cseAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.AppDomain, e.ExceptionObject as Exception);
        }

        [HandleProcessCorruptedStateExceptions]
        private static void cseWinFormsThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.WinFormsThread, e.Exception);
        }

        [HandleProcessCorruptedStateExceptions]
        private static void cseDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.Dispatcher, e.Exception);
        }

        [HandleProcessCorruptedStateExceptions]
        private static void cseUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.TaskScheduler, e.Exception);
        }
        #endregion

        #region Non-CSE handlers
        private static void appDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.AppDomain, e.ExceptionObject as Exception);
        }

        private static void winFormsThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.WinFormsThread, e.Exception);
        }

        private static void dispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.Dispatcher, e.Exception);
        }

        private static void unobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            UnhandledException(ExceptionOrigin.TaskScheduler, e.Exception);
        }
        #endregion
    }
}
