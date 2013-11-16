using System;
using System.Linq;
using System.Security.Cryptography;
using SpencerHakim.Extensions;

namespace SpencerHakim.Auth
{
    /// <summary>
    /// Implements Google's Two Factor authentication of timed one-time passwords
    /// </summary>
    /// <typeparam name="Thmac">The HMAC implementation to use</typeparam>
    public class TOTP<Thmac> : IDisposable where Thmac : HMAC
    {
        private Thmac hmac;
        private int counter;
        private bool disposed = false;

        /// <summary>
        /// Creates a TOTP generator with the specified Base32-encoded secret key and a time window of 30 seconds
        /// </summary>
        /// <param name="secretKey">The secret key for generating the TOTP</param>
        public TOTP(string secretKey) : this(secretKey, 30)
        {
            //
        }

        /// <summary>
        /// Creates a TOTP generator with the specified Base32-encoded secret key and time window
        /// </summary>
        /// <param name="secretKey">The secret key for generating the TOTP</param>
        /// <param name="counter">The time window, in seconds (usually 30)</param>
        public TOTP(string secretKey, int counter)
        {
            this.hmac = (Thmac)Activator.CreateInstance(typeof(Thmac), Convert.FromBase32String(secretKey));
            this.counter = counter;
        }

        /// <summary>
        /// Calculates the TOTP for the current timestamp
        /// </summary>
        /// <returns>Zero-padded string of six-digit TOTP</returns>
        public string Calculate()
        {
            return this.Calculate(Environment.UnixTimestamp);
        }

        /// <summary>
        /// Calculates the TOTP for the given timestamp
        /// </summary>
        /// <param name="timestamp">The timestamp to calculate the TOTP for</param>
        /// <returns>Zero-padded string of six-digit TOTP</returns>
        public string Calculate(DateTimeOffset timestamp)
        {
            return this.Calculate(System.Convert.ToInt64( Math.Round((timestamp - Environment.UnixEpochDateTime).TotalSeconds) ));
        }

        /// <summary>
        /// Generates a six-digit TOTP for the provided timestamp
        /// </summary>
        /// <param name="timestamp">The timestamp to generate the TOTP for</param>
        /// <returns>Zero-padded string of six-digit TOTP</returns>
        public string Calculate(long timestamp)
        {
            timestamp /= this.counter;

            // https://tools.ietf.org/html/rfc4226
            byte[] data = BitConverter.GetBytes(timestamp).Reverse().ToArray();
            byte[] hmacData = this.hmac.ComputeHash(data);

            //truncate
            int offset = hmacData.Last() & 0x0F;
            int totp = (
                ((hmacData[offset + 0] & 0x7f) << 24) |
                ((hmacData[offset + 1] & 0xff) << 16) |
                ((hmacData[offset + 2] & 0xff) << 8) |
                (hmacData[offset + 3] & 0xff)
                    ) % 1000000;

            return totp.ToString("D6");
        }

        /// <summary>
        /// Calculates the TOTP for the current timestamp using the SHA-1 algorithm, and a time window of 30 seconds
        /// </summary>
        /// <param name="key">The secret key for calculating the TOTP</param>
        /// <returns>Zero-padded string of six-digit TOTP</returns>
        public static string Calculate(string key)
        {
            return Calculate(key, Environment.UnixTimestamp);
        }

        /// <summary>
        /// Calculates the TOTP for the given timestamp using the SHA-1 algorithm, and a time window of 30 seconds
        /// </summary>
        /// <param name="key">The secret key for calculating the TOTP</param>
        /// <param name="timestamp">The timestamp to calculate the TOTP for</param>
        /// <returns>Zero-padded string of six-digit TOTP</returns>
        public static string Calculate(string key, DateTimeOffset timestamp)
        {
            return Calculate(key, System.Convert.ToInt64( Math.Round((timestamp - Environment.UnixEpochDateTime).TotalSeconds) ));
        }

        /// <summary>
        /// Calculates the TOTP for the given timestamp using the SHA-1 algorithm, and a time window of 30 seconds
        /// </summary>
        /// <param name="key">The secret key for calculating the TOTP</param>
        /// <param name="timestamp">The timestamp to calculate the TOTP for</param>
        /// <returns>Zero-padded string of six-digit TOTP</returns>
        public static string Calculate(string key, long timestamp)
        {
            var totp = new TOTP<HMACSHA1>(key, 30);
            return totp.Calculate(timestamp);
        }

        ~TOTP()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if( disposed )
                return;
            disposed = true;

            if( disposing )
            {
                this.hmac.SafeDispose();
            }
        }
    }
}
