namespace DiyMusicCommunity.Domain.Entities;

public sealed class Track : Entity
{
    public Guid ReleaseId { get; private set; }
    public string Title { get; private set; }
    public int TrackNumber { get; private set; }

    public Track(Guid id, Guid releaseId, string title, int trackNumber) : base(id)
    {
        if (releaseId == Guid.Empty)
        {
            throw new ArgumentException("ReleaseId cannot be empty.", nameof(releaseId));
        }
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Track title cannot be empty.", nameof(title));
        }
        if (trackNumber < 1)
        {
            throw new ArgumentException("Track number must be greater than zero.", nameof(trackNumber));
        }

        ReleaseId = releaseId;
        Title = title;
        TrackNumber = trackNumber;
    }
}
