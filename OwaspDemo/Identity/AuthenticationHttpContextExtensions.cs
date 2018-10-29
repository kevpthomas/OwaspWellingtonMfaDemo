using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OwaspDemo.Models;

namespace OwaspDemo.Identity
{
    /// <summary>
    /// Extension methods to expose Authentication on HttpContext.
    /// </summary>
    /// <remarks>
    /// Emulates Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.
    /// </remarks>
    public static class AuthenticationHttpContextExtensions
    {
        private const string TwoFactorAuthKey = "2FA";

        /// <summary>
        /// Extension method for authenticate.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> context.</param>
        /// <returns>The <see cref="TwoFactorAuthenticationInfo" />.</returns>
        public static async Task<TwoFactorAuthenticationInfo> AuthenticateAsync(this HttpContext context)
        {
            return await Task.Run(() =>
            {
                var tempDataProvider = (ITempDataProvider)context.RequestServices.GetService(typeof(ITempDataProvider));
                var twoFactorInfo = new TwoFactorAuthenticationInfo
                    {UserId = (string) tempDataProvider.LoadTempData(context)[TwoFactorAuthKey]};
                return twoFactorInfo;
            });
        }

        /// <summary>
        /// Extension method for SignIn.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> context.</param>
        /// <param name="user">User being signed in.</param>
        /// <returns>The task.</returns>
        public static async Task SignInAsync(this HttpContext context, UserData user)
        {
            await Task.Run(() =>
            {
                var tempDataProvider = (ITempDataProvider)context.RequestServices.GetService(typeof(ITempDataProvider));
                var tempDataDictionary = new Dictionary<string, object> {{TwoFactorAuthKey, user.Id}};
                tempDataProvider.SaveTempData(context, tempDataDictionary);
            });
        }

        /// <summary>
        /// Extension method for SignOut using the <see cref="P:Microsoft.AspNetCore.Authentication.AuthenticationOptions.DefaultSignOutScheme" />.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> context.</param>
        /// <returns>The task.</returns>
        public static async Task SignOutAsync(this HttpContext context)
        {
            await Task.Run(() =>
            {
                var tempDataProvider = (ITempDataProvider)context.RequestServices.GetService(typeof(ITempDataProvider));
                tempDataProvider.LoadTempData(context)[TwoFactorAuthKey] = null;
            });
        }
    }
}