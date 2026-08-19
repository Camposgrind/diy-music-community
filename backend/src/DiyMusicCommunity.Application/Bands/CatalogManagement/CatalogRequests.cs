using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Bands.CatalogManagement;

public sealed class BandWriteRequest
{
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public Guid GenreId { get; init; }
    public BandStatus Status { get; init; }
    public string? Location { get; init; }
    public int? FormationYear { get; init; }
    public int? SplitUpYear { get; init; }
    public string? Description { get; init; }
    public string? LogoImageUrl { get; init; }
    public string? BandImageUrl { get; init; }
    public string? MusicUrlPortal { get; init; }
    public string? BandContact { get; init; }
}

public sealed class MemberWriteRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Instrument { get; init; }
    public int? StartYear { get; init; }
    public int? EndYear { get; init; }
    public bool IsCurrent { get; init; }
    public bool IsLastKnownLineup { get; init; }
}

public sealed class ReleaseWriteRequest
{
    public string Title { get; init; } = string.Empty;
    public ReleaseType ReleaseType { get; init; }
    public DateOnly? ReleaseDate { get; init; }
    public int? Year { get; init; }
    public string? LabelText { get; init; }
    public string? CoverImageUrl { get; init; }
    public IReadOnlyList<TrackWriteRequest> Tracks { get; init; } = [];
}

public sealed class TrackWriteRequest
{
    public string Title { get; init; } = string.Empty;
}

public sealed class CatalogResourceModel
{
    public Guid Id { get; init; }
}
