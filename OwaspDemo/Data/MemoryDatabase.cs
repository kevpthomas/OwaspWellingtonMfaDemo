using System.Collections.Generic;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Data
{
    public class MemoryDatabase : IMemoryDatabase
    {
        public ICollection<UserData> RegisteredUsers { get; } = new HashSet<UserData>();
    }
}