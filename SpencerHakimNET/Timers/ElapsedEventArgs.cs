using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpencerHakim.Timers
{
    /// <summary>
    /// Provides data for Elapsed events
    /// </summary>
    public class ElapsedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new ElapsedEventArgs
        /// </summary>
        /// <param name="signalTime">The DateTime at which the Elapsed event fire</param>
        public ElapsedEventArgs(DateTime signalTime)
        {
            this.SignalTime = signalTime;
        }

        /// <summary>
        /// Gets the DateTime at which the Elapsed event fire
        /// </summary>
        public DateTime SignalTime { get; protected set; }
    }
}
