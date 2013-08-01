using System;
using System.Collections.Generic;
using System.Threading;
using SpencerHakim.Extensions;

namespace SpencerHakim.Logging
{
    /// <summary>
    /// Registers and manages ILoggers
    /// </summary>
    public class Log : IDisposable
    {
        private List<ILogger> loggers = new List<ILogger>();
        private Queue< Tuple<Severity, string> > queue = new Queue< Tuple<Severity,string> >();
        private Thread queueThread;
        private AutoResetEvent notify = new AutoResetEvent(false);
        private bool shutdown = false;

        private object syncRoot = new object();
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
            lock( this.syncRoot )
                this.loggers.Add(logger);
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

            this.queue.Enqueue( Tuple.Create(level, text) );
            this.notify.Set();
        }

        private void queueProcessor()
        {
            while( !this.shutdown )
            {
                while( this.queue.Count > 0 )
                {
                    var data = this.queue.Dequeue();

                    lock( this.syncRoot )
                        foreach( var logger in this.loggers )
                        {
                            try
                            {
                                logger.Write(data.Item1, data.Item2);
                            }
                            catch{} //guess we'll just have to live with it?
                        }
                }

                if( !this.shutdown )
                    this.notify.WaitOne();
            }
        }
    }
}
