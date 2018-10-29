using System.Collections.Generic;
using OwaspDemo.Models;

namespace OwaspDemo.Abstractions
{
    /// <summary>
    /// Represents simple in-memory database to be injected with a singleton lifetime.
    /// </summary>
    public interface IMemoryDatabase
    {
        ICollection<UserData> RegisteredUsers { get; }
    }
}