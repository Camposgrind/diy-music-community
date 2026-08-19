using DiyMusicCommunity.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class ImageFileValidator : IImageFileValidator
{
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
    private readonly FileUploadOptions _options;

    public ImageFileValidator(IOptions<FileUploadOptions> options)
    {
        _options = options.Value;
    }

    public ImageValidationResult Validate(byte[] content, string originalFileName, string? declaredContentType)
    {
        if (content.Length == 0)
        {
            return Reject("The image file is empty.");
        }

        var maxBytes = (long)_options.MaxImageSizeMb * 1024 * 1024;
        if (maxBytes <= 0 || content.LongLength > maxBytes)
        {
            return Reject("The image file exceeds the configured maximum size.");
        }

        var extension = Path.GetExtension(originalFileName).TrimStart('.');
        if (!string.Equals(extension, "png", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, "jpg", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, "jpeg", StringComparison.OrdinalIgnoreCase))
        {
            return Reject("Only .png, .jpg, and .jpeg file extensions are allowed.");
        }

        if (content.AsSpan().StartsWith(PngSignature))
        {
            return Accept("image/png", "png", content.LongLength);
        }

        if (content.AsSpan().StartsWith(JpegSignature))
        {
            return Accept("image/jpeg", "jpg", content.LongLength);
        }

        return Reject("Only PNG and JPEG image files are allowed.");
    }

    private static ImageValidationResult Accept(string contentType, string extension, long size)
    {
        return new ImageValidationResult
        {
            IsValid = true,
            ContentType = contentType,
            Extension = extension,
            Size = size
        };
    }

    private static ImageValidationResult Reject(string reason)
    {
        return new ImageValidationResult
        {
            IsValid = false,
            RejectionReason = reason
        };
    }
}
