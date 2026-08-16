using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Releases.GetReleaseDetail;

public sealed class ReleaseDetailModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public ReleaseType ReleaseType { get; init; }
    public DateOnly? ReleaseDate { get; init; }
    public int? Year { get; init; }
    public string? LabelText { get; init; }
    public string? CoverImageUrl { get; init; }
    public ReleaseBandModel? Band { get; init; }
    public IReadOnlyList<Format> Formats { get; init; } = [];
    public IReadOnlyList<ReleaseTrackModel> Tracks { get; init; } = [];
}
