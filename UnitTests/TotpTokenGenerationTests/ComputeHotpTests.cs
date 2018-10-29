using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace UnitTests.TotpTokenGenerationTests
{
    [Trait("Category","Generate Token")]
    public class ComputeHotpTests : UnitTestBase<TotpTokenBuilder>
    {
        [Scenario]
        public void HotpFromLongHash(long binCode, int digitCount, string hotp, long expectedHotp)
        {
            "Given a desired HOTP length"
                .x(() => digitCount = 6);

            "And a truncated hash containing more digits than the desired HOTP length"
                .x(() =>
                {
                    // 2147483647 is the largest possible truncated hash value
                    expectedHotp = Faker.Random.Long(111111, 483647);
                    binCode = long.Parse($"{Faker.Random.Long(1, 2147)}{expectedHotp}");
                });

            "When HOTP value is computed"
                .x(() => hotp = TestInstance.ExtractHotp(binCode, digitCount));

            "Then the HOTP is the least significant 6 digits"
                .x(() => hotp.ShouldBe($"{expectedHotp}"));
        }

        [Scenario]
        public void HotpFromMatchingHash(long binCode, int digitCount, string hotp, long expectedHotp)
        {
            "Given a desired HOTP length"
                .x(() => digitCount = 6);

            "And a truncated hash containing the same digits as the desired HOTP length"
                .x(() => binCode = expectedHotp = Faker.Random.Long(111111, 999999));

            "When HOTP value is computed"
                .x(() => hotp = TestInstance.ExtractHotp(binCode, digitCount));

            "Then the HOTP is the least significant 6 digits"
                .x(() => hotp.ShouldBe($"{expectedHotp}"));
        }

        [Scenario]
        [Example(1, 9, "00000")]
        [Example(11, 99, "0000")]
        [Example(111, 999, "000")]
        [Example(1111, 9999, "00")]
        [Example(11111, 99999, "0")]
        public void HotpFromShortHash(long hashMinimum, long hashMaximum, string expectedPadding,
            long binCode, int digitCount, string hotp, long expectedHotp)
        {
            "Given a desired HOTP length"
                .x(() => digitCount = 6);

            "And a truncated hash containing fewer digits than the desired HOTP length"
                .x(() => binCode = expectedHotp = Faker.Random.Long(hashMinimum, hashMaximum));

            "When HOTP value is computed"
                .x(() => hotp = TestInstance.ExtractHotp(binCode, digitCount));

            "Then the HOTP is the least significant 6 digits"
                .x(() => hotp.ShouldBe($"{expectedPadding}{expectedHotp}"));
        }
    }
}