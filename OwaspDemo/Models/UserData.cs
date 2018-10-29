using System;

namespace OwaspDemo.Models
{
    public class UserData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime LockoutEnd { get; set; } = DateTime.MinValue;
        public string KeyPassword { get; } = Guid.NewGuid().ToString("N");
        public string KeySalt { get; } = Guid.NewGuid().ToString("N");
    }
}