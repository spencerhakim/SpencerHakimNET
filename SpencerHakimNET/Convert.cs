using System;
using System.Collections.Generic;
using System.Text;

namespace SpencerHakim
{
    /// <summary>
    /// Converts data
    /// </summary>
    public class Convert
    {
        private const int IN_BYTE_SIZE = 8;
        private const int OUT_BYTE_SIZE = 5;
        private static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-32 digits.
        /// </summary>
        /// <param name="data">Byte data to encode</param>
        /// <returns>Base-32 encoded data</returns>
        public static string ToBase32String(byte[] data)
        {
            int i = 0, index = 0, digit = 0;
            int current_byte, next_byte;
            StringBuilder result = new StringBuilder((data.Length + 7) * IN_BYTE_SIZE / OUT_BYTE_SIZE);

            while (i < data.Length)
            {
                current_byte = (data[i] >= 0) ? data[i] : (data[i] + 256); // Unsign

                //Is the current digit going to span a byte boundary?
                if (index > (IN_BYTE_SIZE - OUT_BYTE_SIZE))
                {
                    if ((i + 1) < data.Length)
                        next_byte = (data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256);
                    else
                        next_byte = 0;

                    digit = current_byte & (0xFF >> index);
                    index = (index + OUT_BYTE_SIZE) % IN_BYTE_SIZE;
                    digit <<= index;
                    digit |= next_byte >> (IN_BYTE_SIZE - index);
                    i++;
                }
                else
                {
                    digit = (current_byte >> (IN_BYTE_SIZE - (index + OUT_BYTE_SIZE))) & 0x1F;
                    index = (index + OUT_BYTE_SIZE) % IN_BYTE_SIZE;
                    if (index == 0)
                        i++;
                }

                result.Append(alphabet[digit]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data as base-32 digits, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="str">Base-32 encoded data to decode</param>
        /// <returns>Decoded byte data</returns>
        public static byte[] FromBase32String(string str)
        {
            if( str == null )
                throw new ArgumentNullException("str");

            str = str.ToUpper();

            int n = 0;
            int j = 0;
            List<byte> bytes = new List<byte>();

            for(int i=0; i < str.Length; i++)
            {
                n <<= 5; // Move buffer left by 5 to make room
                n += Array.IndexOf(alphabet, str[i]); // Add value into buffer
                j += 5; // Keep track of number of bits in buffer

                if( j >= 8)
                {
                    j -= 8;
                    bytes.Add( (byte)((n & (0xFF << j)) >> j) );
                }
            }

            return bytes.ToArray();
        }
    }
}
