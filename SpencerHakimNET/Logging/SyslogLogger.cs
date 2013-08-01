using System.Net;
using SpencerHakim.Net.Syslog;

namespace SpencerHakim.Logging
{
    /// <summary>
    /// Syslog implementation of ILogger
    /// </summary>
    public class SyslogLogger : ILogger
    {
        /// <summary>
        /// Creates an ILogger backed by a SyslogLogger with the specified parameters
        /// </summary>
        /// <param name="facility">Syslog facility for log output</param>
        /// <param name="ip">IP address and port to send logs to</param>
        /// <returns>ILogger backed by a SyslogLogger</returns>
        public static ILogger Create(SyslogFacility facility, IPEndPoint ip)
        {
            return new SyslogLogger(facility, ip);
        }

        /// <summary>
        /// Gets facility level 
        /// </summary>
        public SyslogFacility Facility { get; private set; }

        /// <summary>
        /// Gets endpoint of syslog daemon
        /// </summary>
        public IPEndPoint Endpoint { get; private set; }

        /// <summary>
        /// Initializes an instance of the SyslogLogger class
        /// </summary>
        /// <param name="facility">Syslog facility for log output</param>
        /// <param name="ip">IP address and port to send logs to</param>
        protected SyslogLogger(SyslogFacility facility, IPEndPoint ip)
        {
            this.Facility = facility;
            this.Endpoint = ip;
        }

        /// <summary>
        /// Writes text to a syslog server
        /// </summary>
        /// <param name="level">The severity level of the log</param>
        /// <param name="text">The text to log</param>
        public void Write(Severity level, string text)
        {
            var message = new Message()
            {
                Facility = SyslogFacility.User,
                Severity = level,
                Text = text
            };

            Syslog.Send(message, this.Endpoint);
        }
    }
}
