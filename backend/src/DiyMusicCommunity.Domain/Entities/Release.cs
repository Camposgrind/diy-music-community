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

    public Release(
        Guid id,
        Guid bandId,
        string title,
        ReleaseType releaseType,
        DateOnly? releaseDate = null,
        int? year = null,
        string? labelText = null,
        string? formatsText = null,
        string? coverImageUrl = null)
        : base(id)
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
        ReleaseDate = releaseDate;
        Year = year ?? releaseDate?.Year;
        LabelText = labelText;
        FormatsText = formatsText;
        CoverImageUrl = coverImageUrl;
    }
}
