using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class Release : Entity
{
    public Guid BandId { get; private set; }
    public string Title { get; private set; }
    public ReleaseType ReleaseType { get; private set; }
    public DateOnly? ReleaseDate { get; private set; }
    public int? Year { get; private set; }
    public string? LabelText { get; private set; }
    public string? FormatsText { get; private set; }
    public string? CoverImageUrl { get; private set; }

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

    public void SetDetails(string? labelText, string? formatsText, string? coverImageUrl)
    {
        LabelText = labelText;
        FormatsText = formatsText;
        CoverImageUrl = coverImageUrl;
    }
}
