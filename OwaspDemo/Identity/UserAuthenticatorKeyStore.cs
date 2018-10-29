using System.Threading.Tasks;
using MultiFactorAuthentication.Abstractions;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

// ReSharper disable IdentifierTypo

namespace OwaspDemo.Identity
{
    public class UserAuthenticatorKeyStore : IUserAuthenticatorKeyStore
    {
        private readonly ICacheContext _cacheContext;
        private readonly ISecretKeyEncrypter _encrypter;

        public UserAuthenticatorKeyStore(ICacheContext cacheContext, ISecretKeyEncrypter encrypter)
        {
            _cacheContext = cacheContext;
            _encrypter = encrypter;
        }

        public async Task<string> GetAuthenticatorKeyAsync(UserData user)
        {
            var token = await _cacheContext.FindTokenByUserIdAsync(user.Id);
            return token != null ? _encrypter.DecryptString(token.Value, user.KeyPassword, user.KeySalt) : string.Empty;
        }

        public async Task SetAuthenticatorKeyAsync(UserData user, string key)
        {
            await _cacheContext.AddAsync(user.Id, _encrypter.EncryptString(key, user.KeyPassword, user.KeySalt));
        }
    }
}