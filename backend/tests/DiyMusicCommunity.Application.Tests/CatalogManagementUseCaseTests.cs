using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.CatalogManagement;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public sealed class CatalogManagementUseCaseTests
{
    [Fact]
    public async Task CreateBand_SplitUpWithoutYear_Should_ReturnInvalidRequest()
    {
        var genreId = Guid.NewGuid();
        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(item => item.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Genre(genreId, "Grindcore")]);
        var useCase = new CatalogManagementUseCase(Mock.Of<IBandRepository>(), genreRepository.Object, Mock.Of<IUnitOfWork>(), Mock.Of<IReleaseRepository>());

        var result = await useCase.CreateBand(new BandWriteRequest
        {
            Name = "Nasum",
            Country = "Sweden",
            GenreId = genreId,
            Status = BandStatus.SplitUp,
            SplitUpYear = null
        });

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidRequest, result.Error!.Code);
    }

    [Fact]
    public async Task CreateRelease_WithTracks_Should_PersistReleaseAndCompleteTrackList()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.Active, DateTime.UtcNow);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetDetailAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var releaseRepository = new Mock<IReleaseRepository>();
        releaseRepository.Setup(item => item.GetDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Guid id, CancellationToken _) => band.Releases.Single(item => item.Id == id));
        var useCase = new CatalogManagementUseCase(repository.Object, Mock.Of<IGenreRepository>(), unitOfWork.Object, releaseRepository.Object);
        var request = new ReleaseWriteRequest
        {
            Title = "Hear Nothing See Nothing Say Nothing",
            ReleaseType = ReleaseType.Album,
            Year = 1982,
            Tracks = [new TrackWriteRequest { Title = "Hear Nothing" }, new TrackWriteRequest { Title = "Protest and Survive" }]
        };

        var result = await useCase.CreateRelease(band.Id, request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Tracks.Count);
        Assert.Single(band.Releases);
        Assert.Equal(2, band.Releases[0].Tracks.Count);
        unitOfWork.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateReleaseTracks_WithOrderedTracks_Should_ReplaceTracksAndAssignConsecutiveNumbers()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.Active, DateTime.UtcNow);
        var release = new Release(Guid.NewGuid(), band.Id, "Hear Nothing", ReleaseType.Album);
        release.ReplaceTracks([("Old Track", 1)]);
        band.AddRelease(release);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetDetailAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var releaseRepository = new Mock<IReleaseRepository>();
        releaseRepository.Setup(item => item.GetDetailAsync(release.Id, It.IsAny<CancellationToken>())).ReturnsAsync(release);
        var useCase = new CatalogManagementUseCase(repository.Object, Mock.Of<IGenreRepository>(), unitOfWork.Object, releaseRepository.Object);

        var result = await useCase.UpdateReleaseTracks(band.Id, release.Id, new TrackListWriteRequest
        {
            Tracks = [new TrackWriteRequest { Title = "Second" }, new TrackWriteRequest { Title = "First" }]
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(["Second", "First"], release.Tracks.OrderBy(track => track.TrackNumber).Select(track => track.Title));
        Assert.Equal([1, 2], release.Tracks.OrderBy(track => track.TrackNumber).Select(track => track.TrackNumber));
        unitOfWork.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateMember_WithDuplicateIdentity_Should_ReturnConflict()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.Active, DateTime.UtcNow);
        var existing = new BandMember(Guid.NewGuid(), band.Id, "Bones", true);
        existing.SetYears(1977, null);
        band.AddMember(existing);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetDetailAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var useCase = new CatalogManagementUseCase(repository.Object, Mock.Of<IGenreRepository>(), Mock.Of<IUnitOfWork>(), Mock.Of<IReleaseRepository>());

        var result = await useCase.CreateMember(band.Id, new MemberWriteRequest { Name = " bones ", StartYear = 1977, IsCurrent = true });

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.Duplicate, result.Error!.Code);
    }

    [Fact]
    public async Task CreateMember_WithLastKnownLineupAndNoEndYear_Should_ReturnInvalidRequest()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.SplitUp, DateTime.UtcNow, 1983);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetDetailAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var useCase = new CatalogManagementUseCase(repository.Object, Mock.Of<IGenreRepository>(), Mock.Of<IUnitOfWork>(), Mock.Of<IReleaseRepository>());

        var result = await useCase.CreateMember(band.Id, new MemberWriteRequest { Name = "Bones", IsLastKnownLineup = true, IsCurrent = false });

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidRequest, result.Error!.Code);
    }

    [Fact]
    public async Task UpdateBand_SplitUpToActive_Should_MoveLastKnownLineupToPastMembers()
    {
        var genreId = Guid.NewGuid();
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", genreId, BandStatus.SplitUp, DateTime.UtcNow, 1986);
        var member = new BandMember(Guid.NewGuid(), band.Id, "Bones", false);
        member.Update("Bones", "Bass", 1980, 1986, false, true);
        band.AddMember(member);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetDetailAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var genreRepository = new Mock<IGenreRepository>();
        genreRepository.Setup(item => item.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([new Genre(genreId, "Punk")]);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var useCase = new CatalogManagementUseCase(repository.Object, genreRepository.Object, unitOfWork.Object, Mock.Of<IReleaseRepository>());

        var result = await useCase.UpdateBand(band.Id, new BandWriteRequest { Name = "Discharge", Country = "UK", GenreId = genreId, Status = BandStatus.Active });

        Assert.True(result.IsSuccess);
        Assert.False(member.IsLastKnownLineup);
        Assert.False(member.IsCurrent);
        unitOfWork.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
