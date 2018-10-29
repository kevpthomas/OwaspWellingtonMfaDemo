using System;
using System.Linq;
using System.Threading.Tasks;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Data
{
    public class MemoryContext : IMemoryContext
    {
        private readonly IMemoryDatabase _db;

        public MemoryContext(IMemoryDatabase db)
        {
            _db = db;
        }

        public async Task AddUserAsync(UserData user)
        {
            await Task.Run(() => _db.RegisteredUsers.Add(user));
        }

        public async Task UpdateUserAsync(UserData user)
        {
            var existingUser = _db.RegisteredUsers
                .FirstOrDefault(x => x.Id.Equals(user.Id, StringComparison.InvariantCultureIgnoreCase));
            if (existingUser != null)
            {
                _db.RegisteredUsers.Remove(existingUser);
            }
            await AddUserAsync(user);
        }

        public async Task<UserData> FindUserByNameAsync(string userName)
        {
            return await Task.Run(() => _db.RegisteredUsers.SingleOrDefault(x =>
                x.Id.Equals(userName, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}