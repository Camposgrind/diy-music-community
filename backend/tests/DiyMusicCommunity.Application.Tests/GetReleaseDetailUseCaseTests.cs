using DiyMusicCommunity.Application.Releases;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public class GetReleaseDetailUseCaseTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static Release MakeRelease(
        Guid? id = null,
        Guid? bandId = null,
        string title = "Grindcore Not War",
        ReleaseType type = ReleaseType.Album)
    {
        return new Release(
            id ?? Guid.NewGuid(),
            bandId ?? Guid.NewGuid(),
            title,
            type);
    }

    private static Band MakeBand(string name = "Convulsions", string country = "Spain")
        => new(Guid.NewGuid(), name, country, Guid.NewGuid(), BandStatus.Active, DateTime.UtcNow);

    private static (GetReleaseDetailUseCase UseCase, Mock<IReleaseRepository> Repo) BuildSut(string? resolvedImageUrl = null)
    {
        var repo = new Mock<IReleaseRepository>();
        var imageUrlResolver = new Mock<IImageUrlResolver>();
        imageUrlResolver
            .Setup(resolver => resolver.ResolveAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resolvedImageUrl);
        var useCase = new GetReleaseDetailUseCase(repo.Object, imageUrlResolver.Object);
        return (useCase, repo);
    }

    // -----------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Handle_ReleaseExists_Should_ReturnSuccessWithReleaseDetailModel()
    {
        var (sut, repo) = BuildSut();
        var release = MakeRelease();
        repo.Setup(r => r.GetDetailAsync(release.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var result = await sut.Handle(release.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(release.Id, result.Value!.Id);
        Assert.Equal(release.Title, result.Value.Title);
        Assert.Equal(release.ReleaseType, result.Value.ReleaseType);
    }

    [Fact]
    public async Task Handle_ReleaseNotFound_Should_ReturnFailureWithNotFoundError()
    {
        var (sut, repo) = BuildSut();
        var unknownId = Guid.NewGuid();
        repo.Setup(r => r.GetDetailAsync(unknownId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Release?)null);

        var result = await sut.Handle(unknownId);

        Assert.True(result.IsFailure);
        Assert.Equal(ReleaseErrors.Codes.NotFound, result.Error!.Code);
    }

    [Fact]
    public async Task Handle_ReleaseWithNullOptionalFields_Should_ReturnNullFields()
    {
        var (sut, repo) = BuildSut();
        var release = MakeRelease();
        repo.Setup(r => r.GetDetailAsync(release.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var result = await sut.Handle(release.Id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.ReleaseDate);
        Assert.Null(result.Value.Year);
        Assert.Null(result.Value.LabelText);
        Assert.Null(result.Value.CoverImageUrl);
        Assert.Null(result.Value.Band);
    }

    [Fact]
    public async Task Handle_ReleaseWithNoTracks_Should_ReturnEmptyTracks()
    {
        var (sut, repo) = BuildSut();
        var release = MakeRelease();
        repo.Setup(r => r.GetDetailAsync(release.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var result = await sut.Handle(release.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Tracks);
        Assert.Empty(result.Value.Tracks);
    }

    [Fact]
    public async Task Handle_ReleaseWithNoFormats_Should_ReturnEmptyFormats()
    {
        var (sut, repo) = BuildSut();
        var release = MakeRelease();
        repo.Setup(r => r.GetDetailAsync(release.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var result = await sut.Handle(release.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Formats);
        Assert.Empty(result.Value.Formats);
    }

    [Fact]
    public async Task Handle_ReleaseWithAllOptionalFields_Should_MapCorrectly()
    {
        var (sut, repo) = BuildSut("https://example.com/cover.jpg");
        var release = MakeRelease(title: "World Downfall");
        release.SetReleaseDate(new DateOnly(1989, 10, 1));
        release.SetDetails("Earache Records", "https://example.com/cover.jpg");
        release.SetCoverBlobPath("releases/world-downfall/cover.png");
        repo.Setup(r => r.GetDetailAsync(release.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var result = await sut.Handle(release.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(1989, 10, 1), result.Value!.ReleaseDate);
        Assert.Equal(1989, result.Value.Year);
        Assert.Equal("Earache Records", result.Value.LabelText);
        Assert.Equal("https://example.com/cover.jpg", result.Value.CoverImageUrl);
    }

    [Fact]
    public async Task Handle_TracksAreSortedByTrackNumber()
    {
        var releaseId = Guid.NewGuid();
        var bandId = Guid.NewGuid();
        var release = MakeRelease(id: releaseId, bandId: bandId);

        var track3 = new Track(Guid.NewGuid(), releaseId, "Track Three", 3);
        var track1 = new Track(Guid.NewGuid(), releaseId, "Track One", 1);
        var track2 = new Track(Guid.NewGuid(), releaseId, "Track Two", 2);

        // Populate the private backing field via reflection (mirrors what EF does at runtime)
        var tracksField = typeof(Release).GetField("_tracks",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        var tracksList = (List<Track>)tracksField.GetValue(release)!;
        tracksList.AddRange([track3, track1, track2]);

        var repo = new Mock<IReleaseRepository>();
        repo.Setup(r => r.GetDetailAsync(releaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(release);

        var sut = new GetReleaseDetailUseCase(repo.Object, Mock.Of<IImageUrlResolver>());
        var result = await sut.Handle(releaseId);

        Assert.True(result.IsSuccess);
        var tracks = result.Value!.Tracks;
        Assert.Equal(3, tracks.Count);
        Assert.Equal(1, tracks[0].TrackNumber);
        Assert.Equal(2, tracks[1].TrackNumber);
        Assert.Equal(3, tracks[2].TrackNumber);
    }
}
