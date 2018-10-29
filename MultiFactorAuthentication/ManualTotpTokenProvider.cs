using MultiFactorAuthentication.Abstractions;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

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
            // create time-based HOTP/TOTP counter calculated from Unix epoch, as a big endian byte array
            var timeCounter = _totpTokenBuilder.CreateCounter(stepSeconds, stepOffset);

            // calculate HMAC hash of time-based counter using secret key
            var sha1Hash = _totpTokenBuilder.GenerateSha1Hash(secretKey, timeCounter);

            // compute random offset index within HMAC hash using least significant byte
            var offset = _totpTokenBuilder.ComputeOffset(sha1Hash);

            // compute full-length HOTP/TOTP token by truncating the HMAC hash 
            var truncatedHash = _totpTokenBuilder.TruncateHash(sha1Hash, offset);

            // extract HOTP/TOTP token of desired length with leading zeros as needed 
            var hotpToken = _totpTokenBuilder.ExtractHotp(truncatedHash, TokenSize);

            return hotpToken;
        }

        public int GetRemainingSecondsInCurrentInterval(int stepSeconds = 30)
        {
            return _totpTokenBuilder.RemainingSecondsInCurrentInterval(stepSeconds);
        }
    }
}