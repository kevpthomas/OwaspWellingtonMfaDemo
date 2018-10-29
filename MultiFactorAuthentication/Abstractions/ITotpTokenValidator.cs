namespace MultiFactorAuthentication.Abstractions
{
    /// <summary>
    /// Provides functionality for TOTP token validation.
    /// </summary>
    public interface ITotpTokenValidator
    {
        /// <summary>
        /// Verifies a provided TOTP token.
        /// </summary>
        /// <param name="base32SecretKey">Key used for token generation as a Base32 string.</param>
        /// <param name="token">TOTP token to verify.</param>
        /// <param name="stepSeconds">Number of seconds in a time-step.</param>
        /// <param name="previous">Number of previous time-steps in the verification window.</param>
        /// <param name="future">Number of future time-steps in the verification window.</param>
        /// <returns>true if the provided token has been verified; otherwise false</returns>
        bool VerifyToken(string base32SecretKey,
            string token,
            int stepSeconds = 30,
            int previous = 0, 
            int future = 0);
    }
}