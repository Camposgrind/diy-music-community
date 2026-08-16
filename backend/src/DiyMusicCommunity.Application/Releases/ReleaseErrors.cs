using DiyMusicCommunity.Application.Common;

namespace DiyMusicCommunity.Application.Releases;

/// <summary>
/// Centralises all error codes and factory methods for Release use cases.
/// No magic strings should appear outside this class.
/// </summary>
public static class ReleaseErrors
{
    /// <summary>Error code constants — use these wherever a code must be compared (e.g. controller routing).</summary>
    public static class Codes
    {
        public const string NotFound = "Release.NotFound";
    }

    public static Error NotFound(Guid id)
        => Error.NotFound(Codes.NotFound, $"No release with id '{id}' was found.");
}
