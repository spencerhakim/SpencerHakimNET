using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpencerHakim.Logging
{
    /// <summary>
    /// Console implementation of ILogger
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly static object consoleLock = new object();

        /// <summary>
        /// Creates an ILogger backed by a ConsoleLogger with the specified parameter
        /// </summary>
        /// <returns>ILogger backed by a ConsoleLogger</returns>
        public static ILogger Create()
        {
            return new ConsoleLogger();
        }

        /// <summary>
        /// Initializes an instance of the ConsoleLogger class
        /// </summary>
        protected ConsoleLogger()
        {
            //
        }

        /// <summary>
        /// Writes text to the console
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

            //select appropriate color for severity
            ConsoleColor bg = Console.BackgroundColor, fg = Console.ForegroundColor;
            switch(level)
            {
                case Severity.Emergency:
                    bg = ConsoleColor.White;
                    fg = ConsoleColor.Red;
                    break;

                case Severity.Alert:
                case Severity.Critical:
                    fg = ConsoleColor.Magenta;
                    break;

                case Severity.Error:
                    fg = ConsoleColor.Red;
                    break;

                case Severity.Warning:
                    fg = ConsoleColor.Yellow;
                    break;

                case Severity.Notice:
                    fg = ConsoleColor.Cyan;
                    break;

                case Severity.Info:
                    fg = ConsoleColor.Gray;
                    break;

                case Severity.Debug:
                    fg = ConsoleColor.DarkGray;
                    break;

                default:
                    break;
            }

            //write to the console
            lock( consoleLock )
            {
                Console.BackgroundColor = bg;
                Console.ForegroundColor = fg;
                Console.WriteLine(formatted);
                Console.ResetColor();
            }
        }
    }
}
