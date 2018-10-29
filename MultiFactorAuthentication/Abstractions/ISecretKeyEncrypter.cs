// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace MultiFactorAuthentication.Abstractions
{
    /// <summary>
    /// Defines a mechanism for HOTP key encryption and decryption.
    /// </summary>
    public interface ISecretKeyEncrypter
    {
        /// <summary>
        /// Encrypts a plain text HOTP key.
        /// </summary>
        /// <param name="plainText">HOTP key to be encrypted.</param>
        /// <param name="passPhrase">Password used to derive the encryption key.</param>
        /// <param name="salt">Salt used to derive the encryption key.</param>
        /// <returns>
        /// An encrypted HOTP key.
        /// </returns>
        string EncryptString(string plainText, string passPhrase, string salt);

        /// <summary>
        /// Decrypts an encrypted HOTP key.
        /// </summary>
        /// <param name="encryptedText">Encrypted HOTP key.</param>
        /// <param name="passPhrase">Password used to derive the decryption key.</param>
        /// <param name="salt">Salt used to derive the decryption key.</param>
        /// <returns>
        /// A plain text HOTP key.
        /// </returns>
        string DecryptString(string encryptedText, string passPhrase, string salt);
    }
}