using System.Security.Cryptography;
using MultiFactorAuthentication.Abstractions;
using Wiry.Base32;
// ReSharper disable CommentTypo

namespace MultiFactorAuthentication
{
    /// <summary>
    /// Implements <see cref="ISecretKeyProvider"/> using Base32 encoding.
    /// </summary>
    public class Base32SecretKeyProvider : ISecretKeyProvider
    {
        public byte[] DecodeKey(string encodedKey)
        {
            /*
             * HMAC: Keyed-Hashing for Message Authentication
             * https://tools.ietf.org/html/rfc2104
             *
             * R6 - The algorithm MUST use a strong shared secret.  The length of
             * the shared secret MUST be at least 128 bits.  This document
             * RECOMMENDs a shared secret length of 160 bits.
             */
            var key = Base32Encoding.Standard.ToBytes(encodedKey);
            return key;
        }

        public string CreateKey()
        {
            /*
             * HOTP: An HMAC-Based One-Time Password Algorithm
             * https://tools.ietf.org/html/rfc4226
             * 
             * The shared secrets are randomly generated.  We RECOMMEND following
             * the recommendations in [RFC4086] and selecting a good and secure
             * random source for generating these secrets.  A (true) random
             * generator requires a naturally occurring source of randomness.
             * Practically, there are two possible avenues to consider for the
             * generation of the shared secrets:
             * 
             *    * Hardware-based generators: they exploit the randomness that
             * occurs in physical phenomena.  A nice implementation can be based on
             * oscillators and built in such ways that active attacks are more
             * difficult to perform.
             * 
             *    * Software-based generators: designing a good software random
             * generator is not an easy task.  A simple, but efficient,
             * implementation should be based on various sources and apply to the
             * sampled sequence a one-way function such as SHA-1.
             * 
             * We RECOMMEND selecting proven products, being hardware or software
             * generators, for the computation of shared secrets.
             */
            var keyArray = new byte[20];
            RandomNumberGenerator.Create().GetBytes(keyArray);
            var encodedKey = Base32Encoding.Standard.GetString(keyArray);
            return encodedKey;
        }
    }
}