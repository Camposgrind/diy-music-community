using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiyMusicCommunity.Api.Converters;
using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Application.Releases;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Infrastructure.Persistence;
using FluentAssertions;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class ReleasesControllerTests
{
    // Mirrors the converters registered in Program.cs — FormatJsonConverter must come first
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new FormatJsonConverter(), new JsonStringEnumConverter() }
    };

    private static readonly Guid GrindcoreGenreId = new("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    private static (CustomWebApplicationFactory Factory, HttpClient Client) CreateClient(
        Action<AppDbContext>? seed = null)
    {
        var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClientWithDb(seed);
        return (factory, client);
    }

    // -----------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GET_ReleaseDetail_ExistingId_Should_Return200WithReleaseDetailModel()
    {
        var bandId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();

        var (factory, client) = CreateClient(db =>
        {
            var band = new Band(bandId, "Terrorizer", "USA", GrindcoreGenreId, BandStatus.SplitUp, DateTime.UtcNow, 1989);
            db.Bands.Add(band);

            var release = new Release(releaseId, bandId, "World Downfall", ReleaseType.Album);
            release.SetReleaseDate(new DateOnly(1989, 10, 1));
            release.SetDetails("Earache Records", "https://example.com/cover.jpg");
            db.Releases.Add(release);
        });
        using var _ = factory;

        var response = await client.GetAsync($"/api/releases/{releaseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReleaseDetailModel>(JsonOptions);
        body.Should().NotBeNull();
        body!.Id.Should().Be(releaseId);
        body.Title.Should().Be("World Downfall");
        body.ReleaseType.Should().Be(ReleaseType.Album);
        body.ReleaseDate.Should().Be(new DateOnly(1989, 10, 1));
        body.Year.Should().Be(1989);
        body.LabelText.Should().Be("Earache Records");
        body.CoverImageUrl.Should().Be("https://example.com/cover.jpg");
        body.Band.Should().NotBeNull();
        body.Band!.BandId.Should().Be(bandId);
        body.Band.Name.Should().Be("Terrorizer");
    }

    [Fact]
    public async Task GET_ReleaseDetail_UnknownId_Should_Return404WithNotFoundError()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var unknownId = Guid.NewGuid();
        var response = await client.GetAsync($"/api/releases/{unknownId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var error = await response.Content.ReadFromJsonAsync<Error>();
        error!.Code.Should().Be(ReleaseErrors.Codes.NotFound);
    }

    [Fact]
    public async Task GET_ReleaseDetail_InvalidGuid_Should_Return404()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        // The {id:guid} route constraint returns 404 (no route match) for non-GUID segments
        var response = await client.GetAsync("/api/releases/not-a-guid");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_ReleaseDetail_WithTracks_Should_ReturnTracksOrderedByTrackNumber()
    {
        var bandId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();

        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(new Band(bandId, "Napalm Death", "UK", GrindcoreGenreId, BandStatus.Active, DateTime.UtcNow));
            db.Releases.Add(new Release(releaseId, bandId, "Scum", ReleaseType.Album));

            db.Tracks.AddRange(
                new Track(Guid.NewGuid(), releaseId, "Multinational Corporations", 1),
                new Track(Guid.NewGuid(), releaseId, "Instinct of Survival", 3),
                new Track(Guid.NewGuid(), releaseId, "The Kill", 2));
        });
        using var _ = factory;

        var response = await client.GetAsync($"/api/releases/{releaseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReleaseDetailModel>(JsonOptions);
        body!.Tracks.Should().NotBeNull();
        body.Tracks.Should().HaveCount(3);
        body.Tracks[0].TrackNumber.Should().Be(1);
        body.Tracks[1].TrackNumber.Should().Be(2);
        body.Tracks[2].TrackNumber.Should().Be(3);
        body.Tracks[0].Title.Should().Be("Multinational Corporations");
    }

    [Fact]
    public async Task GET_ReleaseDetail_WithFormats_Should_ReturnFormats()
    {
        var bandId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();

        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(new Band(bandId, "Repulsion", "USA", GrindcoreGenreId, BandStatus.SplitUp, DateTime.UtcNow, 1994));
            var release = new Release(releaseId, bandId, "Horrified", ReleaseType.Album);
            release.AddFormat(Format.Vinyl12);
            release.AddFormat(Format.CD);
            db.Releases.Add(release);
        });
        using var _ = factory;

        var response = await client.GetAsync($"/api/releases/{releaseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReleaseDetailModel>(JsonOptions);
        body!.Formats.Should().NotBeNull();
        body.Formats.Should().HaveCount(2);
        body.Formats.Should().Contain(Format.Vinyl12);
        body.Formats.Should().Contain(Format.CD);
    }

    [Fact]
    public async Task GET_ReleaseDetail_WithNoTracksAndNoFormats_Should_ReturnEmptyCollections()
    {
        var bandId = Guid.NewGuid();
        var releaseId = Guid.NewGuid();

        var (factory, client) = CreateClient(db =>
        {
            db.Bands.Add(new Band(bandId, "Assück", "USA", GrindcoreGenreId, BandStatus.SplitUp, DateTime.UtcNow, 1998));
            db.Releases.Add(new Release(releaseId, bandId, "Misery Index", ReleaseType.Album));
        });
        using var _ = factory;

        var response = await client.GetAsync($"/api/releases/{releaseId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReleaseDetailModel>(JsonOptions);
        body!.Tracks.Should().NotBeNull().And.BeEmpty();
        body.Formats.Should().NotBeNull().And.BeEmpty();
    }
}
