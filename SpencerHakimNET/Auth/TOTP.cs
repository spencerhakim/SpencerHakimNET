using System;
using System.Linq;
using System.Security.Cryptography;

namespace SpencerHakim.Auth
{
    /// <summary>
    /// Implements Google's Two Factor authentication of timed one-time passwords
    /// </summary>
    public class TOTP
    {
        /// <summary>
        /// Calculates the TOTP for the current timestamp
        /// </summary>
        /// <param name="key">The secret key for calculating the TOTP</param>
        /// <returns>Integer value of the TOTP. You may need to format it as a string.</returns>
        public static int Calculate(string key)
        {
            return Calculate(key, Environment.UnixTimestamp);
        }

        /// <summary>
        /// Calculates the TOTP for the given timestamp
        /// </summary>
        /// <param name="key">The secret key for calculating the TOTP</param>
        /// <param name="timestamp">The timestamp to calculate the TOTP for</param>
        /// <returns>Integer value of the TOTP. You may need to format it as a string.</returns>
        public static int Calculate(string key, long timestamp)
        {
            timestamp /= 30;

            // https://tools.ietf.org/html/rfc4226
            byte[] secretKey = Convert.FromBase32String(key);
            byte[] data = BitConverter.GetBytes(timestamp).Reverse().ToArray();
            byte[] hmac = new HMACSHA1(secretKey).ComputeHash(data);

            //truncate
            int offset = hmac.Last() & 0x0F;
            return (
                ((hmac[offset + 0] & 0x7f) << 24) |
                ((hmac[offset + 1] & 0xff) << 16) |
                ((hmac[offset + 2] & 0xff) << 8) |
                (hmac[offset + 3] & 0xff)
                    ) % 1000000;
        }
    }
}
