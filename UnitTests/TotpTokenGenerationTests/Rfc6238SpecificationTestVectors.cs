using System;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure

namespace UnitTests.TotpTokenGenerationTests
{
    /// <summary>
    /// Verifies the correct manual computation of TOTP tokens as specified
    /// in RFC 6238. Verification uses the Test Vectors described in
    /// Appendix B of RFC 6238.
    /// https://tools.ietf.org/html/rfc6238#appendix-B
    /// </summary>
    [Trait("Category","OWASP Demo")]
    [Trait("Category","Generate Token")]
    public class Rfc6238SpecificationTestVectors : UnitTestBase<ManualTotpTokenProvider>
    {
        [Scenario]
        public void Compute1970Token(byte[] secretKey, string token)
        {
            "Given UTC Time 1970-01-01 00:00:59"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(1970, 1, 1, 0, 0, 59, DateTimeKind.Utc)));

            $"And the RFC 6238 test token shared secret {Rfc6238SecretKeyEncoded}"
                .x(() => secretKey = Rfc6238SecretKey);

            "When I generate a TOTP token from the secret key"
                .x(x => token = TestInstance.ComputeToken(secretKey));

            "Then the TOTP 6-digit token should be 287082"
                .x(() => token.ShouldBe("287082"));
        }

        [Scenario]
        public void Compute2005Token(byte[] secretKey, string token)
        {
            "Given UTC Time 2005-03-18 01:58:29"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(2005, 3, 18, 1, 58, 29, DateTimeKind.Utc)));

            $"And the RFC 6238 test token shared secret {Rfc6238SecretKeyEncoded}"
                .x(() => secretKey = Rfc6238SecretKey);

            "When I generate a TOTP token from the secret key"
                .x(x => token = TestInstance.ComputeToken(secretKey));

            "Then the TOTP 6-digit token should be 081804"
                .x(() => token.ShouldBe("081804"));
        }

        [Scenario]
        public void Compute2009Token(byte[] secretKey, string token)
        {
            "Given UTC Time 2009-02-13 23:31:30"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(2009, 2, 13, 23, 31, 30, DateTimeKind.Utc)));

            $"And the RFC 6238 test token shared secret {Rfc6238SecretKeyEncoded}"
                .x(() => secretKey = Rfc6238SecretKey);

            "When I generate a TOTP token from the secret key"
                .x(x => token = TestInstance.ComputeToken(secretKey));

            "Then the TOTP 6-digit token should be 005924"
                .x(() => token.ShouldBe("005924"));
        }

        [Scenario]
        public void Compute2033Token(byte[] secretKey, string token)
        {
            "Given UTC Time 2033-05-18 03:33:20"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(2033, 5, 18, 3, 33, 20, DateTimeKind.Utc)));

            $"And the RFC 6238 test token shared secret {Rfc6238SecretKeyEncoded}"
                .x(() => secretKey = Rfc6238SecretKey);

            "When I generate a TOTP token from the secret key"
                .x(x => token = TestInstance.ComputeToken(secretKey));

            "Then the TOTP 6-digit token should be 279037"
                .x(() => token.ShouldBe("279037"));
        }

        [Scenario]
        public void Compute2063Token(byte[] secretKey, string token)
        {
            "Given UTC Time 2603-10-11 11:33:20"
                .x(() => DateTime.SetupGet(x => x.UtcNow)
                    .Returns(new DateTime(2603, 10, 11, 11, 33, 20, DateTimeKind.Utc)));

            $"And the RFC 6238 test token shared secret {Rfc6238SecretKeyEncoded}"
                .x(() => secretKey = Rfc6238SecretKey);

            "When I generate a TOTP token from the secret key"
                .x(x => token = TestInstance.ComputeToken(secretKey));

            "Then the TOTP 6-digit token should be 353130"
                .x(() => token.ShouldBe("353130"));
        }
    }
}