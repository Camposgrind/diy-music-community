using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Releases.Images;

public sealed class ReleaseImagesUseCase
{
    private readonly IReleaseRepository _releaseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITemporaryImageStorage _temporaryImageStorage;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IImageFileValidator _imageFileValidator;
    private readonly IImageUploadSettings _imageUploadSettings;

    public ReleaseImagesUseCase(IReleaseRepository releaseRepository, IUnitOfWork unitOfWork, ITemporaryImageStorage temporaryImageStorage, IBlobStorageService blobStorageService, IImageFileValidator imageFileValidator, IImageUploadSettings imageUploadSettings)
    { _releaseRepository = releaseRepository; _unitOfWork = unitOfWork; _temporaryImageStorage = temporaryImageStorage; _blobStorageService = blobStorageService; _imageFileValidator = imageFileValidator; _imageUploadSettings = imageUploadSettings; }

    public async Task<Result<TemporaryReleaseImageModel>> UploadTemporaryAsync(Guid releaseId, UploadTemporaryReleaseImageRequest request, CancellationToken cancellationToken = default)
    {
        if (await _releaseRepository.GetDetailAsync(releaseId, cancellationToken) is null) { return Result<TemporaryReleaseImageModel>.Failure(ReleaseErrors.NotFound(releaseId)); }
        var validation = _imageFileValidator.Validate(request.Content, request.OriginalFileName, request.DeclaredContentType);
        if (!validation.IsValid) { return Result<TemporaryReleaseImageModel>.Failure(Error.Validation("Release.InvalidImage", validation.RejectionReason ?? "The image file is invalid.")); }
        await _temporaryImageStorage.DeleteExpiredAsync(cancellationToken);
        var id = Guid.NewGuid().ToString("N");
        await _temporaryImageStorage.SaveAsync(new TemporaryImageFile(id, releaseId, ReleaseImageType.ReleaseCover.ToString(), Path.GetFileName(request.OriginalFileName), validation.ContentType!, validation.Extension!, request.Content, DateTime.UtcNow.Add(_imageUploadSettings.TemporaryFileLifetime)), cancellationToken);
        return Result<TemporaryReleaseImageModel>.Success(new TemporaryReleaseImageModel { TemporaryFileId = id, OriginalFileName = Path.GetFileName(request.OriginalFileName), SanitizedFileName = $"{id}.{validation.Extension}", DetectedContentType = validation.ContentType!, Extension = validation.Extension!, Size = validation.Size });
    }

    public async Task<Result<ConfirmReleaseImageModel>> ConfirmAsync(Guid releaseId, ConfirmReleaseImageRequest request, CancellationToken cancellationToken = default)
    {
        var release = await _releaseRepository.GetDetailAsync(releaseId, cancellationToken);
        if (release is null) { return Result<ConfirmReleaseImageModel>.Failure(ReleaseErrors.NotFound(releaseId)); }
        var temporary = await _temporaryImageStorage.GetAsync(request.TemporaryFileId, cancellationToken);
        if (temporary is null || temporary.ExpiresAtUtc <= DateTime.UtcNow || temporary.OwnerId != releaseId || temporary.ImageType != ReleaseImageType.ReleaseCover.ToString()) { return Result<ConfirmReleaseImageModel>.Failure(Error.Validation("Release.InvalidImage", "The temporary release cover is invalid or expired.")); }
        var path = $"releases/{releaseId:D}/cover/{Guid.NewGuid():N}.{temporary.Extension}";
        try { await using var stream = new MemoryStream(temporary.Content, false); await _blobStorageService.UploadAsync(stream, path, temporary.ContentType, cancellationToken); }
        catch { return Result<ConfirmReleaseImageModel>.Failure(Error.Validation("Release.Storage", "The image could not be uploaded to storage.")); }
        var previous = release.ReleaseCoverBlobPath;
        release.SetCoverBlobPath(path);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _temporaryImageStorage.DeleteAsync(temporary.Id, cancellationToken);
        if (!string.IsNullOrWhiteSpace(previous)) { try { await _blobStorageService.DeleteIfExistsAsync(previous, cancellationToken); } catch { } }
        var url = await _blobStorageService.GenerateReadUriAsync(path, cancellationToken);
        return Result<ConfirmReleaseImageModel>.Success(new ConfirmReleaseImageModel { ReleaseId = releaseId, ImageType = ReleaseImageType.ReleaseCover, ImageUrl = url.ToString() });
    }
}

public sealed class UploadTemporaryReleaseImageRequest { public string OriginalFileName { get; init; } = string.Empty; public string? DeclaredContentType { get; init; } public byte[] Content { get; init; } = []; }
public sealed class ConfirmReleaseImageRequest { public string TemporaryFileId { get; init; } = string.Empty; }
public sealed class TemporaryReleaseImageModel { public string TemporaryFileId { get; init; } = string.Empty; public string OriginalFileName { get; init; } = string.Empty; public string SanitizedFileName { get; init; } = string.Empty; public string DetectedContentType { get; init; } = string.Empty; public string Extension { get; init; } = string.Empty; public long Size { get; init; } }
public sealed class ConfirmReleaseImageModel { public Guid ReleaseId { get; init; } public ReleaseImageType ImageType { get; init; } public string ImageUrl { get; init; } = string.Empty; }
