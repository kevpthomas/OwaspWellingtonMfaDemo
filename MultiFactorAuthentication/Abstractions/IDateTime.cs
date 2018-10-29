using System;

namespace MultiFactorAuthentication.Abstractions
{
    /// <summary>
    /// Provides an adapter for static <see cref="DateTime"/> members.
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gets a <see cref="T:System.DateTime" /> object that is set to the current date and time on this computer,
        /// expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        /// <returns>An object whose value is the current UTC date and time.</returns>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets a <see cref="T:System.DateTime" /> object that is set to the Unix Epoch date and time,
        /// expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        /// <returns>An object whose value is January 1, 1970 00:00:00.</returns>
        DateTime UnixEpoch { get; }
    }
}