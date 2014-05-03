﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using SpencerHakim.Extensions;
using MSG = System.Tuple<SpencerHakim.Logging.Severity, string>;

namespace SpencerHakim.Logging
{
    /// <summary>
    /// Really simple logger. If you actually need something legit, use something else.
    /// </summary>
    public class Log : IDisposable
    {
        private List<ILogger> loggers = new List<ILogger>();
        private ConcurrentQueue<MSG> queue = new ConcurrentQueue<MSG>();
        private Thread queueThread;
        private AutoResetEvent notify = new AutoResetEvent(false);
        private bool shutdown = false;

        private object syncLoggers = new object();
        private bool disposed = false;

        /// <summary>
        /// Gets and sets the minimum severity level to actually log
        /// </summary>
        public Severity VerbosityLevel { get; set; }

        /// <summary>
        /// Initializes an instace of the Log class
        /// </summary>
        public Log()
        {
            this.queueThread = new Thread(this.queueProcessor);
            this.queueThread.Name = "Log Queue Processor";
            this.queueThread.IsBackground = false;
            this.queueThread.Start();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Log()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Disposer
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes managed and unmanaged resources
        /// </summary>
        /// <param name="disposing">Disposes both managed and unmanaged when true, unmanaged only when false</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId="notify")]
        protected virtual void Dispose(bool disposing)
        {
            if( !this.disposed )
            {
                this.disposed = true;

                this.shutdown = true;
                this.notify.Set();

                if( disposing )
                    this.notify.SafeDispose();
            }
        }

        /// <summary>
        /// Registers the given logger
        /// </summary>
        /// <param name="logger">The logger to register</param>
        public void Add(ILogger logger)
        {
            if( logger == null )
                throw new ArgumentNullException("logger");

            //don't want to modify the collection while iterating through it
            lock( this.syncLoggers )
                this.loggers.Add(logger);
        }

        /// <summary>
        /// Writes text to all loggers
        /// </summary>
        /// <param name="level">The severity level of the log</param>
        /// <param name="text">The text to log</param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        public void Write(Severity sev, string text, params object[] args)
        {
            text = String.Format(text, args);
            Write(sev, text);
        }

        /// <summary>
        /// Writes text to all loggers
        /// </summary>
        /// <param name="level">The severity level of the log</param>
        /// <param name="text">The text to log</param>
        public void Write(Severity level, string text)
        {
            if( level > this.VerbosityLevel )
                return;

            this.queue.Enqueue( new MSG(level, text) );
            this.notify.Set();
        }

        #region Quick calls
        /// <summary>
        /// Writes an Emergency message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Emergency(string text)
        {
            this.Write(Severity.Emergency, text);
        }

        /// <summary>
        /// Writes an Alert message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Alert(string text)
        {
            this.Write(Severity.Alert, text);
        }

        /// <summary>
        /// Writes a Critical message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Critical(string text)
        {
            this.Write(Severity.Critical, text);
        }

        /// <summary>
        /// Writes an Error message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Error(string text)
        {
            this.Write(Severity.Error, text);
        }

        /// <summary>
        /// Writes a Warning message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Warning(string text)
        {
            this.Write(Severity.Warning, text);
        }

        /// <summary>
        /// Writes a Notice message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Notice(string text)
        {
            this.Write(Severity.Notice, text);
        }

        /// <summary>
        /// Writes an Info message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Info(string text)
        {
            this.Write(Severity.Info, text);
        }

        /// <summary>
        /// Writes a Debug message
        /// </summary>
        /// <param name="text">The message to log</param>
        public void Debug(string text)
        {
            this.Write(Severity.Debug, text);
        }
        #endregion

        private void queueProcessor()
        {
            while( !this.shutdown )
            {
                MSG data = null;
                while( this.queue.TryDequeue(out data) )
                {
                    lock( this.syncLoggers )
                    {
                        foreach( var logger in this.loggers )
                        {
                            try
                            {
                                logger.Write(data.Item1, data.Item2);
                            }
                            catch{} //guess we'll just have to live with it?
                        }
                    }
                }

                if( !this.shutdown )
                    this.notify.WaitOne();
            }
        }
    }
}
