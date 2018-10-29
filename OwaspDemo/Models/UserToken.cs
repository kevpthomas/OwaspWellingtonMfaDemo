namespace OwaspDemo.Models
{
    public class UserToken
    {
        public UserToken(string userId, string value)
        {
            UserId = userId;
            Value = value;
        }

        public string UserId { get; }
        public string Value { get; }
    }
}