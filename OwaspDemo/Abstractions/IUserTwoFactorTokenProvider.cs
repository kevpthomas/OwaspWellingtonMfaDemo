using System.Threading.Tasks;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Provides an abstraction for two factor token generators.
    /// </summary>
    /// <remarks>
    /// Emulates Microsoft.AspNetCore.Identity.IUserTwoFactorTokenProvider.
    /// </remarks>
    public interface IUserTwoFactorTokenProvider
    {
        /// <summary>
        /// Generates a token for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="manager">The <see cref="IUserManager" /> that can be used to retrieve user properties.</param>
        /// <param name="user">The user a token should be generated for.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the token for the specified
        /// <paramref name="user" />.
        /// </returns>
        Task<string> GenerateAsync(IUserManager manager, UserData user);

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="token" /> is valid for the given
        /// <paramref name="user" />.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <param name="manager">The <see cref="IUserManager" /> that can be used to retrieve user properties.</param>
        /// <param name="user">The user a token should be validated for.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the a flag
        /// indicating the result of validating the <paramref name="token"> for the specified </paramref><paramref name="user" />.
        /// The task will return true if the token is valid, otherwise false.
        /// </returns>
        Task<bool> ValidateAsync(string token, IUserManager manager, UserData user);
    }
}