namespace DiyMusicCommunity.Application.Bands.GetBandDetail;

/// <summary>Represents a reference to another band a member has been part of.</summary>
public sealed class BandMemberOtherBandModel
{
    /// <summary>The id of the other band.</summary>
    public Guid BandId { get; init; }

    /// <summary>The name of the other band.</summary>
    public string? BandName { get; init; }
}
