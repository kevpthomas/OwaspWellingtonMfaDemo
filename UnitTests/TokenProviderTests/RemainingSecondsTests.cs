using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure

namespace UnitTests.TokenProviderTests
{
    [Trait("Category","Token Provider")]
    public class RemainingSecondsTests : UnitTestBase<TotpTokenBuilder>
    {
        private const int ThirtySeconds = 30;

        [Scenario]
        public void CurrentTimeAtBeginningOfTimeStep(int remainingSeconds)
        {
            $"Given a time step of {ThirtySeconds} seconds"
                .x(() => {});

            "And a current UTC Time at the beginning of a time step"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(UnixEpoch.AddSeconds(ThirtySeconds * Faker.Random.Int(1, 5000))));

            "When I calculate the remaining seconds in the current time interval"
                .x(() => remainingSeconds = TestInstance.RemainingSecondsInCurrentInterval());

            $"Then the remaining seconds should be {ThirtySeconds}"
                .x(() => remainingSeconds.ShouldBe(ThirtySeconds));
        }

        [Scenario]
        [Example(1)]
        [Example(15)]
        [Example(29)]
        public void CurrentTimeInMiddleOfTimeStep(int secondsIntoTimeStep, int remainingSeconds)
        {
            $"Given a time step of {ThirtySeconds} seconds"
                .x(() => {});

            $"And a current UTC Time {secondsIntoTimeStep} seconds into a time step"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(UnixEpoch.AddSeconds(ThirtySeconds * Faker.Random.Int(1, 5000) + secondsIntoTimeStep)));

            "When I calculate the remaining seconds in the current time interval"
                .x(() => remainingSeconds = TestInstance.RemainingSecondsInCurrentInterval());

            $"Then the remaining seconds should be {ThirtySeconds - secondsIntoTimeStep}"
                .x(() => remainingSeconds.ShouldBe(ThirtySeconds - secondsIntoTimeStep));
        }
    }
}