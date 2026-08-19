using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Bands.Images;

public sealed class UploadTemporaryBandImageRequest
{
    public BandImageType ImageType { get; init; }
    public string OriginalFileName { get; init; } = string.Empty;
    public string? DeclaredContentType { get; init; }
    public byte[] Content { get; init; } = [];
}

public sealed class ConfirmBandImageRequest
{
    public ConfirmBandImageRequest(BandImageType imageType, string temporaryFileId)
    {
        ImageType = imageType;
        TemporaryFileId = temporaryFileId;
    }

    public BandImageType ImageType { get; }
    public string TemporaryFileId { get; }
}

public sealed class TemporaryBandImageModel
{
    public string TemporaryFileId { get; init; } = string.Empty;
    public string OriginalFileName { get; init; } = string.Empty;
    public string SanitizedFileName { get; init; } = string.Empty;
    public string DetectedContentType { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public long Size { get; init; }
    public string? PreviewUrl { get; init; }
}

public sealed class ConfirmBandImageModel
{
    public Guid BandId { get; init; }
    public BandImageType ImageType { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string BlobPath { get; init; } = string.Empty;
}
