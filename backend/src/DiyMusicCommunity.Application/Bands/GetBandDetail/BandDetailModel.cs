using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Bands.GetBandDetail;

/// <summary>Full band profile returned by GET /api/bands/{id}.</summary>
public sealed class BandDetailModel
{
    /// <summary>Unique identifier of the band.</summary>
    public Guid Id { get; init; }

    /// <summary>Band name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Country of origin.</summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>City or region. Null if not specified.</summary>
    public string? Location { get; init; }

    /// <summary>Current activity status.</summary>
    public BandStatus Status { get; init; }

    /// <summary>Primary genre name. Null if genre is not loaded.</summary>
    public string? Genre { get; init; }

    /// <summary>Year the band was formed. Null if not specified.</summary>
    public int? FormationYear { get; init; }

    /// <summary>Year the band split up. Null unless its status is SplitUp.</summary>
    public int? SplitUpYear { get; init; }

    /// <summary>Biography or description. Null if not specified.</summary>
    public string? Description { get; init; }

    /// <summary>URL of the band logo image. Null if not specified.</summary>
    public string? LogoImageUrl { get; init; }

    /// <summary>URL of the band photo. Null if not specified.</summary>
    public string? BandImageUrl { get; init; }

    /// <summary>URL to the band's music portal (e.g. Bandcamp). Null if not specified.</summary>
    public string? MusicUrlPortal { get; init; }

    /// <summary>Contact information for the band. Null if not specified.</summary>
    public string? BandContact { get; init; }

    /// <summary>Discography — never null; empty list when the band has no releases.</summary>
    public IReadOnlyList<BandReleaseModel> Releases { get; init; } = [];

    /// <summary>Lineup — never null; empty list when no members are recorded.</summary>
    public IReadOnlyList<BandMemberModel> Members { get; init; } = [];
}
