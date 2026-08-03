namespace DiyMusicCommunity.Domain.Entities;

public sealed class BandMemberOtherBand : Entity
{
    public Guid BandMemberId { get; private set; }
    public Guid OtherBandId { get; private set; }

    public BandMemberOtherBand(Guid id, Guid bandMemberId, Guid otherBandId) : base(id)
    {
        if (bandMemberId == Guid.Empty)
        {
            throw new ArgumentException("BandMemberId cannot be empty.", nameof(bandMemberId));
        }
        if (otherBandId == Guid.Empty)
        {
            throw new ArgumentException("OtherBandId cannot be empty.", nameof(otherBandId));
        }

        BandMemberId = bandMemberId;
        OtherBandId = otherBandId;
    }
}
