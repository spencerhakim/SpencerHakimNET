using System;
using System.Threading;
using SpencerHakim.Extensions;

namespace SpencerHakim.Timers
{
    /// <summary>
    /// A Timer that is pausable. Based upon System.Threading.Timer, which has a resolution of ~15ms
    /// </summary>
    public class PausableTimer : IDisposable
    {
        #region Privates
        private Timer timer;
        private TimeSpan remainingTime;
        private DateTime startDateTime;
        private object syncRoot = new object();
        private volatile bool disposed = false;
        #endregion

        #region Constructors/destructor
        /// <summary>
        /// Creates a pausable wrapper for System.Threading.Timer with an interval of 100ms
        /// </summary>
        public PausableTimer() : this(new TimeSpan(0, 0, 0, 0, 100))
        {
            //
        }

        /// <summary>
        /// Creates a pausable wrapper for System.Threading.Timer with the specified interval
        /// </summary>
        /// <param name="interval">The interval for the timer to fire at</param>
        public PausableTimer(TimeSpan interval)
        {
            this.timer = new Timer(this.callback, null, Timeout.Infinite, Timeout.Infinite);
            this.Interval = interval;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~PausableTimer()
        {
            this.Dispose(false);
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Disposes of the object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if( !this.disposed )
            {
                if( disposing )
                    this.timer.SafeDispose();

                this.disposed = true;
            }
        }
        #endregion

        #region Properties and Events
        /// <summary>
        /// Occurs when the interval elapses 
        /// </summary>
        public event EventHandler<ElapsedEventArgs> Elapsed = delegate{};

        /// <summary>
        /// Gets or sets the interval at which to raise the Elapsed event 
        /// </summary>
        /// <remarks>TimeSpan.MaxValue is treated as Timeout.Infinite</remarks>
        public TimeSpan Interval
        {
            get { return this.interval; }
            set
            {
                lock( this.syncRoot )
                {
                    TimeSpan t = value;

                    //MaxValue will be treated as Infinite
                    if( t == TimeSpan.MaxValue )
                        t = new TimeSpan(0, 0, 0, 0, Timeout.Infinite);

                    if( t == this.interval )
                        return;

                    //Timer throws NotSupportedExceptions when dueTime or period exceeds 4,294,967,294ms
                    if( t.TotalMilliseconds > UInt32.MaxValue - 1 )
                        throw new NotSupportedException("Interval cannot be larger than 4,294,967,294ms");

                    this.interval = t;

                    //This is what the standard timers do
                    if( this.Enabled )
                    {
                        this.Reset();
                        this.Enabled = true;
                    }
                    else
                    {
                        this.remainingTime = TimeSpan.Zero;
                    }
                }
            }
        }
        private TimeSpan interval = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets a value indicating whether the Timer should raise the Elapsed event
        /// </summary>
        public bool Enabled
        {
            get { return this.enabled; }
            set
            {
                lock( this.syncRoot )
                {
                    if( value == this.enabled )
                        return;

                    this.enabled = value;
                    if( value )
                    {
                        this.startDateTime = DateTime.Now;
                        this.timer.Change(this.Interval - this.remainingTime, this.Interval);
                    }
                    else
                    {
                        this.timer.Change(Timeout.Infinite, Timeout.Infinite); //0,0 triggers once immediately; inf,inf triggers never
                        this.remainingTime = (this.Interval < DateTime.Now - this.startDateTime) ? this.Interval : (DateTime.Now - this.startDateTime);
                    }
                }
            }
        }
        private volatile bool enabled;
        #endregion

        #region Methods
        /// <summary>
        /// Stops and resets the remaining time in the timer
        /// </summary>
        public void Reset()
        {
            lock( this.syncRoot )
            {
                this.Enabled = false;
                this.remainingTime = TimeSpan.Zero;
            }
        }

        private void callback(object state)
        {
            bool fire;
            lock( this.syncRoot )
            {
                this.startDateTime = DateTime.Now;
                fire = this.Elapsed != null && this.Enabled;
            }

            try
            {
                // System.Timers.Timer.Elapsed swallows exceptions, bah
                if( fire )
                    this.Elapsed(this, new ElapsedEventArgs(DateTime.Now));
            }
            catch{}
        }
        #endregion
    }
}
