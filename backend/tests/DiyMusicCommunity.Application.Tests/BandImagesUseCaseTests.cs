using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.Images;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public sealed class BandImagesUseCaseTests
{
    [Fact]
    public async Task ConfirmBandImage_ValidTemporaryPhoto_ShouldPersistStablePathAndDeleteTemporaryFile()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.Active, DateTime.UtcNow);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetByIdAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var temporaryStorage = new Mock<ITemporaryImageStorage>();
        temporaryStorage.Setup(item => item.GetAsync("temporary-file", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TemporaryImageFile(
                "temporary-file",
                band.Id,
                BandImageType.BandPhoto.ToString(),
                "photo.png",
                "image/png",
                "png",
                [137, 80, 78, 71],
                DateTime.UtcNow.AddMinutes(15)));
        var blobStorage = new Mock<IBlobStorageService>();
        blobStorage.Setup(item => item.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), "image/png", It.IsAny<CancellationToken>()))
            .ReturnsAsync("bands/path");
        blobStorage.Setup(item => item.GenerateReadUriAsync("bands/path", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("https://example.test/bands/path?sig=read"));
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(item => item.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var useCase = new BandImagesUseCase(repository.Object, unitOfWork.Object, temporaryStorage.Object, blobStorage.Object, Mock.Of<IImageFileValidator>(), Mock.Of<IImageUploadSettings>());

        var result = await useCase.ConfirmAsync(band.Id, new ConfirmBandImageRequest(BandImageType.BandPhoto, "temporary-file"));

        Assert.True(result.IsSuccess);
        Assert.Equal("bands/path", band.BandPhotoBlobPath);
        Assert.Equal("https://example.test/bands/path?sig=read", result.Value!.ImageUrl);
        unitOfWork.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        temporaryStorage.Verify(item => item.DeleteAsync("temporary-file", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmBandImage_TemporaryFileForAnotherBand_ShouldNotUploadOrDeleteIt()
    {
        var band = new Band(Guid.NewGuid(), "Discharge", "UK", Guid.NewGuid(), BandStatus.Active, DateTime.UtcNow);
        var repository = new Mock<IBandRepository>();
        repository.Setup(item => item.GetByIdAsync(band.Id, It.IsAny<CancellationToken>())).ReturnsAsync(band);
        var temporaryStorage = new Mock<ITemporaryImageStorage>();
        temporaryStorage.Setup(item => item.GetAsync("temporary-file", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TemporaryImageFile(
                "temporary-file",
                Guid.NewGuid(),
                BandImageType.BandPhoto.ToString(),
                "photo.png",
                "image/png",
                "png",
                [137, 80, 78, 71],
                DateTime.UtcNow.AddMinutes(15)));
        var blobStorage = new Mock<IBlobStorageService>();
        var useCase = new BandImagesUseCase(repository.Object, Mock.Of<IUnitOfWork>(), temporaryStorage.Object, blobStorage.Object, Mock.Of<IImageFileValidator>(), Mock.Of<IImageUploadSettings>());

        var result = await useCase.ConfirmAsync(band.Id, new ConfirmBandImageRequest(BandImageType.BandPhoto, "temporary-file"));

        Assert.True(result.IsFailure);
        Assert.Equal(BandErrors.Codes.InvalidRequest, result.Error!.Code);
        blobStorage.Verify(item => item.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        temporaryStorage.Verify(item => item.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
