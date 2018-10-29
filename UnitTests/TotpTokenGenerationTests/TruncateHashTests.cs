using System;
using System.Collections.Generic;
using System.Linq;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure

namespace UnitTests.TotpTokenGenerationTests
{
    [Trait("Category","Generate Token")]
    public class TruncateHashTests : UnitTestBase<TotpTokenBuilder>
    {
        private readonly int _offset;

        public TruncateHashTests()
        {
            _offset = Faker.Random.Int(0, 15);
        }

        [Scenario]
        public void OffsetFromLeastSignificantByte(byte[] sha1Hash, int truncatedHash)
        {
            var hmacResult = CalculateHmacResult();
            var expected = CalculateExpectedValue(hmacResult);

            "Given a valid SHA1 encrypted hash"
                .x(() => sha1Hash = hmacResult);

            "When the hash is truncated"
                .x(() => truncatedHash = TestInstance.TruncateHash(sha1Hash, _offset));

            "Then the truncated value was calculated by combining the 4 consecutive bytes starting at the offset and moving towards the least significant byte"
                .x(() => (truncatedHash - expected[0]).ShouldBe(expected[1] + expected[2] + expected[3]));
        }

        [Scenario]
        public void MinimumTruncatedValue(byte[] sha1Hash, int truncatedHash)
        {
            "Given a SHA1 encrypted hash containing the smallest possible byte values"
                .x(() => sha1Hash = Enumerable.Repeat(byte.MinValue, 20).ToArray());

            "When the hash is truncated"
                .x(() => truncatedHash = TestInstance.TruncateHash(sha1Hash, _offset));

            "Then the smallest possible truncated value is 0"
                .x(() => truncatedHash.ShouldBe(0));
        }

        [Scenario]
        public void MaximumTruncatedValue(byte[] sha1Hash, int truncatedHash)
        {
            "Given a SHA1 encrypted hash containing the largest possible byte values"
                .x(() => sha1Hash = Enumerable.Repeat(byte.MaxValue, 20).ToArray());

            "When the hash is truncated"
                .x(() => truncatedHash = TestInstance.TruncateHash(sha1Hash, _offset));

            "Then the largest possible truncated value is 2,147,483,647"
                .x(() => truncatedHash.ShouldBe(2147483647));
        }

        private byte[] CalculateHmacResult()
        {
            var hmacResult = Enumerable.Repeat(byte.MinValue, 20).ToArray();
            hmacResult[_offset] = Faker.Random.Byte();
            hmacResult[_offset + 1] = Faker.Random.Byte();
            hmacResult[_offset + 2] = Faker.Random.Byte();
            hmacResult[_offset + 3] = Faker.Random.Byte();

            return hmacResult;
        }

        private int[] CalculateExpectedValue(byte[] hmacResult)
        {
            return new List<int>
            {
                (hmacResult[_offset] & sbyte.MaxValue) * (int) Math.Pow(2, 24),
                (hmacResult[_offset + 1] & byte.MaxValue) * (int) Math.Pow(2, 16),
                (hmacResult[_offset + 2] & byte.MaxValue) * (int) Math.Pow(2, 8),
                (hmacResult[_offset + 3] & byte.MaxValue)
            }.ToArray();
        }
    }
}