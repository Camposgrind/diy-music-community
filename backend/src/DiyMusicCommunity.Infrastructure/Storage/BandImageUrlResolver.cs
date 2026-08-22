using DiyMusicCommunity.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class BandImageUrlResolver : IImageUrlResolver
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<BandImageUrlResolver> _logger;

    public BandImageUrlResolver(IBlobStorageService blobStorageService, ILogger<BandImageUrlResolver> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    public async Task<string?> ResolveAsync(string? blobPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
        {
            return null;
        }

        try
        {
            if (!await _blobStorageService.ExistsAsync(blobPath, cancellationToken))
            {
                _logger.LogWarning("Band image blob {BlobPath} does not exist.", blobPath);

                return null;
            }

            var uri = await _blobStorageService.GenerateReadUriAsync(blobPath, cancellationToken);

            return uri.ToString();
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Band image blob {BlobPath} could not be resolved.", blobPath);
            return null;
        }
    }
}
