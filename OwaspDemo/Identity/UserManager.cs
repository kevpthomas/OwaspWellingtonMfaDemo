using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MultiFactorAuthentication.Abstractions;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Identity
{
    /// <summary>
    /// Simple User Manager meant to emulate Microsoft.AspNetCore.Identity.UserManager
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly IMemoryContext _memoryContext;
        private readonly IUserAuthenticatorKeyStore _authenticatorKeyStore;
        private readonly IUserTwoFactorTokenProvider _twoFactorTokenProvider;
        private readonly ISecretKeyProvider _secretKeyProvider;
        private readonly IPasswordHasher<UserData> _passwordHasher;
        private readonly IDateTime _dateTime;
        private readonly IConfiguration _configuration;

        public UserManager(IMemoryContext memoryContext,
            IUserAuthenticatorKeyStore authenticatorKeyStore,
            IUserTwoFactorTokenProvider twoFactorTokenProvider, 
            ISecretKeyProvider secretKeyProvider, 
            IPasswordHasher<UserData> passwordHasher,
            IDateTime dateTime,
            IConfiguration configuration)
        {
            _memoryContext = memoryContext;
            _authenticatorKeyStore = authenticatorKeyStore;
            _twoFactorTokenProvider = twoFactorTokenProvider;
            _secretKeyProvider = secretKeyProvider;
            _passwordHasher = passwordHasher;
            _dateTime = dateTime;
            _configuration = configuration;
        }

        public async Task<IdentityResult> CreateAsync(string user, string password)
        {
            await _memoryContext.AddUserAsync(new UserData
            {
                Id = user,
                UserName = user,
                PasswordHash = _passwordHasher.HashPassword(null, password)
            });

            var registeredUser = await _memoryContext.FindUserByNameAsync(user);

            return registeredUser != null
                ? IdentityResult.Success
                : IdentityResult.Failed(Enumerable.Empty<IdentityError>().ToArray());
        }

        public async Task<UserData> FindByNameAsync(string userName)
        {
            return await _memoryContext.FindUserByNameAsync(userName);
        }

        public Task<UserData> GetUserAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof (principal));

            var userId = GetUserId(principal);

            if (userId != null)
                return FindByNameAsync(userId);

            return Task.FromResult(default (UserData));
        }

        public string GetUserId(ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof (principal));
            return principal.Identity.Name;
        }

        public async Task<bool> CheckPasswordAsync(UserData user, string password)
        {
            if (user == null) return false;

            var isVerified = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, password) != PasswordVerificationResult.Failed;

            if (!isVerified)
            {
                await UpdateAccessFailedCount(user);
            }

            return isVerified;
        }

        public async Task<bool> GetTwoFactorEnabledAsync(UserData user)
        {
            return await Task.Run(() => user.TwoFactorEnabled);
        }

        public async Task<string> GetAuthenticatorKeyAsync(UserData user)
        {
            return await _authenticatorKeyStore.GetAuthenticatorKeyAsync(user);
        }

        public async Task<IdentityResult> ResetAuthenticatorKeyAsync(UserData user)
        {
            var key = _secretKeyProvider.CreateKey();
            await _authenticatorKeyStore.SetAuthenticatorKeyAsync(user, key);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> SetTwoFactorEnabledAsync(UserData user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            await _memoryContext.UpdateUserAsync(user);
            return IdentityResult.Success;
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(UserData user, string token)
        {
            var isVerified = await _twoFactorTokenProvider.ValidateAsync(token, this, user);

            if (!isVerified)
            {
                await UpdateAccessFailedCount(user);
            }

            return isVerified;
        }

        public async Task<bool> IsLockedOutAsync(UserData user)
        {
            return await Task.Run(() => user.LockoutEnd > _dateTime.UtcNow);
        }

        public async Task<IdentityResult> ResetAccessFailedCountAsync(UserData user)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = DateTime.MinValue;
            
            await _memoryContext.UpdateUserAsync(user);

            return IdentityResult.Success;
        }

        private async Task UpdateAccessFailedCount(UserData user)
        {
            user.AccessFailedCount += 1;

            if (user.AccessFailedCount >= 3)
            {
                user.LockoutEnd = _dateTime.UtcNow.AddMinutes(int.Parse(_configuration["lockoutEndMinutes"]));
            }

            await _memoryContext.UpdateUserAsync(user);
        }
    }
}