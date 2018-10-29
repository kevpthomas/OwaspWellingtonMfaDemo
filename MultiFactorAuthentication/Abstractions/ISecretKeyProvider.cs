// ReSharper disable CommentTypo
namespace MultiFactorAuthentication.Abstractions
{
    /// <summary>
    /// Defines HOTP key functionality.
    /// </summary>
    public interface ISecretKeyProvider
    {
        /// <summary>
        /// Decodes an encoded HOTP key to an array of bytes.
        /// </summary>
        /// <param name="encodedKey">Key to be decoded.</param>
        /// <returns>
        /// HOTP key decoded to an array of bytes.
        /// </returns>
        byte[] DecodeKey(string encodedKey);

        /// <summary>
        /// Creates an encoded HOTP key.
        /// </summary>
        /// <returns>
        /// Encoded HOTP key.
        /// </returns>
        string CreateKey();
    }
}