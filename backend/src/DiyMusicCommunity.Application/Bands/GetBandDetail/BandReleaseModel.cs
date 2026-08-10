using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Bands.GetBandDetail;

/// <summary>Represents a release in the band detail response.</summary>
public sealed class BandReleaseModel
{
    /// <summary>Unique identifier of the release.</summary>
    public Guid Id { get; init; }

    /// <summary>Title of the release.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Type of release (Album, EP, Demo, Split, Compilation).</summary>
    public ReleaseType ReleaseType { get; init; }

    /// <summary>Year of release. Null if not specified.</summary>
    public int? Year { get; init; }
}
