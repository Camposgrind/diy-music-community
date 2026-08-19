namespace DiyMusicCommunity.Application.Abstractions;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string blobPath, string contentType, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string blobPath, CancellationToken cancellationToken = default);
    Task<Uri> GenerateReadUriAsync(string blobPath, CancellationToken cancellationToken = default);
    Task DeleteIfExistsAsync(string blobPath, CancellationToken cancellationToken = default);
}
