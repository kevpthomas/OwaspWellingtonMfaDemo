using MultiFactorAuthentication.Abstractions;
// ReSharper disable IdentifierTypo

namespace MultiFactorAuthentication
{
    /// <summary>
    /// Implements <see cref="ITotpTokenProvider"/> using demonstration
    /// methods of how to generate a TOTP token. 
    /// </summary>
    public class ManualTotpTokenProvider : ITotpTokenProvider
    {
        private readonly ISecretKeyProvider _secretKeyProvider;
        private readonly ITotpTokenBuilder _totpTokenBuilder;

        public ManualTotpTokenProvider(ISecretKeyProvider secretKeyProvider, 
            ITotpTokenBuilder totpTokenBuilder)
        {
            _secretKeyProvider = secretKeyProvider;
            _totpTokenBuilder = totpTokenBuilder;
        }

        public int TokenSize => 6;

        public string ComputeToken(string base32SecretKey, int stepSeconds = 30, int stepOffset = 0)
        {
            return ComputeToken(_secretKeyProvider.DecodeKey(base32SecretKey), stepSeconds, stepOffset);
        }

        public string ComputeToken(byte[] secretKey, int stepSeconds = 30, int stepOffset = 0)
        {
            var timeCounter = _totpTokenBuilder.CreateCounter(stepSeconds, stepOffset);

            var sha1Hash = _totpTokenBuilder.GenerateSha1Hash(secretKey, timeCounter);

            var offset = _totpTokenBuilder.ComputeOffset(sha1Hash);

            var truncatedHash = _totpTokenBuilder.TruncateHash(sha1Hash, offset);

            var hotpToken = _totpTokenBuilder.ComputeHotp(truncatedHash, TokenSize);

            return hotpToken;
        }

        public int GetRemainingSecondsInCurrentInterval(int stepSeconds = 30)
        {
            return _totpTokenBuilder.RemainingSecondsInCurrentInterval(stepSeconds);
        }
    }
}