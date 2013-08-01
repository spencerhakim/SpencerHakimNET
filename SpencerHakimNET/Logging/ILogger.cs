namespace SpencerHakim.Logging
{
    /// <summary>
    /// Syslog message severity levels
    /// </summary>
    public enum Severity : int
    {
        /// <summary>
        /// System is unusable. A "panic" condition usually affecting multiple apps/servers/sites. At this level it would usually notify all tech staff on call.
        /// </summary>
        Emergency = 0,

        /// <summary>
        /// Action must be taken immediately. Should be corrected immediately, therefore notify staff who can fix the problem. An example would be the loss of a primary ISP connection.
        /// </summary>
        Alert = 1,

        /// <summary>
        /// Critical conditions. Should be corrected immediately, but indicates failure in a primary system. An example would be the loss of a backup ISP connection.
        /// </summary>
        Critical = 2,

        /// <summary>
        /// Error conditions. Non-urgent failures, these should be relayed to developers or admins; each item must be resolved within a given time.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Warning conditions. Not an error, but indication that an error will occur if action is not taken, e.g. file system 85% full - each item must be resolved within a given time.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Normal but significant condition. Events that are unusual but not error conditions - might be summarized in an email to developers or admins to spot potential problems - no immediate action required.
        /// </summary>
        Notice = 5,

        /// <summary>
        /// Informational messages. Normal and may be harvested for reporting, measuring throughput, etc. - no action required.
        /// </summary>
        Info = 6,

        /// <summary>
        /// Debug-level messages. Info useful to developers for debugging the application, not useful during operations.
        /// </summary>
        Debug = 7
    }

    /// <summary>
    /// Interface for logger implentations
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes text to a log output
        /// </summary>
        /// <param name="level">The severity level of the log</param>
        /// <param name="text">The text to log</param>
        void Write(Severity level, string text);
    }
}
