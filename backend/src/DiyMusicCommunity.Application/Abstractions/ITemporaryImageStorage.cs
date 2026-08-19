namespace DiyMusicCommunity.Application.Abstractions;

public interface ITemporaryImageStorage
{
    Task SaveAsync(TemporaryImageFile file, CancellationToken cancellationToken = default);
    Task<TemporaryImageFile?> GetAsync(string temporaryFileId, CancellationToken cancellationToken = default);
    Task DeleteAsync(string temporaryFileId, CancellationToken cancellationToken = default);
    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
}

public sealed class TemporaryImageFile
{
    public TemporaryImageFile(string id, Guid ownerId, string imageType, string originalFileName, string contentType, string extension, byte[] content, DateTime expiresAtUtc)
    {
        Id = id;
        OwnerId = ownerId;
        ImageType = imageType;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        Extension = extension;
        Content = content;
        ExpiresAtUtc = expiresAtUtc;
    }

    public string Id { get; }
    public Guid OwnerId { get; }
    public string ImageType { get; }
    public string OriginalFileName { get; }
    public string ContentType { get; }
    public string Extension { get; }
    public byte[] Content { get; }
    public DateTime ExpiresAtUtc { get; }
}
