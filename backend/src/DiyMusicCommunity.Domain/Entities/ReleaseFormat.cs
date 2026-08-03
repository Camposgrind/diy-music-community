using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class ReleaseFormat : Entity
{
    public Guid ReleaseId { get; private set; }
    public Format Format { get; private set; }

    public ReleaseFormat(Guid id, Guid releaseId, Format format) : base(id)
    {
        if (releaseId == Guid.Empty)
        {
            throw new ArgumentException("ReleaseId cannot be empty.", nameof(releaseId));
        }

        ReleaseId = releaseId;
        Format = format;
    }
}
