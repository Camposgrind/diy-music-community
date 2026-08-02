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

    public BandMember(Guid id, Guid bandId, string name, bool isCurrent) : base(id)
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
        IsCurrent = isCurrent;
    }

    // --- Optional field setters ---

    public void SetInstrument(string? instrument)
    {
        Instrument = instrument;
    }

    public void SetYears(int? startYear, int? endYear)
    {
        if (endYear.HasValue && startYear.HasValue && endYear.Value < startYear.Value)
        {
            throw new ArgumentException("End year cannot be before start year.", nameof(endYear));
        }

        StartYear = startYear;
        EndYear = endYear;

        // Rule: if EndYear is set, member is not current
        if (endYear.HasValue)
        {
            IsCurrent = false;
        }
    }

    public void SetAlsoInBands(string? alsoInBandsText)
    {
        AlsoInBandsText = alsoInBandsText;
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
