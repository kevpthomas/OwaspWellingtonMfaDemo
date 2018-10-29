namespace MultiFactorAuthentication.Abstractions
{
    /// <summary>
    /// Describes functionality used to provide a TOTP token,
    /// defined in RFC 6238, as an authentication factor.
    /// </summary>
    public interface ITotpTokenProvider
    {
        /// <summary>
        /// Computes a TOTP token.
        /// </summary>
        /// <param name="base32SecretKey">Key used for token generation as a Base32 string.</param>
        /// <param name="stepSeconds">Number of seconds in a time-step.</param>
        /// <param name="stepOffset">Number of seconds to offset the time counter.</param>
        /// <returns>
        /// A TOTP token.
        /// </returns>
        string ComputeToken(string base32SecretKey, int stepSeconds = 30, int stepOffset = 0);

        /// <summary>
        /// Computes a TOTP token.
        /// </summary>
        /// <param name="secretKey">Key used for token generation as a byte array.</param>
        /// <param name="stepSeconds">Number of seconds in a time-step.</param>
        /// <param name="stepOffset">Number of seconds to offset the time counter.</param>
        /// <returns>
        /// A TOTP token.
        /// </returns>
        string ComputeToken(byte[] secretKey, int stepSeconds = 30, int stepOffset = 0);

        /// <summary>
        /// Computes the number of seconds remaining in the current time-step interval.
        /// </summary>
        /// <param name="stepSeconds">Number of seconds in a time-step.</param>
        /// <returns>Remaining seconds in the current time-step interval.</returns>
        int GetRemainingSecondsInCurrentInterval(int stepSeconds = 30);
    }
}