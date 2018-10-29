using System;
using System.Text;
using OtpSharp;
using Wiry.Base32;

namespace MultiFactorAuthentication
{
    public static class MultifactorExtensions
    {
        /// <summary>
        /// Derives an instance of <see cref="Totp"/> from a secret key.
        /// </summary>
        /// <param name="base32SecretKey">Key used for token generation as a Base32 string.</param>
        /// <param name="stepSeconds">Number of seconds in a time-step.</param>
        /// <param name="stepOffset">Number of seconds to offset the time counter.</param>
        /// <returns>
        /// An instance of <see cref="Totp"/> encapsulating the provided key, time-step, and offset.
        /// </returns>
        public static Totp ToTotp(this string base32SecretKey, int stepSeconds = 30, int stepOffset = 0)
        {
            return Base32Encoding.Standard.ToBytes(base32SecretKey)
                .ToTotp(stepSeconds, stepOffset);
        }

        /// <summary>
        /// Derives an instance of <see cref="Totp"/> from a secret key.
        /// </summary>
        /// <param name="secretKey">Key used for token generation as a byte array.</param>
        /// <param name="stepSeconds">Number of seconds in a time-step.</param>
        /// <param name="stepOffset">Number of seconds to offset the time counter.</param>
        /// <returns>
        /// An instance of <see cref="Totp"/> encapsulating the provided key, time-step, and offset.
        /// </returns>
        public static Totp ToTotp(this byte[] secretKey, int stepSeconds = 30, int stepOffset = 0)
        {
            return new Totp(secretKey, 
                stepSeconds, 
                timeCorrection: stepOffset == 0 ? null : new TimeCorrection(DateTime.UtcNow.AddSeconds(stepOffset * stepSeconds)));
        }

        /// <summary>
        /// Converts an ASCII string to a byte array.
        /// </summary>
        /// <param name="ascii">Source ASCII string.</param>
        /// <returns>Byte array</returns>
        public static byte[] ToBytesFromAscii(this string ascii)
        {
            return Encoding.ASCII.GetBytes(ascii);
        }
    }
}