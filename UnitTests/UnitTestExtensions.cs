using System;
using System.Linq;
using System.Text;
using Bogus;
using Shouldly;

namespace UnitTests
{
    public static class UnitTestExtensions
    {
        public static string SecretKey(this Faker faker, int secretKeyLength = 32)
        {
            var secretKeyBuilder = new StringBuilder();

            while (secretKeyBuilder.Length < secretKeyLength)
            {
                secretKeyBuilder.Append(faker.Lorem.Word());
            }

            return secretKeyBuilder
                .ToString()
                .Substring(0, secretKeyLength)
                .ToUpperInvariant();
        }

        /// <summary>
        /// Converts a hex string to a byte array
        /// </summary>
        /// <param name="hex">Source hex string.</param>
        /// <returns>Byte array</returns>
        /// <remarks>https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array</remarks>
        public static byte[] ToBytesFromHex(this string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <param name="data">Source byte array.</param>
        /// <returns>Hex string without dashes and converted to lowercase invariant.</returns>
        /// <remarks>https://stackoverflow.com/questions/623104/byte-to-hex-string</remarks>
        public static string ToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty).ToLowerInvariant();
        }

        public static void ShouldBe(this byte actual, byte expected)
        {
            ((int) actual).ShouldBe(expected);
        }

        public static void ShouldBe(this byte actual, byte expected, Func<string> customMessage)
        {
            ((int) actual).ShouldBe(expected, customMessage);
        }

        public static void ShouldBeGreaterThan(this byte actual, byte expected)
        {
            ((int) actual).ShouldBeGreaterThan(expected);
        }

        public static void ShouldBeGreaterThan(this byte actual, byte expected, Func<string> customMessage)
        {
            ((int) actual).ShouldBeGreaterThan(expected, customMessage);
        }
    }
}