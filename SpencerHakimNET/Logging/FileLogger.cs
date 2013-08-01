using System;
using System.IO;
using SpencerHakim.Extensions;

namespace SpencerHakim.Logging
{
    /// <summary>
    /// File implementation of ILogger
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// Creates an ILogger backed by a FileLogger with the specified parameter
        /// </summary>
        /// <param name="file">File to write logs to</param>
        /// <returns>ILogger backed by a FileLogger</returns>
        public static ILogger Create(string file)
        {
            return new FileLogger(file);
        }

        /// <summary>
        /// Gets file to write logs to
        /// </summary>
        public string LogFile { get; private set; }

        /// <summary>
        /// Initializes an instance of the FileLogger class
        /// </summary>
        /// <param name="file">File to log to</param>
        protected FileLogger(string file)
        {
            this.LogFile = file;
        }

        /// <summary>
        /// Writes text to a file
        /// </summary>
        /// <param name="level">The severity level of the log</param>
        /// <param name="text">The text to log</param>
        public void Write(Severity level, string text)
        {
            string formatted = String.Format(
                "{0}: <{1}> {2}",
                DateTime.Now,
                Enum.GetName(typeof(Severity), level),
                text
            );

            FileStream fs = null;
            try
            {
                fs = new FileStream(this.LogFile, FileMode.Append, FileAccess.Write, FileShare.Read);
                using( var sw = new StreamWriter(fs) )
                    sw.WriteLine(formatted);
            }
            finally
            {
                fs.SafeDispose();
            }
        }
    }
}
