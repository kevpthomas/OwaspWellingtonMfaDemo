using System;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace MultiFactorAuthentication.Abstractions
{
    /// <summary>
    /// Describes functionality used to build a TOTP token as
    /// defined in RFC 6238.
    /// </summary>
    public interface ITotpTokenBuilder
    {
        /// <summary>
        /// Gets the current Unix (UTC) time.
        /// </summary>
        DateTime CurrentUnixTime { get; }

        /// <summary>
        /// Gets the Unix Epoch value as described in RFC 6238.
        /// </summary>
        /// <remarks>
        /// References:
        /// https://tools.ietf.org/html/rfc6238
        /// </remarks>
        DateTime UnixEpoch { get; }

        /// <summary>
        /// Creates a time-based, non-decreasing counter to be used as the counter in the HOTP algorithm
        /// to generate a TOTP code. 
        /// </summary>
        /// <param name="stepSeconds">Time-step size in seconds, defaults to 30</param>
        /// <param name="stepOffset">Step offset used for clock drift correction.</param>
        /// <returns>
        /// Time counter represented as a big-endian byte array.
        /// </returns>
        /// <remarks>
        /// References:
        /// https://en.wikipedia.org/wiki/Time-based_One-time_Password_algorithm
        /// https://tools.ietf.org/html/rfc6238
        /// https://tools.ietf.org/html/rfc4226#section-5.2
        /// </remarks>
        byte[] CreateCounter(int stepSeconds = 30, int stepOffset = 0);

        /// <summary>
        /// Calculates an HMAC-SHA1 hash of a provided TOTP time counter
        /// using a provided hash algorithm key.
        /// </summary>
        /// <param name="secretKey">The key to use in the hash algorithm.</param>
        /// <param name="timeCounter">The TOTP time counter to be hashed.</param>
        /// <returns>The computed hash code.</returns>
        /// <remarks>
        /// References:
        /// https://tools.ietf.org/html/rfc2104
        /// </remarks>
        byte[] GenerateSha1Hash(byte[] secretKey, byte[] timeCounter);

        /// <summary>
        /// Computes a random offset within the provided HMAC-SHA1 hash
        /// using the least significant byte.
        /// </summary>
        /// <param name="hmacResult"> An HMAC-SHA1 hash of a TOTP time counter.</param>
        /// <returns>
        /// An offset index randomly selected to be in the range of 0 to 15.
        /// </returns>
        /// <remarks>
        /// References:
        /// https://tools.ietf.org/html/rfc4226
        /// </remarks>
        int ComputeOffset(byte[] hmacResult);

        /// <summary>
        /// Dynamically truncates an HMAC-SHA1 hash of a TOTP time counter
        /// using a provided offset.
        /// </summary>
        /// <param name="hmacResult"> An HMAC-SHA1 hash of a TOTP time counter.</param>
        /// <param name="offset">A random offset index.</param>
        /// <returns>
        /// A truncated hash containing an HOTP token.
        /// </returns>
        /// <remarks>
        /// References:
        /// https://tools.ietf.org/html/rfc4226
        /// </remarks>
        int TruncateHash(byte[] hmacResult, int offset);

        /// <summary>
        /// Extracts an HOTP token from a provided truncated hash
        /// using a provided number of digits.
        /// </summary>
        /// <param name="binCode">A truncated hash containing an HOTP token.</param>
        /// <param name="digitCount">Number of digits in the extracted HOTP token.</param>
        /// <returns>
        /// An HOTP token of the desired length with leading 0's as needed,
        /// extracted from the least-significant digits of the original truncated hash.
        /// </returns>
        /// <remarks>
        /// References:
        /// https://tools.ietf.org/html/rfc4226
        /// </remarks>
        string ComputeHotp(long binCode, int digitCount);

        /// <summary>
        /// Computes the remaining seconds in the current TOTP time step interval.
        /// </summary>
        /// <param name="stepSeconds">Number of seconds in a time step.</param>
        /// <returns>
        /// Number of remaining seconds in the current time step.
        /// </returns>
        int RemainingSecondsInCurrentInterval(int stepSeconds = 30);
    }
}