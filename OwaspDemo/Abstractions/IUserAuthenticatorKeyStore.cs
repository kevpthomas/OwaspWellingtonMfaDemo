using System.Threading.Tasks;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Provides an abstraction for a store which stores info about user's authenticator.
    /// </summary>
    /// <remarks>
    /// Emulates Microsoft.AspNetCore.Identity.IUserAuthenticatorKeyStore
    /// </remarks>
    public interface IUserAuthenticatorKeyStore
    {
        /// <summary>
        /// Get the authenticator key for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation,
        /// containing the security stamp for the specified <paramref name="user" />.</returns>
        Task<string> GetAuthenticatorKeyAsync(UserData user);

        /// <summary>
        /// Sets the authenticator key for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose authenticator key should be set.</param>
        /// <param name="key">The authenticator key to set.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task SetAuthenticatorKeyAsync(UserData user, string key);
    }
}