using MultiFactorAuthentication;
using MultiFactorAuthentication.Abstractions;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace UnitTests.TotpTokenVerificationTests
{
    [Trait("Category","OWASP Demo")]
    [Trait("Category","Verify Token")]
    public class ClockDriftTests : UnitTestBase<ManualTotpTokenValidator>
    {
        private const int TimeStepSeconds = 30;

        private const int ClockDriftSteps = 2;

        [Scenario]
        public void VerifyTokenWithSynchronisedClocks(string secretKey, string token, bool isValid)
        {
            var clientTokenProvider = Container.Resolve<ManualTotpTokenProvider>();

            "Given a random UTC Time"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(Faker.Date.Between(System.DateTime.UtcNow, System.DateTime.UtcNow.AddYears(10))));

            "And a valid secret key"
                .x(() => secretKey = Faker.SecretKey());

            "When I generate a client TOTP token from the secret key"
                .x(() => token = clientTokenProvider.ComputeToken(secretKey));

            "And I verify the token against a synchronised verification server"
                .x(() => isValid = TestInstance.VerifyToken(secretKey, token));

            "Then the token should be valid"
                .x(() => isValid.ShouldBe(true));
        }

        [Scenario]
        public void VerifyTokenWithUncorrectedClockDrift(string secretKey, string token, bool isValid)
        {
            var clockDriftDateTime = CreateMockDateTime();

            var validationServerUtcTime =
                Faker.Date.Between(System.DateTime.UtcNow, System.DateTime.UtcNow.AddYears(10));

            var clientTokenProvider = new ManualTotpTokenProvider(Container.Resolve<ISecretKeyProvider>(),
                new TotpTokenBuilder(clockDriftDateTime.Object));

            "Given a random verification server UTC Time"
                .x(() => DateTime.SetupGet(x => x.UtcNow).Returns(validationServerUtcTime));

            $"And client clock drift greater than the {TimeStepSeconds} second time step"
                .x(() => clockDriftDateTime.SetupGet(x => x.UtcNow).Returns(validationServerUtcTime.AddSeconds(TimeStepSeconds * Faker.Random.Int(2, 10))));

            "And a valid secret key"
                .x(() => secretKey = Faker.SecretKey());

            "When I generate a client TOTP token from the secret key"
                .x(() => token = clientTokenProvider.ComputeToken(secretKey));

            "And I verify the token against a non-synchronised verification server"
                .x(() => isValid = TestInstance.VerifyToken(secretKey, token, TimeStepSeconds));

            "Then the token should be invalid"
                .x(() => isValid.ShouldBe(false));
        }

        [Scenario]
        public void VerifyTokenWithCorrectedClockDrift(string secretKey, string token, bool isValid)
        {
            var clockDriftDateTime = CreateMockDateTime();

            var validationServerUtcTime =
                Faker.Date.Between(System.DateTime.UtcNow, System.DateTime.UtcNow.AddYears(10));

            var clientTokenProvider = new ManualTotpTokenProvider(Container.Resolve<ISecretKeyProvider>(),
                new TotpTokenBuilder(clockDriftDateTime.Object));

            "Given a random verification server UTC Time"
                .x(() => DateTime.SetupGet(x => x.UtcNow).Returns(validationServerUtcTime));

            $"And client clock drift greater than the {TimeStepSeconds} second time step"
                .x(() => clockDriftDateTime.SetupGet(x => x.UtcNow).Returns(validationServerUtcTime.AddSeconds(TimeStepSeconds * ClockDriftSteps)));

            "And a valid secret key"
                .x(() => secretKey = Faker.SecretKey());

            "When I generate a client TOTP token from the secret key"
                .x(() => token = clientTokenProvider.ComputeToken(secretKey));

            "And I verify the token against a verification server with clock drift threshold applied"
                .x(() => isValid = TestInstance.VerifyToken(secretKey, token, TimeStepSeconds, future: ClockDriftSteps));

            "Then the token should be valid"
                .x(() => isValid.ShouldBe(true));
        }
    }
}