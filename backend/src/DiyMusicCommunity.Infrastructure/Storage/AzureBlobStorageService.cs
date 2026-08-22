using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DiyMusicCommunity.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class AzureBlobStorageService : IBlobStorageService
{
    private readonly AzureStorageOptions _options;

    public AzureBlobStorageService(IOptions<AzureStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(Stream stream, string blobPath, string contentType, CancellationToken cancellationToken = default)
    {
        var blob = GetContainerClient().GetBlobClient(blobPath);

        await blob.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return blobPath;
    }

    public Task<Uri> GenerateReadUriAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var blob = GetContainerClient().GetBlobClient(blobPath);

        if (!blob.CanGenerateSasUri)
        {
            throw new InvalidOperationException("Azure Storage must be configured with a credential capable of generating read SAS URLs.");
        }

        var builder = new BlobSasBuilder
        {
            BlobContainerName = _options.ContainerName,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(_options.SasLifetimeDays)
        };

        builder.SetPermissions(BlobSasPermissions.Read);

        return Task.FromResult(blob.GenerateSasUri(builder));
    }

    public async Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var response = await GetContainerClient().GetBlobClient(blobPath).ExistsAsync(cancellationToken);

        return response.Value;
    }

    public async Task DeleteIfExistsAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        await GetContainerClient().DeleteBlobIfExistsAsync(blobPath, cancellationToken: cancellationToken);
    }

    private BlobContainerClient GetContainerClient()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString) || _options.ConnectionString.StartsWith("SET_VIA_", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Azure Storage connection string is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.ContainerName) || _options.SasLifetimeDays <= 0)
        {
            throw new InvalidOperationException("Azure Storage container name and SAS lifetime must be configured.");
        }

        var serviceClient = new BlobServiceClient(_options.ConnectionString);

        return serviceClient.GetBlobContainerClient(_options.ContainerName);
    }
}
