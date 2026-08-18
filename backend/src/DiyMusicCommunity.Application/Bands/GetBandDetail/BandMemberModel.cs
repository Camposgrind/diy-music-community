namespace DiyMusicCommunity.Application.Bands.GetBandDetail;

/// <summary>Represents a band member in the band detail response.</summary>
public sealed class BandMemberModel
{
    /// <summary>Unique identifier of the member.</summary>
    public Guid Id { get; init; }

    /// <summary>Display name of the member.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Instrument(s) played. Null if not specified.</summary>
    public string? Instrument { get; init; }

    /// <summary>Year the member joined the band. Null if not specified.</summary>
    public int? StartYear { get; init; }

    /// <summary>Year the member left the band. Null if still active or not specified.</summary>
    public int? EndYear { get; init; }

    /// <summary>Whether the member is currently active in the band.</summary>
    public bool IsCurrent { get; init; }

    /// <summary>Whether the member belongs to a split-up band's final known lineup.</summary>
    public bool IsLastKnownLineup { get; init; }

    /// <summary>Other bands the member has been part of.</summary>
    public IReadOnlyList<BandMemberOtherBandModel> OtherBands { get; init; } = [];
}
