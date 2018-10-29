using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Provides the APIs for user sign in.
    /// </summary>
    /// <remarks>
    /// Implementation emulates Microsoft.AspNetCore.Identity.SignInManager.
    /// </remarks>
    public interface ISignInManager
    {
        /// <summary>
        /// Attempts to sign in the specified <paramref name="userName" /> and <paramref name="password" /> combination
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="userName">The user name to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult" />
        /// for the sign-in attempt.</returns>
        Task<SignInResult> PasswordSignInAsync(string userName, string password);

        /// <summary>
        /// Signs in the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to sign-in.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task SignInAsync(string user);

        /// <summary>
        /// Gets the <see cref="UserData"/> for the current two factor authentication login, as an asynchronous operation.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation containing the <see cref="UserData"/>
        /// for the sign-in attempt.</returns>
        Task<UserData> GetTwoFactorAuthenticationUserAsync();

        /// <summary>
        /// Validates the sign in code from an authenticator app and creates and signs in the user, as an asynchronous operation.
        /// </summary>
        /// <param name="code">The two factor authentication code to validate.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult" />
        /// for the sign-in attempt.</returns>
        Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code);
    }
}