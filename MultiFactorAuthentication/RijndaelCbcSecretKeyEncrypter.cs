using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MultiFactorAuthentication.Abstractions;
// ReSharper disable IdentifierTypo

namespace MultiFactorAuthentication
{
    /// <summary>
    /// Implements <see cref="ISecretKeyEncrypter"/> using the AES/Rijndael algorithm
    /// in CBC mode.
    /// </summary>
    /// <remarks>
    ///adapted from https://tekeye.uk/visual_studio/encrypt-decrypt-c-sharp-string
    /// </remarks>
    public class RijndaelCbcSecretKeyEncrypter : ISecretKeyEncrypter
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string InitVector = "bpjK4wuTsLYuNugJ";

        // This constant is used to determine the keysize of the encryption algorithm
        private const int Keysize = 256;

        public string EncryptString(string plainText, string passPhrase, string salt)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var password = new Rfc2898DeriveBytes(passPhrase, salt.ToBytesFromAscii());
            var keyBytes = password.GetBytes(Keysize / 8);
            var symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            var cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string DecryptString(string encryptedText, string passPhrase, string salt)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            var cipherTextBytes = Convert.FromBase64String(encryptedText);
            var password = new Rfc2898DeriveBytes(passPhrase, salt.ToBytesFromAscii());
            var keyBytes = password.GetBytes(Keysize / 8);
            var symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            var plainTextBytes = new byte[cipherTextBytes.Length];
            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}