using DiyMusicCommunity.Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class ImageFileValidatorTests
{
    [Fact]
    public void Validate_PngMagicBytesWithAllowedExtension_ShouldDetectPng()
    {
        var validator = CreateValidator();

        var result = validator.Validate([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "photo.png", "image/png");

        Assert.True(result.IsValid);
        Assert.Equal("image/png", result.ContentType);
        Assert.Equal("png", result.Extension);
    }

    [Fact]
    public void Validate_FileNamedAsImageWithInvalidMagicBytes_ShouldRejectIt()
    {
        var validator = CreateValidator();

        var result = validator.Validate([0x00, 0x11, 0x22], "malicious.jpg", "image/jpeg");

        Assert.False(result.IsValid);
        Assert.NotNull(result.RejectionReason);
    }

    private static ImageFileValidator CreateValidator()
    {
        return new ImageFileValidator(Options.Create(new FileUploadOptions
        {
            MaxImageSizeMb = 5,
            TemporaryFileLifetimeMinutes = 30
        }));
    }
}
