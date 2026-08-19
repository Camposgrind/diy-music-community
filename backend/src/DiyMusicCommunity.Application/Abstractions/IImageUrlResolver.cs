namespace DiyMusicCommunity.Application.Abstractions;

public interface IImageUrlResolver
{
    Task<string?> ResolveAsync(string? blobPath, CancellationToken cancellationToken = default);
}
