using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public class GetBandDetailUseCaseTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static Band MakeBand(
        string name = "Napalm Death",
        string country = "UK",
        BandStatus status = BandStatus.Active,
        Guid? genreId = null)
    {
        return new Band(
            Guid.NewGuid(),
            name,
            country,
            genreId ?? Guid.NewGuid(),
            status,
            DateTime.UtcNow);
    }

    private static Genre MakeGenre(string name = "Grindcore")
        => new(Guid.NewGuid(), name);

    private static (GetBandDetailUseCase UseCase, Mock<IBandRepository> Repo) BuildSut()
    {
        var repo = new Mock<IBandRepository>();
        var useCase = new GetBandDetailUseCase(repo.Object);
        return (useCase, repo);
    }

    // -----------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Handle_BandExists_Should_ReturnSuccessWithBandDetailModel()
    {
        var (sut, repo) = BuildSut();
        var band = MakeBand();
        repo.Setup(r => r.GetDetailAsync(band.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(band);

        var result = await sut.Handle(band.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(band.Id, result.Value!.Id);
        Assert.Equal(band.Name, result.Value.Name);
        Assert.Equal(band.Country, result.Value.Country);
    }

    [Fact]
    public async Task Handle_BandNotFound_Should_ReturnFailureWithNotFoundError()
    {
        var (sut, repo) = BuildSut();
        var unknownId = Guid.NewGuid();
        repo.Setup(r => r.GetDetailAsync(unknownId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Band?)null);

        var result = await sut.Handle(unknownId);

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.NotFound, result.Error!.Code);
    }

    [Fact]
    public async Task Handle_BandWithNoReleases_Should_ReturnEmptyReleasesList()
    {
        var (sut, repo) = BuildSut();
        var band = MakeBand();
        repo.Setup(r => r.GetDetailAsync(band.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(band);

        var result = await sut.Handle(band.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Releases);
        Assert.Empty(result.Value.Releases);
    }

    [Fact]
    public async Task Handle_BandWithNoMembers_Should_ReturnEmptyMembersList()
    {
        var (sut, repo) = BuildSut();
        var band = MakeBand();
        repo.Setup(r => r.GetDetailAsync(band.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(band);

        var result = await sut.Handle(band.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.Members);
        Assert.Empty(result.Value.Members);
    }

    [Fact]
    public async Task Handle_BandWithNullGenre_Should_MapGenreToNull()
    {
        var (sut, repo) = BuildSut();
        var band = MakeBand();
        repo.Setup(r => r.GetDetailAsync(band.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(band);

        var result = await sut.Handle(band.Id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Genre);
    }

    [Fact]
    public async Task Handle_BandWithNullOptionalFields_Should_MapAllNullableFieldsToNull()
    {
        var (sut, repo) = BuildSut();
        var band = MakeBand();
        repo.Setup(r => r.GetDetailAsync(band.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(band);

        var result = await sut.Handle(band.Id);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Location);
        Assert.Null(result.Value.Description);
        Assert.Null(result.Value.LogoImageUrl);
        Assert.Null(result.Value.BandImageUrl);
        Assert.Null(result.Value.MusicUrlPortal);
        Assert.Null(result.Value.BandContact);
    }
}
