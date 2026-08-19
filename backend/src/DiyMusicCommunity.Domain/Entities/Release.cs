using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class Release : Entity
{
    private readonly List<ReleaseFormat> _formats = new();
    private readonly List<Track> _tracks = new();

    public Guid BandId { get; private set; }
    public Band? Band { get; private set; }
    public string Title { get; private set; }
    public ReleaseType ReleaseType { get; private set; }
    public DateOnly? ReleaseDate { get; private set; }
    public int? Year { get; private set; }
    public string? LabelText { get; private set; }
    public string? CoverImageUrl { get; private set; }

    public IReadOnlyList<ReleaseFormat> Formats
    {
        get { return _formats.AsReadOnly(); }
    }

    public IReadOnlyList<Track> Tracks => _tracks.AsReadOnly();

    public Release(Guid id, Guid bandId, string title, ReleaseType releaseType) : base(id)
    {
        if (bandId == Guid.Empty)
        {
            throw new ArgumentException("BandId cannot be empty.", nameof(bandId));
        }
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Release title cannot be empty.", nameof(title));
        }

        BandId = bandId;
        Title = title;
        ReleaseType = releaseType;
    }

    // --- Optional field setters ---

    public void SetReleaseDate(DateOnly? releaseDate)
    {
        ReleaseDate = releaseDate;
        if (releaseDate.HasValue && !Year.HasValue)
        {
            Year = releaseDate.Value.Year;
        }
    }

    public void SetYear(int? year)
    {
        Year = year;
    }

    public void SetDetails(string? labelText, string? coverImageUrl)
    {
        LabelText = labelText;
        CoverImageUrl = coverImageUrl;
    }

    public void Update(string title, ReleaseType releaseType, DateOnly? releaseDate, int? year, string? labelText, string? coverImageUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Release title cannot be empty.", nameof(title));
        }

        Title = title;
        ReleaseType = releaseType;
        ReleaseDate = releaseDate;
        Year = releaseDate?.Year ?? year;
        LabelText = labelText;
        CoverImageUrl = coverImageUrl;
    }

    public void ReplaceTracks(IReadOnlyList<(string Title, int TrackNumber)> tracks)
    {
        if (tracks.GroupBy(track => track.TrackNumber).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Track numbers must be unique within a release.", nameof(tracks));
        }

        var newTracks = new List<Track>();
        foreach (var track in tracks)
        {
            newTracks.Add(new Track(Guid.NewGuid(), Id, track.Title, track.TrackNumber));
        }

        _tracks.Clear();
        _tracks.AddRange(newTracks);
    }

    // --- Formats ---

    public void AddFormat(Format format)
    {
        if (_formats.Any(f => f.Format == format))
        {
            throw new ArgumentException($"Format '{format}' is already added to this release.", nameof(format));
        }

        _formats.Add(new ReleaseFormat(Guid.NewGuid(), Id, format));
    }

    public void RemoveFormat(Format format)
    {
        var existing = _formats.FirstOrDefault(f => f.Format == format);
        if (existing is null)
        {
            throw new ArgumentException($"Format '{format}' is not present on this release.", nameof(format));
        }

        _formats.Remove(existing);
    }

    public void ReplaceFormats(IReadOnlyList<Format> formats)
    {
        if (formats.GroupBy(format => format).Any(group => group.Count() > 1))
        {
            throw new ArgumentException("Release formats must be unique.", nameof(formats));
        }

        _formats.Clear();
        foreach (var format in formats)
        {
            _formats.Add(new ReleaseFormat(Guid.NewGuid(), Id, format));
        }
    }

    public IReadOnlyList<Format> GetFormats()
    {
        return _formats.Select(f => f.Format).ToList().AsReadOnly();
    }
}
