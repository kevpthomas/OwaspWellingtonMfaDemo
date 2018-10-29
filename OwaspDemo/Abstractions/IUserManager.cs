using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Provides the APIs for managing user in a persistence store.
    /// </summary>
    /// <remarks>
    /// Implementation emulates Microsoft.AspNetCore.Identity.UserManager.
    /// </remarks>
    public interface IUserManager
    {
        /// <summary>
        /// Creates the specified <paramref name="user" /> in the backing store with given password,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user to hash and store.</param>
        /// <returns></returns>
        Task<IdentityResult> CreateAsync(string user, string password);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified user name.
        /// </summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="userName" /> if it exists.
        /// </returns>
        Task<UserData> FindByNameAsync(string userName);

        /// <summary>
        /// Returns a flag indicating whether the given <paramref name="password" /> is valid for the
        /// specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose password should be validated.</param>
        /// <param name="password">The password to validate</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing true if
        /// the specified <paramref name="password" /> matches the one store for the <paramref name="user" />,
        /// otherwise false.</returns>
        Task<bool> CheckPasswordAsync(UserData user, string password);

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be retrieved.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, true if the specified <paramref name="user " />
        /// has two factor authentication enabled, otherwise false.
        /// </returns>
        Task<bool> GetTwoFactorEnabledAsync(UserData user);

        /// <summary>
        /// Verifies the specified two factor authentication <paramref name="token" /> against the <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user the token is supposed to be for.</param>
        /// <param name="token">The token to verify.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents result of the asynchronous operation, true if the token is valid,
        /// otherwise false.
        /// </returns>
        Task<bool> VerifyTwoFactorTokenAsync(UserData user, string token);

        /// <summary>
        /// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
        /// the principal or null.
        /// </summary>
        /// <param name="principal">The principal which contains the user id claim.</param>
        /// <returns>The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
        /// the principal or null</returns>
        Task<UserData> GetUserAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Returns the User ID claim value if present otherwise returns null.
        /// </summary>
        /// <param name="principal">The <see cref="T:System.Security.Claims.ClaimsPrincipal" /> instance.</param>
        /// <returns>The User ID claim value, or null if the claim is not present.</returns>
        /// <remarks>The User ID claim is identified by <see cref="F:System.Security.Claims.ClaimTypes.NameIdentifier" />.</remarks>
        string GetUserId(ClaimsPrincipal principal);

        /// <summary>
        /// Returns the authenticator key for the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The authenticator key</returns>
        Task<string> GetAuthenticatorKeyAsync(UserData user);

        /// <summary>
        /// Resets the authenticator key for the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Whether the user was successfully updated.</returns>
        Task<IdentityResult> ResetAuthenticatorKeyAsync(UserData user);

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation,
        /// the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the operation
        /// </returns>
        Task<IdentityResult> SetTwoFactorEnabledAsync(UserData user, bool enabled);

        /// <summary>
        /// Resets the access failed count for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation,
        /// containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the operation.
        /// </returns>
        Task<IdentityResult> ResetAccessFailedCountAsync(UserData user);

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user" /> his locked out,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose locked out status should be retrieved.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, true if the specified <paramref name="user " />
        /// is locked out, otherwise false.
        /// </returns>
        Task<bool> IsLockedOutAsync(UserData user);
    }
}