using MultiFactorAuthentication.Abstractions;

namespace MultiFactorAuthentication
{
    /// <summary>
    /// Implements <see cref="ITotpTokenProvider"/> using the OtpSharp library.
    /// </summary>
    public class OtpSharpTotpTokenProvider : ITotpTokenProvider
    {
        private readonly ITotpTokenBuilder _totpTokenBuilder;

        public OtpSharpTotpTokenProvider(ITotpTokenBuilder totpTokenBuilder)
        {
            _totpTokenBuilder = totpTokenBuilder;
        }

        public string ComputeToken(string base32SecretKey, int stepSeconds = 30, int stepOffset = 0)
        {
            var otp = base32SecretKey.ToTotp(stepSeconds, stepOffset);
            return otp.ComputeTotp();
        }

        public string ComputeToken(byte[] secretKey, int stepSeconds = 30, int stepOffset = 0)
        {
            var otp = secretKey.ToTotp(stepSeconds, stepOffset);
            return otp.ComputeTotp();
        }

        public int GetRemainingSecondsInCurrentInterval(int stepSeconds = 30)
        {
            return _totpTokenBuilder.RemainingSecondsInCurrentInterval(stepSeconds);
        }
    }
}