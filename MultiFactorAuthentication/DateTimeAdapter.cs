using System;
using MultiFactorAuthentication.Abstractions;

namespace MultiFactorAuthentication
{
    public class DateTimeAdapter : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime UnixEpoch { get; } = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}