using System;
using System.Collections.Generic;
using System.Linq;
using MultiFactorAuthentication.Abstractions;

namespace MultiFactorAuthentication
{
    /// <summary>
    /// Implements <see cref="ITotpTokenValidator"/> using demonstration
    /// methods of how to verify a TOTP token. 
    /// </summary>
    public class ManualTotpTokenValidator : ITotpTokenValidator
    {
        private readonly ITotpTokenProvider _totpTokenProvider;

        public ManualTotpTokenValidator(ITotpTokenProvider totpTokenProvider)
        {
            _totpTokenProvider = totpTokenProvider;
        }

        public bool VerifyToken(string base32SecretKey, 
            string token,
            int stepSeconds = 30,
            int previous = 0, 
            int future = 0)
        {
            var verificationTokens = new List<string>();

            for (var stepOffset = -1 * previous; stepOffset <= future; stepOffset++)
            {
                verificationTokens.Add(_totpTokenProvider.ComputeToken(base32SecretKey, stepSeconds, stepOffset));
            }

            return verificationTokens.Contains(token, StringComparer.InvariantCulture);
        }
    }
}