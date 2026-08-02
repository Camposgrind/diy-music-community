namespace DiyMusicCommunity.Domain.Entities;

public sealed class BandMember : Entity
{
    public Guid BandId { get; private set; }
    public string Name { get; private set; }
    public string? Instrument { get; private set; }
    public int? StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public bool IsCurrent { get; private set; }
    public string? AlsoInBandsText { get; private set; }

    public BandMember(
        Guid id,
        Guid bandId,
        string name,
        bool isCurrent,
        string? instrument = null,
        int? startYear = null,
        int? endYear = null,
        string? alsoInBandsText = null)
        : base(id)
    {
        if (bandId == Guid.Empty)
        {
            throw new ArgumentException("BandId cannot be empty.", nameof(bandId));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Member name cannot be empty.", nameof(name));
        }

        BandId = bandId;
        Name = name;
        Instrument = instrument;
        StartYear = startYear;
        EndYear = endYear;
        AlsoInBandsText = alsoInBandsText;

        // Rule: if EndYear is set, member is not current
        IsCurrent = endYear.HasValue ? false : isCurrent;
    }

    public void SetDeparted(int endYear)
    {
        if (endYear < (StartYear ?? 0))
        {
            throw new ArgumentException("End year cannot be before start year.", nameof(endYear));
        }

        EndYear = endYear;
        IsCurrent = false;
    }
}
