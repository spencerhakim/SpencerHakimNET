using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SpencerHakim.Logging;

namespace SpencerHakim.Net.Syslog
{
    /// <summary>
    /// Syslog facility
    /// </summary>
    public enum SyslogFacility : int
    {
#pragma warning disable 1591 //fuck writing XML docs for this shit
        Kernel = 0,
        User = 1,
        Mail = 2,
        Daemon = 3,
        Auth = 4,
        Syslog = 5,
        Lpr = 6,
        News = 7,
        UUCP = 8,
        Cron = 9,
        Local0 = 10,
        Local1 = 11,
        Local2 = 12,
        Local3 = 13,
        Local4 = 14,
        Local5 = 15,
        Local6 = 16,
        Local7 = 17,
#pragma warning restore 1591
    }

    /// <summary>
    /// Contains all the necessary data for a syslog message
    /// </summary>
    public class Message
    {
#pragma warning disable 1591 //yeah, still too lazy
        public SyslogFacility Facility { get; set; }
        public Severity Severity { get; set; }
        public string AppName { get; private set; }
        public int ProcessID { get; private set; }
        public string MessageID { get; set; }
        public string Text { get; set; }
#pragma warning restore 1591

        /// <summary>
        /// Initializes an instance of the Message class
        /// </summary>
        public Message()
        {
            this.AppName = Process.GetCurrentProcess().ProcessName;
            this.ProcessID = Process.GetCurrentProcess().Id;
        }
    }

    /// <summary>
    /// Syslog client
    /// </summary>
    public static class Syslog
    {
        private readonly static string NILVALUE = "-"; //syslog nil value
        private readonly static int VERSION = 1; //protocol version

        /// <summary>
        /// Sends the given syslog message to the specified IP and port
        /// </summary>
        /// <param name="message">Syslog message to send</param>
        /// <param name="remote">IP and port of the syslog daemon</param>
        public static void Send(Message message, IPEndPoint remote)
        {
            if( message == null )
                throw new ArgumentNullException("message");

            if( remote == null )
                throw new ArgumentNullException("remote");

            //define fields
            int pri = ((int)message.Facility * 8) + (int)message.Severity;
            string timestamp = new DateTimeOffset(DateTime.Now, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now)).ToString("yyyy-MM-ddTHH:mm:ss.ffffffzzz");
            string hostname = Dns.GetHostEntry( Dns.GetHostName() ).HostName;
            string appName = String.IsNullOrWhiteSpace(message.AppName) ? NILVALUE : message.AppName;
            string msgId = String.IsNullOrWhiteSpace(message.MessageID) ? NILVALUE : message.MessageID;

            //build header
            string header = String.Format("<{0}>{1} {2} {3} {4} {5} {6}", pri, VERSION, timestamp, hostname, appName, message.ProcessID, msgId);

            //build message bytes
            List<byte> syslogMsg = new List<byte>();
            syslogMsg.AddRange( Encoding.ASCII.GetBytes(header) );
            syslogMsg.AddRange( Encoding.ASCII.GetBytes(" " + NILVALUE) ); //tag?
            if( !String.IsNullOrWhiteSpace(message.Text) )
                syslogMsg.AddRange( Encoding.UTF8.GetBytes(message.Text) );

            //send UDP datagram
            byte[] bytes = syslogMsg.ToArray();
            using( UdpClient udpClient = new UdpClient(remote) )
                udpClient.Send(bytes, bytes.Length);
        }
    }
}
