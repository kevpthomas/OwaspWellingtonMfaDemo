using MultiFactorAuthentication.Abstractions;
using OtpSharp;

namespace MultiFactorAuthentication
{
    /// <summary>
    /// Implements <see cref="ITotpTokenValidator"/> using the OtpSharp library.
    /// </summary>
    public class OtpSharpTotpTokenValidator : ITotpTokenValidator
    {
        public bool VerifyToken(string base32SecretKey, 
            string token,
            int stepSeconds = 30,
            int previous = 0, 
            int future = 0)
        {
            var otp = base32SecretKey.ToTotp(stepSeconds);
            var isValid = otp.VerifyTotp(token, out _, new VerificationWindow(previous, future));
            return isValid;
        }
    }
}