using System.Linq;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure

namespace UnitTests.TotpTokenGenerationTests
{
    [Trait("Category","Generate Token")]
    public class OffsetTests : UnitTestBase<TotpTokenBuilder>
    {
        [Scenario]
        public void OffsetFromLeastSignificantByte(byte[] hmacResult, int offset)
        {
            var offsetByte = Faker.Random.Byte(1, 14);

            var sha1HashList = Enumerable.Repeat((byte)0, 19).ToList();
            sha1HashList.Add(offsetByte);

            "Given a valid SHA1 encrypted hash"
                .x(() => hmacResult = sha1HashList.ToArray());

            "When the offset is calculated"
                .x(() => offset = TestInstance.ComputeOffset(hmacResult));

            "Then the offset was calculated from the least significant byte"
                .x(() => offset.ShouldBe(offsetByte & 0xf));
        }

        [Scenario]
        public void MinimumOffset(byte[] hmacResult, int offset)
        {
            var sha1HashList = Enumerable.Repeat((byte)0, 19).ToList();
            sha1HashList.Add(byte.MinValue);

            "Given a valid SHA1 encrypted hash"
                .x(() => hmacResult = sha1HashList.ToArray());

            "When the offset is calculated from the minimum byte 00000000"
                .x(() => offset = TestInstance.ComputeOffset(hmacResult));

            "Then the offset is 0"
                .x(() => offset.ShouldBe(0));
        }

        [Scenario]
        public void MaximumOffset(byte[] hmacResult, int offset)
        {
            var sha1HashList = Enumerable.Repeat((byte)0, 19).ToList();
            sha1HashList.Add(byte.MaxValue);

            "Given a valid SHA1 encrypted hash"
                .x(() => hmacResult = sha1HashList.ToArray());

            "When the offset is calculated from the maximum byte 11111111"
                .x(() => offset = TestInstance.ComputeOffset(hmacResult));

            "Then the offset is 15"
                .x(() => offset.ShouldBe(15));
        }
    }
}