using System.Collections.Generic;
using System.Linq;
using MultiFactorAuthentication;
using Shouldly;
using Xbehave;
using Xunit;

// ReSharper disable ImplicitlyCapturedClosure
// ReSharper disable RedundantAssignment

namespace UnitTests.Base32SecretKeyProviderTests
{
    [Trait("Category","HMAC Secret Key")]
    public class HmacSecretKeyBase32Tests : UnitTestBase<Base32SecretKeyProvider>
    {
        public HmacSecretKeyBase32Tests()
        {
            Faker.Random.Int(1, 20);
        }

        [Scenario]
        public void KeysAreRandomBase32(string encodedKey)
        {
            var base32AlphabetBytes = Base32Alphabet.ToBytesFromAscii();
            var numberOfKeys = Faker.Random.Int(10, 200);

            var keysWithPossibleDuplicates = new List<string>();
            var keysWithNoDuplicates = new HashSet<string>();

            $"When I create many ({numberOfKeys}) keys"
                .x(() =>
                {
                    for (var i = 0; i < numberOfKeys; i++)
                    {
                        encodedKey = TestInstance.CreateKey();
                        keysWithPossibleDuplicates.Add(encodedKey);
                        keysWithNoDuplicates.Add(encodedKey);
                    }
                });

            "Then encoded keys are random and unique"
                .x(() => keysWithPossibleDuplicates.Count.ShouldBe(keysWithNoDuplicates.Count));

            "And the encoded keys are Base32"
                .x(() => keysWithNoDuplicates.First().ToBytesFromAscii().ShouldAllBe(x => base32AlphabetBytes.Contains(x)));

            /*
             * There is no easy way to test for sufficient randomness as specified in
             * RFC 4086 https://tools.ietf.org/html/rfc4086.
             *
             * Trust the output from System.Security.Cryptography.RandomNumberGenerator
             */
        }
    }
}