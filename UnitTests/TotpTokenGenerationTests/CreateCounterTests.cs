using System;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable ImplicitlyCapturedClosure

namespace UnitTests.TotpTokenGenerationTests
{
    [Trait("Category","Generate Token")]
    public class CreateCounterTests : UnitTestBase<TotpTokenBuilder>
    {
        [Scenario]
        public void ConfirmUnixEpoch(TotpTokenBuilder testInstance, DateTime unixEpoch)
        {
            "Given a TOTP current time calculator"
                .x(() => testInstance = TestInstance);

            "When I get the Unix Epoch for a current time calculation"
                .x(() => unixEpoch = testInstance.UnixEpoch);

            "Then the current time is calculated from midnight UTC of January 1, 1970 as specified in RFC 6238"
                .x(() => unixEpoch.ShouldSatisfyAllConditions(
                    () => unixEpoch.Year.ShouldBe(1970),
                    () => unixEpoch.Month.ShouldBe(1),
                    () => unixEpoch.Day.ShouldBe(1),
                    () => unixEpoch.Hour.ShouldBe(0),
                    () => unixEpoch.Minute.ShouldBe(0),
                    () => unixEpoch.Second.ShouldBe(0)
                ));
        }

        [Scenario]
        public void ConfirmBigEndianTransformation(int timeStep, byte[] timeCounter)
        {
            "Given a time step of 30 seconds"
                .x(() => timeStep = 30);

            "And a current UTC Time at the second time step"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(1970, 1, 1, 0, 1, 0, DateTimeKind.Utc)));

            "When I generate a TOTP time counter"
                .x(() => timeCounter = TestInstance.CreateCounter(timeStep));

            "Then an 8-byte counter is generated as specified in RFC 4226"
                .x(() => timeCounter.Length.ShouldBe(8));

            "And the counter is big-endian as specified in RFC 4226"
                .x(() => timeCounter.ShouldSatisfyAllConditions(
                    () => timeCounter[0].ShouldBe(0),
                    () => timeCounter[1].ShouldBe(0),
                    () => timeCounter[2].ShouldBe(0),
                    () => timeCounter[3].ShouldBe(0),
                    () => timeCounter[4].ShouldBe(0),
                    () => timeCounter[5].ShouldBe(0),
                    () => timeCounter[6].ShouldBe(0),
                    () => timeCounter[7].ShouldBeGreaterThan(0)
                ));
        }

        [Scenario]
        [Example(0, 30, 1)]
        [Example(0, 59, 1)]
        [Example(1, 0, 2)]
        [Example(1, 29, 2)]
        [Example(1, 30, 3)]
        [Example(1, 59, 3)]
        public void ComputeTimeCounter(int unixMinutes, int unixSeconds, long expectedTimeCounter,
            int timeStep, byte[] timeCounter)
        {
            "Given UTC Time"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(1970, 1, 1, 0, unixMinutes, unixSeconds, DateTimeKind.Utc)));

            "And a time step of 30 seconds"
                .x(() => timeStep = 30);

            "When I generate a TOTP time counter"
                .x(() => timeCounter = TestInstance.CreateCounter(timeStep));

            "Then the time counter should be in an increment of the time step as specified in RFC 6238"
                .x(() => timeCounter[7].ShouldBe((byte)expectedTimeCounter));
        }

        [Scenario]
        [Example(2039)]
        [Example(2100)]
        public void ComputeTimeCounterBeyondYear2038(int year, int timeStep, Exception ex)
        {
            "Given UTC Time beyond the year 2038"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            "And a random valid time step"
                .x(() => timeStep = Faker.Random.Int(15, 60));

            "When I generate a TOTP time counter"
                .x(x => ex = Record.Exception(() => TestInstance.CreateCounter(timeStep)));

            "Then a time counter is created without error as specified in RFC 6238"
                .x(() => ex.ShouldBeNull());
        }
    }
}