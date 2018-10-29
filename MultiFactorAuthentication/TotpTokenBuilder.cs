using System;
using System.Security.Cryptography;
using MultiFactorAuthentication.Abstractions;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace MultiFactorAuthentication
{
    public class TotpTokenBuilder : ITotpTokenBuilder
    {    
        private readonly IDateTime _dateTime;

        public TotpTokenBuilder(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public DateTime CurrentUnixTime => _dateTime.UtcNow;

        public DateTime UnixEpoch => _dateTime.UnixEpoch;

        public byte[] CreateCounter(int stepSeconds = 30, int stepOffset = 0)
        {
            var totpCounter = CreateTotpCounter(stepSeconds) + stepOffset;

            var totpBytes = GetBigEndianBytes(totpCounter);

            return totpBytes;
        }

        public byte[] GenerateSha1Hash(byte[] secretKey, byte[] timeCounter)
        {
            /*
             * HMAC: Keyed-Hashing for Message Authentication
             * https://tools.ietf.org/html/rfc2104
             *
             * The key for HMAC can be of any length (keys longer than B bytes are
             * first hashed using H).  However, less than L bytes is strongly
             * discouraged as it would decrease the security strength of the
             * function.  Keys longer than L bytes are acceptable but the extra
             * length would not significantly increase the function strength. (A
             * longer key may be advisable if the randomness of the key is
             * considered weak.)
             * 
             * Keys need to be chosen at random (or using a cryptographically strong
             * pseudo-random generator seeded with a random seed), and periodically
             * refreshed.  (Current attacks do not indicate a specific recommended
             * frequency for key changes as these attacks are practically
             * infeasible.  However, periodic key refreshment is a fundamental
             * security practice that helps against potential weaknesses of the
             * function and keys, and limits the damage of an exposed key.)
             */

            byte[] sha1Hash;

            using (var hmacHashGenerator = (HMAC) new HMACSHA1())
            {
                hmacHashGenerator.Key = secretKey;
                sha1Hash = hmacHashGenerator.ComputeHash(timeCounter);
            }

            return sha1Hash;
        }

        public int ComputeOffset(byte[] hmacResult)
        {
            /*
             * FROM RFC 4226:        int offset = hmac_result[19] & 0xf;
             * this step will always evaluate to a number between 0 and 15
             *
             * & (binary AND) operator references:
             * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/and-operator
             * https://www.tutorialspoint.com/csharp/csharp_bitwise_operators.htm
             */

            return hmacResult[hmacResult.Length - 1] & 15;  // 15 is 1111 in binary, 0xf in hexadecimal
        }

        public int TruncateHash(byte[] hmacResult, int offset)
        {
            /*
             * FROM RFC 4226: perform a dynamic truncation by generating a 4-byte string,
             * and then convert that string to a number in 0...2^{31}-1.
             *
             * int bin_code = (hmac_result[offset] & 0x7f) << 24
             *     | (hmac_result[offset + 1] & 0xff) << 16
             *     | (hmac_result[offset + 2] & 0xff) <<  8
             *     | (hmac_result[offset + 3] & 0xff);
             *
             * << (left shift) operator references:
             * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/left-shift-operator
             * https://stackoverflow.com/questions/141525/what-are-bitwise-shift-bit-shift-operators-and-how-do-they-work
             *
             * | (binary OR) operator references:
             * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/or-operator
             * https://www.tutorialspoint.com/csharp/csharp_bitwise_operators.htm
             */

            // sbyte.MaxValue = 127 = 0x7f
            // byte.MaxValue = 255 = 0xff
            var binCode = (hmacResult[offset] & sbyte.MaxValue) << 24            
                                | (hmacResult[offset + 1] & byte.MaxValue) << 16
                                | (hmacResult[offset + 2] & byte.MaxValue) << 8
                                | (hmacResult[offset + 3] & byte.MaxValue);

            return binCode;
        }

        public string ExtractHotp(long binCode, int digitCount)
        {
            /*
             * Extracts the least-significant digits from the hash
             * and then left-pads with 0's.
             *
             * We then take this bin_code number modulo 1,000,000 (10^6) to generate
             * the 6-digit HOTP value decimal.
             *
             * % (modulo) operator references:
             * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/remainder-operator
             * https://www.computerhope.com/jargon/m/modulo.htm
             * https://www.khanacademy.org/computing/computer-science/cryptography/modarithmetic/a/what-is-modular-arithmetic
             */

            return ((int) binCode % (int) Math.Pow(10, digitCount))
                .ToString()
                .PadLeft(digitCount, '0');
        }


        private long CreateTotpCounter(int stepSeconds = 30)
        {
            /*
             * 1 tick = one ten-millionth of a second
             * https://docs.microsoft.com/en-us/dotnet/api/system.datetime.ticks?view=netframework-4.7.2
             *
             *
             * TOTP Time Counter formula is
             * https://en.wikipedia.org/wiki/Time-based_One-time_Password_algorithm
             *
             *        T - T0
             * Ct  =  -------
             *           Tx
             *
             * Ct = timeCounter
             * T  = CurrentUnixTime
             * T0 = UnixEpoch
             * Tx = numberOfDurations
             */

            var unixDifferenceSeconds = (CurrentUnixTime - UnixEpoch).TotalSeconds;

            var timeCounter = unixDifferenceSeconds / stepSeconds;

            // C# truncates the timeCounter value when casting to long
            return (long)timeCounter;
        }

        private static byte[] GetBigEndianBytes(long timeCounter)
        {
            /*
             * The HOTP values generated by the HOTP generator are treated as big endian.
             * https://tools.ietf.org/html/rfc4226#section-5.2
             *
             * OtpSharp.KeyUtilities.GetBigEndianBytes
             * https://en.wikipedia.org/wiki/Endianness
             * https://referencesource.microsoft.com/#mscorlib/system/bitconverter.cs
             * Converts the Time Counter into a byte array.
             * BitConverter class by default converts to little-endian format, so there is a manual
             * conversion from little-endian to big-endian by reversing the byte array sequence.
             *
             * In big-endian format, whenever addressing memory or sending/storing words bytewise,
             * the most significant byte—the byte containing the most significant bit—is stored first
             * (has the lowest address)
             *
             * The least-significant byte is on the right-hand side because if you change it, the change
             * has the least effect on the overall number
             * https://stackoverflow.com/questions/16535335/what-does-least-significant-byte-mean
             */

            byte[] bytes = BitConverter.GetBytes(timeCounter);
            Array.Reverse(bytes);
            return bytes;
        }

        public int RemainingSecondsInCurrentInterval(int stepSeconds = 30)
        {
            var unixDifferenceSeconds = (CurrentUnixTime - UnixEpoch).Seconds;
            var timeCounterSeconds = ((double)unixDifferenceSeconds / stepSeconds);
            var offsetSeconds = (int)Math.Round((1 - timeCounterSeconds) * stepSeconds, 0, MidpointRounding.AwayFromZero);
            var remainingSeconds = offsetSeconds <= 0 ? stepSeconds + offsetSeconds : offsetSeconds;
            return remainingSeconds;
        }
    }
}