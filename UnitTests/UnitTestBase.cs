using System;
using Bogus;
using Moq;
using MultiFactorAuthentication;
using MultiFactorAuthentication.Abstractions;
using TinyIoC;
using Xbehave;

namespace UnitTests
{
    public abstract class UnitTestBase<TUnderTest> where TUnderTest : class 
    {
        // Appendix B of RFC 6238 https://tools.ietf.org/html/rfc6238#appendix-B
        protected const string Rfc6238SecretKeyEncoded = "12345678901234567890";
        protected readonly byte[] Rfc6238SecretKey = Rfc6238SecretKeyEncoded.ToBytesFromAscii();

        // https://tools.ietf.org/html/rfc4648
        protected const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        protected TinyIoCContainer Container;
        
        protected Mock<IDateTime> DateTime;

        protected DateTime UnixEpoch { get; } = new DateTimeAdapter().UnixEpoch;

        protected Faker Faker => new Faker();

        private TUnderTest _testInstance;
        protected TUnderTest TestInstance => _testInstance ?? (_testInstance = Container.Resolve<TUnderTest>());

        protected Mock<IDateTime> CreateMockDateTime()
        {
            var mockDateTime = new Mock<IDateTime>();
            mockDateTime.SetupGet(x => x.UnixEpoch).Returns(UnixEpoch);
            return mockDateTime;
        }

        [Background]
        public virtual void Setup()
        {
            _testInstance = null;

            Container = new TinyIoCContainer();
            Container.AutoRegister(DuplicateImplementationActions.RegisterSingle);

            Container.Register<ISecretKeyProvider, Base32SecretKeyProvider>();
            Container.Register<ITotpTokenProvider, ManualTotpTokenProvider>();

            DateTime = CreateMockDateTime();
            Container.Register((c,p) => DateTime.Object);
        }
    }
}