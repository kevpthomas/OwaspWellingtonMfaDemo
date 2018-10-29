using System;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable RedundantAssignment

namespace UnitTests.Base32SecretKeyProviderTests
{
    [Trait("Category","HMAC Secret Key")]
    public class HmacSecretKeyStrengthTests : UnitTestBase<Base32SecretKeyProvider>
    {
        private readonly int _hmacKeyByteCount;

        public HmacSecretKeyStrengthTests()
        {
            _hmacKeyByteCount = Faker.Random.Int(1, 20);
        }

        [Scenario]
        [Example(1)]
        [Example(31)]
        public void KeyIsInvalidLength(int invalidLength, string encodedKey, Exception ex)
        {
            "Given a key of insufficient length for Base32 decoding"
                .x(() => encodedKey = Faker.SecretKey().Substring(0, invalidLength));

            "When I attempt to decode the key"
                .x(() => ex = Record.Exception(() => TestInstance.DecodeKey(encodedKey)));

            "Then an invalid length exception is thrown"
                .x(() => ex.ShouldSatisfyAllConditions(
                    () => ex.ShouldNotBeNull(),
                    () => ex.ShouldBeOfType<FormatException>(),
                    () => ex.Message.ShouldBe("Invalid length"))
                );
        }

        [Scenario]
        [Example('0')]
        [Example('1')]
        [Example('8')]
        [Example('9')]
        [Example('a')]
        public void KeyContainsInvalidCharacter(char invalidChar, string encodedKey, Exception ex)
        {
            "Given a key containing invalid characters for Base32 decoding"
                .x(() => encodedKey = new string(invalidChar, 32));

            "When I attempt to decode the key"
                .x(() => ex = Record.Exception(() => TestInstance.DecodeKey(encodedKey)));

            "Then an invalid character exception is thrown"
                .x(() => ex.ShouldSatisfyAllConditions(
                    () => ex.ShouldNotBeNull(),
                    () => ex.ShouldBeOfType<FormatException>(),
                    () => ex.Message.ShouldBe("Invalid character"))
                );
        }

        [Scenario]
        public void KeyIsRecommendedStrengthWhenEncoded(string encodedKey)
        {
            "When I create a secret key"
                .x(() => encodedKey = TestInstance.CreateKey());

            "Then the encoded key meets RFC 4226 shared secret recommended length requirement of 160 bits (20 bytes)"
                .x(() => encodedKey.ShouldSatisfyAllConditions(
                    () => encodedKey.Length.ShouldBe(32),
                    () => TestInstance.DecodeKey(encodedKey).Length.ShouldBe(20)
                ));
        }

        [Scenario]
        public void KeyIsRecommendedLengthWhenEncoded(string encodedKey, byte[] hmacHash)
        {
            "When I create a secret key"
                .x(() => encodedKey = TestInstance.CreateKey());

            "And an HMAC hash"
                .x(() => hmacHash = Container.Resolve<TotpTokenBuilder>().GenerateSha1Hash(
                    Faker.Random.Bytes(_hmacKeyByteCount),
                    BitConverter.GetBytes(Faker.Random.Long(1500000000, 1600000000))));

            "Then the secret key is the length of the HMAC output as specified in RFC 6238"
                .x(() => TestInstance.DecodeKey(encodedKey).Length.ShouldBe(hmacHash.Length));
        }
    }
}