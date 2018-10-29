using System.Threading.Tasks;
using MultiFactorAuthentication.Abstractions;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Identity
{
    public class ManualUserTwoFactorTokenProvider : IUserTwoFactorTokenProvider
    {
        private readonly ITotpTokenProvider _totpTokenProvider;
        private readonly ITotpTokenValidator _totpTokenValidator;

        public ManualUserTwoFactorTokenProvider(ITotpTokenProvider totpTokenProvider, 
            ITotpTokenValidator totpTokenValidator)
        {
            _totpTokenProvider = totpTokenProvider;
            _totpTokenValidator = totpTokenValidator;
        }

        public async Task<string> GenerateAsync(IUserManager manager, UserData user)
        {
            var secretKey = await manager.GetAuthenticatorKeyAsync(user);
            return _totpTokenProvider.ComputeToken(secretKey);
        }

        public async Task<bool> ValidateAsync(string token, IUserManager manager, UserData user)
        {
            var secretKey = await manager.GetAuthenticatorKeyAsync(user);
            return _totpTokenValidator.VerifyToken(secretKey, token);
        }
    }
}