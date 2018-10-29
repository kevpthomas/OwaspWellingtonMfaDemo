using System.Threading.Tasks;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Represents a simple database context for accessing in-cache data.
    /// </summary>
    public interface ICacheContext
    {
        Task<UserToken> FindTokenByUserIdAsync(string userId);
        Task<UserToken> AddAsync(string userId, string value);
    }
}