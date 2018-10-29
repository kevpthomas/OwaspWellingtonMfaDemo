using System.Linq;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable StringLiteralTypo

namespace UnitTests.TotpTokenGenerationTests
{
    /// <summary>
    /// Verifies HMAC-SHA-1 hashing as specified in RFC 2104 and RFC 2202.
    /// </summary>
    /// <remarks>
    /// References:
    /// https://tools.ietf.org/html/rfc2104
    /// https://tools.ietf.org/html/rfc2202
    /// </remarks>
    [Trait("Category","Generate Token")]
    public class Rfc2202SpecificationTestVectors : UnitTestBase<TotpTokenBuilder>
    {
        [Scenario]
        public void TestCase1(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 1"
                .x(() => key = "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b".ToBytesFromHex());

            "And data from test case 1"
                .x(() => data = "Hi There".ToBytesFromAscii());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0xb617318655057264e28bc0b6fb378c8ef146be00"
                .x(() => digest.ShouldBe("b617318655057264e28bc0b6fb378c8ef146be00"));
        }

        [Scenario]
        public void TestCase2(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 2"
                .x(() => key = "Jefe".ToBytesFromAscii());

            "And data from test case 2"
                .x(() => data = "what do ya want for nothing?".ToBytesFromAscii());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0xeffcdf6ae5eb2fa2d27416d5f184df9c259a7c79"
                .x(() => digest.ShouldBe("effcdf6ae5eb2fa2d27416d5f184df9c259a7c79"));
        }

        [Scenario]
        public void TestCase3(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 3"
                .x(() => key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa".ToBytesFromHex());

            "And data from test case 3"
                .x(() => data = string.Concat(Enumerable.Repeat("dd", 50)).ToBytesFromHex());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0x125d7342b9ac11cd91a39af48aa17b4f63f175d3"
                .x(() => digest.ShouldBe("125d7342b9ac11cd91a39af48aa17b4f63f175d3"));
        }

        [Scenario]
        public void TestCase4(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 4"
                .x(() => key = "0102030405060708090a0b0c0d0e0f10111213141516171819".ToBytesFromHex());

            "And data from test case 4"
                .x(() => data = string.Concat(Enumerable.Repeat("cd", 50)).ToBytesFromHex());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0x4c9007f4026250c6bc8414f9bf50c86c2d7235da"
                .x(() => digest.ShouldBe("4c9007f4026250c6bc8414f9bf50c86c2d7235da"));
        }

        [Scenario]
        public void TestCase5(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 5"
                .x(() => key = "0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c0c".ToBytesFromHex());

            "And data from test case 5"
                .x(() => data = "Test With Truncation".ToBytesFromAscii());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0x4c1a03424b55e07fe7f27be1d58bb9324a9a5a04"
                .x(() => digest.ShouldBe("4c1a03424b55e07fe7f27be1d58bb9324a9a5a04"));
        }

        [Scenario]
        public void TestCase6(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 6"
                .x(() => key = string.Concat(Enumerable.Repeat("aa", 80)).ToBytesFromHex());

            "And data from test case 6"
                .x(() => data = "Test Using Larger Than Block-Size Key - Hash Key First".ToBytesFromAscii());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0xaa4ae5e15272d00e95705637ce8a3b55ed402112"
                .x(() => digest.ShouldBe("aa4ae5e15272d00e95705637ce8a3b55ed402112"));
        }

        [Scenario]
        public void TestCase7(byte[] key, byte[] data, string digest)
        {
            "Given key from test case 7"
                .x(() => key = string.Concat(Enumerable.Repeat("aa", 80)).ToBytesFromHex());

            "And data from test case 7"
                .x(() => data = "Test Using Larger Than Block-Size Key and Larger Than One Block-Size Data".ToBytesFromAscii());

            "When I compute an HMAC-SHA-1 hash"
                .x(x => digest = TestInstance.GenerateSha1Hash(key, data).ToHexString());

            "Then the digest should be 0xe8e99d0f45237d786d6bbaa7965c7808bbff1a91"
                .x(() => digest.ShouldBe("e8e99d0f45237d786d6bbaa7965c7808bbff1a91"));
        }
    }
}