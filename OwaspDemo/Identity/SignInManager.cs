using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Identity
{
    /// <summary>
    /// Simple SignIn Manager meant to emulate Microsoft.AspNetCore.Identity.SignInManager<TUser>
    /// </summary>
    public class SignInManager : ISignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManager _userManager;
        private readonly IMemoryContext _memoryContext;

        public SignInManager(IHttpContextAccessor httpContextAccessor, 
            IUserManager userManager,
            IMemoryContext memoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _memoryContext = memoryContext;
        }

        public HttpContext Context => _httpContextAccessor.HttpContext;

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return SignInResult.Failed;

            var isLockedOut = await _userManager.IsLockedOutAsync(user);
            if (isLockedOut) return SignInResult.LockedOut;

            var signInResult1 = await CheckPasswordSignInAsync(user, password);
            SignInResult signInResult2;
            if (signInResult1.Succeeded)
            {
                signInResult2 = await SignInOrTwoFactorAsync(user);
            }
            else
            {
                signInResult2 = signInResult1;
            }

            return signInResult2;
        }

        public async Task SignInAsync(string user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user));
            identity.AddClaim(new Claim(ClaimTypes.Name, user));

            // Authenticate using the identity
            var principal = new ClaimsPrincipal(identity);
            await Context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                principal, 
                new AuthenticationProperties { IsPersistent = false });

            var userData = await _userManager.FindByNameAsync(identity.Name);
            await _userManager.ResetAccessFailedCountAsync(userData);
        }

        public async Task<UserData> GetTwoFactorAuthenticationUserAsync()
        {
            var twoFactorInfo = await Context.AuthenticateAsync();

            if (twoFactorInfo == null) return default(UserData);

            return await _userManager.FindByNameAsync(twoFactorInfo.UserId);
        }

        public async Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code)
        {
            var twoFactorInfo = await Context.AuthenticateAsync();

            if (twoFactorInfo?.UserId == null)
                return SignInResult.Failed;

            var user = await _userManager.FindByNameAsync(twoFactorInfo.UserId);
            if (user == null) return SignInResult.Failed;

            var isLockedOut = await _userManager.IsLockedOutAsync(user);
            if (isLockedOut) return SignInResult.LockedOut;

            var isTwoFactorVerified = await _userManager.VerifyTwoFactorTokenAsync(user, code);
            if (!isTwoFactorVerified) return SignInResult.Failed;

            await Context.SignOutAsync();
            await SignInAsync(user.Id);
            return SignInResult.Success;
        }

        private async Task<SignInResult> SignInOrTwoFactorAsync(UserData user)
        {
            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            if (isTwoFactorEnabled)
            {
                await Context.SignInAsync(user);
                return SignInResult.TwoFactorRequired;
            }

            await SignInAsync(user.Id);
            return SignInResult.Success;
        }

        private async Task<SignInResult> CheckPasswordSignInAsync(UserData user, string password)
        {
            var isAuthorised = await _userManager.CheckPasswordAsync(user, password);
            return isAuthorised ? SignInResult.Success : SignInResult.NotAllowed;
        }
    }
}