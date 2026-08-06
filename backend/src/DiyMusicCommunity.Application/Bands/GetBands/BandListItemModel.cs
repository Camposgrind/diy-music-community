namespace DiyMusicCommunity.Application.Bands.GetBands;

/// <summary>Lightweight band summary used in the public band list table.</summary>
public sealed class BandListItemModel
{
    /// <summary>Unique band identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>Band name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Country of origin (e.g. "UK", "Spain", "USA").</summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>Genre name (e.g. "Grindcore", "D-Beat", "Crust").</summary>
    public string Genre { get; init; } = string.Empty;

    /// <summary>Current activity status of the band.</summary>
    /// <example>Active</example>
    public string Status { get; init; } = string.Empty;

    /// <summary>Year the band was formed. Null if unknown.</summary>
    /// <example>1977</example>
    public int? FormationYear { get; init; }
}
