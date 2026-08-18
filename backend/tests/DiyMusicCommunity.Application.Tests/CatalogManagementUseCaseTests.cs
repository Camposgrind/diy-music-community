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
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.SplitUp, DateTime.UtcNow);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetDetailAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var useCase = new CatalogManagementUseCase(repository.Object, Mock.Of<IGenreRepository>(), Mock.Of<IUnitOfWork>(), Mock.Of<IReleaseRepository>());

        var result = await useCase.CreateMember(band.Id, new MemberWriteRequest { Name = "Bones", IsLastKnownLineup = true, IsCurrent = false });

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidRequest, result.Error!.Code);
    }
}
