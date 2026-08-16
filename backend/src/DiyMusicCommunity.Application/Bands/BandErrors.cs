using DiyMusicCommunity.Application.Common;

namespace DiyMusicCommunity.Application.Bands;

/// <summary>
/// Centralises all error codes and factory methods for Band use cases.
/// No magic strings should appear outside this class.
/// </summary>
public static class BandErrors
{
    /// <summary>Error code constants — use these wherever a code must be compared (e.g. controller routing).</summary>
    public static class Codes
    {
        public const string InvalidFilter = "Band.InvalidFilter";
        public const string TooManyResults = "Band.TooManyResults";
        public const string NotFound = "Band.NotFound";
        public const string Duplicate = "Catalog.Duplicate";
        public const string InvalidRequest = "Catalog.InvalidRequest";
    }

    public static Error InvalidFilter(string message)
        => Error.Validation(Codes.InvalidFilter, message);

    public static Error TooManyResults(int cap)
        => Error.UnprocessableEntity(
            Codes.TooManyResults,
            $"Your search returned more than {cap} bands. Please refine your filters to narrow the results.");

    public static Error NotFound(Guid id)
        => Error.NotFound(Codes.NotFound, $"No band with id '{id}' was found.");

    public static Error Duplicate(string resource)
        => Error.Conflict(Codes.Duplicate, $"A {resource} with the same identity already exists.");

    public static Error InvalidRequest(string message)
        => Error.Validation(Codes.InvalidRequest, message);
}
