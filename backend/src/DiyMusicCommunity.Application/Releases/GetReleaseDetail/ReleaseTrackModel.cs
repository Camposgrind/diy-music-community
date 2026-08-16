namespace DiyMusicCommunity.Application.Releases.GetReleaseDetail;

public sealed class ReleaseTrackModel
{
    public Guid ReleaseId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int TrackNumber { get; init; }
}
