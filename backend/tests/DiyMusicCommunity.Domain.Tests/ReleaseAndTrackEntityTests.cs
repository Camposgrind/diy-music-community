using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Tests;

public class ReleaseFormatTests
{
    private static readonly Guid BandId = Guid.NewGuid();

    private static Release CreateRelease()
    {
        return new Release(Guid.NewGuid(), BandId, "World Downfall", ReleaseType.Album);
    }

    // --- AddFormat ---

    [Fact]
    public void AddFormat_Should_AppendFormatToRelease()
    {
        var release = CreateRelease();

        release.AddFormat(Format.Vinyl12);

        Assert.Contains(Format.Vinyl12, release.GetFormats());
    }

    [Fact]
    public void AddFormat_Duplicate_Should_ThrowArgumentException()
    {
        var release = CreateRelease();
        release.AddFormat(Format.CD);

        Assert.Throws<ArgumentException>(() => release.AddFormat(Format.CD));
    }

    [Fact]
    public void AddFormat_Multiple_Should_StoreAllFormats()
    {
        var release = CreateRelease();

        release.AddFormat(Format.Vinyl12);
        release.AddFormat(Format.CD);
        release.AddFormat(Format.Digital);

        var formats = release.GetFormats();
        Assert.Equal(3, formats.Count);
        Assert.Contains(Format.Vinyl12, formats);
        Assert.Contains(Format.CD, formats);
        Assert.Contains(Format.Digital, formats);
    }

    // --- RemoveFormat ---

    [Fact]
    public void RemoveFormat_Existing_Should_RemoveFromRelease()
    {
        var release = CreateRelease();
        release.AddFormat(Format.Cassette);

        release.RemoveFormat(Format.Cassette);

        Assert.DoesNotContain(Format.Cassette, release.GetFormats());
    }

    [Fact]
    public void RemoveFormat_NotPresent_Should_ThrowArgumentException()
    {
        var release = CreateRelease();

        Assert.Throws<ArgumentException>(() => release.RemoveFormat(Format.DVD));
    }

    // --- FormatsText is gone ---

    [Fact]
    public void NewRelease_Should_HaveNoFormats()
    {
        var release = CreateRelease();

        Assert.Empty(release.GetFormats());
    }
}


public class ReleaseEntityTests
{
    private static readonly Guid BandId = Guid.NewGuid();

    private static Release CreateRelease(
        string title = "World Downfall",
        ReleaseType type = ReleaseType.Album,
        DateOnly? releaseDate = null)
    {
        var release = new Release(Guid.NewGuid(), BandId, title, type);
        if (releaseDate.HasValue)
        {
            release.SetReleaseDate(releaseDate);
        }
        return release;
    }

    // --- Construction guards ---

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewRelease_WithEmptyTitle_Should_ThrowArgumentException(string title)
    {
        Assert.Throws<ArgumentException>(() => CreateRelease(title: title));
    }

    [Fact]
    public void NewRelease_WithEmptyBandId_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Release(Guid.NewGuid(), Guid.Empty, "World Downfall", ReleaseType.Album));
    }

    // --- Year derived from ReleaseDate ---

    [Fact]
    public void NewRelease_WithReleaseDate_Should_DeriveYear()
    {
        var date = new DateOnly(1989, 6, 1);
        var release = CreateRelease(releaseDate: date);

        Assert.Equal(1989, release.Year);
        Assert.Equal(date, release.ReleaseDate);
    }

    [Fact]
    public void NewRelease_WithExplicitYear_Should_UseExplicitYear()
    {
        var release = new Release(Guid.NewGuid(), BandId, "World Downfall", ReleaseType.Album);
        release.SetYear(1989);

        Assert.Equal(1989, release.Year);
    }

    [Fact]
    public void NewRelease_WithoutDateOrYear_Should_HaveNullYear()
    {
        var release = CreateRelease();

        Assert.Null(release.Year);
        Assert.Null(release.ReleaseDate);
    }

    // --- Belongs to band ---

    [Fact]
    public void NewRelease_Should_BelongToSuppliedBand()
    {
        var release = CreateRelease();

        Assert.Equal(BandId, release.BandId);
    }
}

public class TrackEntityTests
{
    private static readonly Guid ReleaseId = Guid.NewGuid();

    // --- Construction guards ---

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewTrack_WithEmptyTitle_Should_ThrowArgumentException(string title)
    {
        Assert.Throws<ArgumentException>(() =>
            new Track(Guid.NewGuid(), ReleaseId, title, 1));
    }

    [Fact]
    public void NewTrack_WithEmptyReleaseId_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Track(Guid.NewGuid(), Guid.Empty, "Fear of Napalm", 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NewTrack_WithTrackNumberLessThanOne_Should_ThrowArgumentException(int number)
    {
        Assert.Throws<ArgumentException>(() =>
            new Track(Guid.NewGuid(), ReleaseId, "Fear of Napalm", number));
    }

    // --- TrackNumber unique per release (ordering) ---

    [Fact]
    public void Tracks_Should_BeOrderableByTrackNumber()
    {
        var tracks = new[]
        {
            new Track(Guid.NewGuid(), ReleaseId, "Track C", 3),
            new Track(Guid.NewGuid(), ReleaseId, "Track A", 1),
            new Track(Guid.NewGuid(), ReleaseId, "Track B", 2)
        };

        var ordered = tracks.OrderBy(t => t.TrackNumber).ToList();

        Assert.Equal(1, ordered[0].TrackNumber);
        Assert.Equal(2, ordered[1].TrackNumber);
        Assert.Equal(3, ordered[2].TrackNumber);
    }

    [Fact]
    public void NewTrack_Should_BelongToSuppliedRelease()
    {
        var track = new Track(Guid.NewGuid(), ReleaseId, "Fear of Napalm", 1);

        Assert.Equal(ReleaseId, track.ReleaseId);
    }
}
