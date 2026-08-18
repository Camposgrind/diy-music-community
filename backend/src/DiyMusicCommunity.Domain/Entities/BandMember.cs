namespace DiyMusicCommunity.Domain.Entities;

public sealed class BandMember : Entity
{
    private readonly List<BandMemberOtherBand> _otherBands = new();

    public Guid BandId { get; private set; }
    public string Name { get; private set; }
    public string? Instrument { get; private set; }
    public int? StartYear { get; private set; }
    public int? EndYear { get; private set; }
    public bool IsCurrent { get; private set; }
    public bool IsLastKnownLineup { get; private set; }

    public IReadOnlyList<BandMemberOtherBand> OtherBands
    {
        get { return _otherBands.AsReadOnly(); }
    }

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

        if (endYear.HasValue)
        {
            IsCurrent = false;
        }
    }

    public void Update(string name, string? instrument, int? startYear, int? endYear, bool isCurrent, bool isLastKnownLineup)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Member name cannot be empty.", nameof(name));
        }
        if (isLastKnownLineup && !endYear.HasValue)
        {
            throw new ArgumentException("Last known lineup members require an end year.", nameof(endYear));
        }

        SetYears(startYear, endYear);
        Name = name;
        Instrument = instrument;
        IsCurrent = !endYear.HasValue && isCurrent;
        IsLastKnownLineup = isLastKnownLineup;
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

    // --- Other bands ---

    public void AddOtherBand(Guid otherBandId)
    {
        if (otherBandId == Guid.Empty)
        {
            throw new ArgumentException("OtherBandId cannot be empty.", nameof(otherBandId));
        }
        if (_otherBands.Any(b => b.OtherBandId == otherBandId))
        {
            throw new ArgumentException($"Band '{otherBandId}' is already listed for this member.", nameof(otherBandId));
        }

        _otherBands.Add(new BandMemberOtherBand(Guid.NewGuid(), Id, otherBandId));
    }

    public void RemoveOtherBand(Guid otherBandId)
    {
        var existing = _otherBands.FirstOrDefault(b => b.OtherBandId == otherBandId);
        if (existing is null)
        {
            throw new ArgumentException($"Band '{otherBandId}' is not listed for this member.", nameof(otherBandId));
        }

        _otherBands.Remove(existing);
    }

    public IReadOnlyList<Guid> GetOtherBands()
    {
        return _otherBands.Select(b => b.OtherBandId).ToList().AsReadOnly();
    }
}

