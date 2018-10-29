using System.Threading.Tasks;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Represents a simple database context for accessing in-memory data.
    /// </summary>
    public interface IMemoryContext
    {
        Task AddUserAsync(UserData user);
        Task<UserData> FindUserByNameAsync(string userName);
        Task UpdateUserAsync(UserData user);
    }
}