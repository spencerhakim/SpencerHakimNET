using System;
using System.Runtime.InteropServices;

namespace SpencerHakim.Timers
{
    /// <summary>
    /// High-precision timer
    /// </summary>
    public class MultimediaTimer : IDisposable
    {
        static class NativeMethods
        {
            /// <summary>
            /// Times the set event.
            /// </summary>
            /// <param name="uDelay">The u delay.</param>
            /// <param name="uResolution">The u resolution.</param>
            /// <param name="lpTimeProc">The lp time proc.</param>
            /// <param name="dwUser">The dw user.</param>
            /// <param name="fuEvent">The fu event.</param>
            /// <returns></returns>
            [DllImport("Winmm.dll", SetLastError=true)]
            public static extern uint timeSetEvent(uint uDelay, uint uResolution, TimerCallback lpTimeProc, UIntPtr dwUser, TIME fuEvent);

            /// <summary>
            /// Times the kill event.
            /// </summary>
            /// <param name="uTimerID">The u timer ID.</param>
            /// <returns></returns>
            [DllImport("Winmm.dll", SetLastError=true)]
            public static extern uint timeKillEvent(uint uTimerID);

            /// <summary>
            /// Times the get time.
            /// </summary>
            /// <returns></returns>
            [DllImport("Winmm.dll", SetLastError=true)]
            public static extern uint timeGetTime();

            /// <summary>
            /// Times the begin period.
            /// </summary>
            /// <param name="uPeriod">The u period.</param>
            /// <returns></returns>
            [DllImport("Winmm.dll", SetLastError=true)]
            public static extern uint timeBeginPeriod(uint uPeriod);

            /// <summary>
            /// Times the end period.
            /// </summary>
            /// <param name="uPeriod">The u period.</param>
            /// <returns></returns>
            [DllImport("Winmm.dll", SetLastError=true)]
            public static extern uint timeEndPeriod(uint uPeriod);
        }

        #region Privates
        private uint timerId = 0;
        private object syncRoot = new object();
        private volatile bool disposed = false;
        #endregion

        #region Types
        private enum TIME : uint
        {
            /// <summary>
            /// Event occurs once, after uDelay milliseconds
            /// </summary>
            ONESHOT = 0,

            /// <summary>
            /// Event occurs every uDelay milliseconds
            /// </summary>
            PERIODIC = 1
        }

        /// <summary>
        /// Delegate definition for the API callback
        /// </summary>
        private delegate void TimerCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);
        #endregion

        #region Constructors/desructor
        /// <summary>
        /// Creates a new instance of the MultimediaTimer class with an interval of 100ms
        /// </summary>
        public MultimediaTimer() : this(new TimeSpan(0, 0, 0, 0, 100))
        {
            //
        }

        /// <summary>
        /// Creates a new instance of the MultimediaTimer class with the specified interval
        /// </summary>
        /// <param name="interval">The interval period, in milliseconds</param>
        public MultimediaTimer(TimeSpan interval)
        {
            this.Interval = interval;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MultimediaTimer()
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
                /*if( disposing )
                    this.timer.SafeDispose();*/

                this.Enabled = false;
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
        /// Gets and sets the running state of the timer
        /// </summary>
        public bool Enabled
        {
            get { return this.enabled; }
            set
            {
                lock( this.syncRoot )
                {
                    if( this.enabled == value )
                        return;
                    this.enabled = value;

                    //start
                    if( this.enabled )
                    {
                        //Set the timer type flags
                        TIME fuEvent = (this.AutoReset ? TIME.PERIODIC : TIME.ONESHOT);

                        this.timerId = NativeMethods.timeSetEvent((uint)this.Interval.TotalMilliseconds, (uint)this.Resolution.TotalMilliseconds, this.callback, UIntPtr.Zero, fuEvent);
                        if (this.timerId == 0)
                            Marshal.ThrowExceptionForHR( Marshal.GetHRForLastWin32Error() );
                    }
                    else //stop
                    {
                        if (this.timerId != 0)
                        {
                            if( NativeMethods.timeKillEvent(this.timerId) != 0 )
                                Marshal.ThrowExceptionForHR( Marshal.GetHRForLastWin32Error() );
                            this.timerId = 0;
                        }
                    }
                }
            }
        }
        private volatile bool enabled = false;

        /// <summary>
        /// Gets and sets whether the timer fires repeatedly or just once. Changing this restarts the timer
        /// </summary>
        public bool AutoReset
        {
            get { return this.autoReset; }
            set
            {
                lock( this.syncRoot )
                {
                    bool temp = this.Enabled;
                    this.Enabled = false;

                    this.autoReset = value;
                    
                    this.Enabled = temp;
                }
            }
        }
        private volatile bool autoReset = true;

        /// <summary>
        /// Gets and sets the timer interval period, in milliseconds. Changing this restarts the timer
        /// </summary>
        public TimeSpan Interval
        {
            get { return this.interval; }
            set
            {
                lock( this.syncRoot )
                {
                    bool temp = this.Enabled;
                    this.Enabled = false;

                    this.interval = value;
                    
                    this.Enabled = temp;
                }
            }
        }
        private TimeSpan interval = new TimeSpan(0, 0, 0, 0, 100);

        /// <summary>
        /// Gets and sets the timer accuracy, in milliseconds. Use the maximum possible value for your needs, as lower values increase system load. Changing this restarts the timer
        /// </summary>
        public TimeSpan Resolution
        {
            get { return this.resolution; }
            set
            {
                lock( this.syncRoot )
                {
                    bool temp = this.Enabled;
                    this.Enabled = false;

                    this.resolution = value;
                    
                    this.Enabled = temp;
                }
            }
        }
        private TimeSpan resolution = new TimeSpan(0, 0, 0, 0, 5);
        #endregion

        #region Methods
        /// <summary>
        /// Starts or restarts the timer
        /// </summary>
        public void Start()
        {
            lock (this.syncRoot)
            {
                //Kill any existing timer, then start again
                this.Enabled = false;
                this.Enabled = true;
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            lock (this.syncRoot)
            {
                this.Enabled = false;
            }
        }

        private void callback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            bool fire;
            lock( this.syncRoot )
                fire = this.Elapsed != null && this.Enabled;

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
