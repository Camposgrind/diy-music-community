using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Bands.GetBands;

/// <summary>Query parameters for the GET /api/bands endpoint.</summary>
public sealed class GetBandsQuery
{
    /// <summary>
    /// Optional partial name filter (case-insensitive contains match).
    /// Maximum 200 characters.
    /// </summary>
    /// <example>discharge</example>
    public string? Name { get; init; }

    /// <summary>
    /// Optional exact country filter (case-insensitive).
    /// Maximum 100 characters.
    /// </summary>
    /// <example>UK</example>
    public string? Country { get; init; }

    /// <summary>Optional genre identifier for an exact match on Band.GenreId.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid? GenreId { get; init; }

    /// <summary>Optional band activity status filter.</summary>
    /// <example>Active</example>
    public BandStatus? Status { get; init; }

    /// <summary>Page number (1-based). Defaults to 1.</summary>
    /// <example>1</example>
    public int Page { get; init; } = 1;

    /// <summary>Number of items per page. Defaults to 20. Maximum 50.</summary>
    /// <example>20</example>
    public int PageSize { get; init; } = 20;
}
