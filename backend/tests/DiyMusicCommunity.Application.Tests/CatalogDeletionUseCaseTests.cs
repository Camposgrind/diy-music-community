using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.CatalogDeletion;
using DiyMusicCommunity.Domain.Abstractions;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public sealed class CatalogDeletionUseCaseTests
{
    [Fact]
    public async Task DeleteBand_ExistingBand_Should_DeleteAndReturnSuccess()
    {
        var repository = new Mock<ICatalogDeletionRepository>();
        repository.Setup(item => item.DeleteBandAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var useCase = new CatalogDeletionUseCase(repository.Object);

        var result = await useCase.DeleteBand(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        repository.Verify(item => item.DeleteBandAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMember_UnknownMember_Should_ReturnNotFound()
    {
        var repository = new Mock<ICatalogDeletionRepository>();
        repository.Setup(item => item.DeleteMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var useCase = new CatalogDeletionUseCase(repository.Object);

        var result = await useCase.DeleteMember(Guid.NewGuid(), Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Member.NotFound", result.Error!.Code);
    }

    [Fact]
    public async Task DeleteRelease_UnknownRelease_Should_ReturnNotFound()
    {
        var repository = new Mock<ICatalogDeletionRepository>();
        repository.Setup(item => item.DeleteReleaseAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var useCase = new CatalogDeletionUseCase(repository.Object);

        var result = await useCase.DeleteRelease(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Release.NotFound", result.Error!.Code);
    }

    [Fact]
    public async Task DeleteTrack_ExistingTrack_Should_DeleteAndRenumber()
    {
        var repository = new Mock<ICatalogDeletionRepository>();
        repository.Setup(item => item.DeleteTrackAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var useCase = new CatalogDeletionUseCase(repository.Object);

        var result = await useCase.DeleteTrack(Guid.NewGuid(), Guid.NewGuid());

        Assert.True(result.IsSuccess);
        repository.Verify(item => item.DeleteTrackAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
