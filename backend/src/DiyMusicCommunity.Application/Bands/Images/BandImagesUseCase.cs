using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Application.Bands.Images;

public sealed class BandImagesUseCase
{
    private readonly IBandRepository _bandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITemporaryImageStorage _temporaryImageStorage;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IImageFileValidator _imageFileValidator;
    private readonly IImageUploadSettings _imageUploadSettings;

    public BandImagesUseCase(IBandRepository bandRepository, IUnitOfWork unitOfWork, ITemporaryImageStorage temporaryImageStorage, IBlobStorageService blobStorageService, IImageFileValidator imageFileValidator, IImageUploadSettings imageUploadSettings)
    {
        _bandRepository = bandRepository;
        _unitOfWork = unitOfWork;
        _temporaryImageStorage = temporaryImageStorage;
        _blobStorageService = blobStorageService;
        _imageFileValidator = imageFileValidator;
        _imageUploadSettings = imageUploadSettings;
    }

    public async Task<Result<TemporaryBandImageModel>> UploadTemporaryAsync(Guid bandId, UploadTemporaryBandImageRequest request, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetByIdAsync(bandId, cancellationToken);
        if (band is null)
        {
            return Result<TemporaryBandImageModel>.Failure(BandErrors.NotFound(bandId));
        }

        if (!Enum.IsDefined(request.ImageType))
        {
            return Result<TemporaryBandImageModel>.Failure(BandErrors.InvalidRequest("The image type is invalid."));
        }

        var originalFileName = SanitizeFileName(request.OriginalFileName);
        var validation = _imageFileValidator.Validate(request.Content, originalFileName, request.DeclaredContentType);
        if (!validation.IsValid)
        {
            return Result<TemporaryBandImageModel>.Failure(BandErrors.InvalidRequest(validation.RejectionReason ?? "The image file is invalid."));
        }

        await _temporaryImageStorage.DeleteExpiredAsync(cancellationToken);
        var temporaryFileId = Guid.NewGuid().ToString("N");
        var sanitizedFileName = $"{temporaryFileId}.{validation.Extension}";
        var temporaryFile = new TemporaryImageFile(temporaryFileId, bandId, request.ImageType.ToString(), originalFileName, validation.ContentType!, validation.Extension!, request.Content, DateTime.UtcNow.Add(_imageUploadSettings.TemporaryFileLifetime));
        await _temporaryImageStorage.SaveAsync(temporaryFile, cancellationToken);

        return Result<TemporaryBandImageModel>.Success(new TemporaryBandImageModel
        {
            TemporaryFileId = temporaryFileId,
            OriginalFileName = originalFileName,
            SanitizedFileName = sanitizedFileName,
            DetectedContentType = validation.ContentType!,
            Extension = validation.Extension!,
            Size = validation.Size
        });
    }

    public async Task<Result<ConfirmBandImageModel>> ConfirmAsync(Guid bandId, ConfirmBandImageRequest request, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(request.ImageType) || string.IsNullOrWhiteSpace(request.TemporaryFileId))
        {
            return Result<ConfirmBandImageModel>.Failure(BandErrors.InvalidRequest("The image type and temporary file id are required."));
        }

        var band = await _bandRepository.GetByIdAsync(bandId, cancellationToken);
        if (band is null)
        {
            return Result<ConfirmBandImageModel>.Failure(BandErrors.NotFound(bandId));
        }

        await _temporaryImageStorage.DeleteExpiredAsync(cancellationToken);
        var temporaryFile = await _temporaryImageStorage.GetAsync(request.TemporaryFileId, cancellationToken);
        if (temporaryFile is null || temporaryFile.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result<ConfirmBandImageModel>.Failure(BandErrors.InvalidRequest("The temporary image file was not found or has expired."));
        }

        if (temporaryFile.OwnerId != bandId || temporaryFile.ImageType != request.ImageType.ToString())
        {
            return Result<ConfirmBandImageModel>.Failure(BandErrors.InvalidRequest("The temporary image file does not belong to this band and image type."));
        }

        var category = request.ImageType == BandImageType.BandPhoto ? "photo" : "logo";
        var blobPath = $"bands/{bandId:D}/{category}/{Guid.NewGuid():N}.{temporaryFile.Extension}";
        string uploadedPath;
        try
        {
            await using var contentStream = new MemoryStream(temporaryFile.Content, writable: false);
            uploadedPath = await _blobStorageService.UploadAsync(contentStream, blobPath, temporaryFile.ContentType, cancellationToken);
        }
        catch
        {
            return Result<ConfirmBandImageModel>.Failure(BandErrors.InvalidRequest("The image could not be uploaded to storage."));
        }

        var previousPath = request.ImageType == BandImageType.BandPhoto ? band.BandPhotoBlobPath : band.LogoImageBlobPath;
        band.SetImageBlobPath(request.ImageType, uploadedPath);
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _blobStorageService.DeleteIfExistsAsync(uploadedPath, cancellationToken);
            return Result<ConfirmBandImageModel>.Failure(BandErrors.InvalidRequest("The image path could not be persisted."));
        }
        await _temporaryImageStorage.DeleteAsync(temporaryFile.Id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousPath) && !string.Equals(previousPath, uploadedPath, StringComparison.Ordinal))
        {
            try
            {
                await _blobStorageService.DeleteIfExistsAsync(previousPath, cancellationToken);
            }
            catch
            {
            }
        }

        var readUri = await _blobStorageService.GenerateReadUriAsync(uploadedPath, cancellationToken);
        return Result<ConfirmBandImageModel>.Success(new ConfirmBandImageModel
        {
            BandId = bandId,
            ImageType = request.ImageType,
            BlobPath = uploadedPath,
            ImageUrl = readUri.ToString()
        });
    }

    private static string SanitizeFileName(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            return "image";
        }

        return string.Concat(safeFileName.Select(character => Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));
    }

}
