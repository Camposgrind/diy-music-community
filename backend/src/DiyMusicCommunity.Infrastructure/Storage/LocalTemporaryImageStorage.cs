using System.Text.Json;
using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.Extensions.Options;

namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class LocalTemporaryImageStorage : ITemporaryImageStorage
{
    private readonly string _rootDirectory;

    public LocalTemporaryImageStorage(IOptions<FileUploadOptions> options)
    {
        var configuredDirectory = options.Value.TemporaryDirectory;
        _rootDirectory = string.IsNullOrWhiteSpace(configuredDirectory)
            ? Path.Combine(AppContext.BaseDirectory, "App_Data", "temporary-images")
            : Path.GetFullPath(configuredDirectory);
        Directory.CreateDirectory(_rootDirectory);
    }

    public async Task SaveAsync(TemporaryImageFile file, CancellationToken cancellationToken = default)
    {
        var paths = GetPaths(file.Id);

        await File.WriteAllBytesAsync(paths.ContentPath, file.Content, cancellationToken);

        var metadata = new TemporaryImageMetadata
        {
            Id = file.Id,
            OwnerId = file.OwnerId,
            ImageType = file.ImageType,
            OriginalFileName = file.OriginalFileName,
            ContentType = file.ContentType,
            Extension = file.Extension,
            ExpiresAtUtc = file.ExpiresAtUtc
        };

        await using var stream = File.Create(paths.MetadataPath);

        await JsonSerializer.SerializeAsync(stream, metadata, cancellationToken: cancellationToken);
    }

    public async Task<TemporaryImageFile?> GetAsync(string temporaryFileId, CancellationToken cancellationToken = default)
    {
        if (!IsSafeId(temporaryFileId))
        {
            return null;
        }

        var paths = GetPaths(temporaryFileId);
        if (!File.Exists(paths.ContentPath) || !File.Exists(paths.MetadataPath))
        {
            return null;
        }

        await using var stream = File.OpenRead(paths.MetadataPath);

        var metadata = await JsonSerializer.DeserializeAsync<TemporaryImageMetadata>(stream, cancellationToken: cancellationToken);

        if (metadata is null || metadata.Id != temporaryFileId)
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(paths.ContentPath, cancellationToken);

        return new TemporaryImageFile(metadata.Id, metadata.OwnerId, metadata.ImageType, metadata.OriginalFileName, metadata.ContentType, metadata.Extension, content, metadata.ExpiresAtUtc);
    }

    public Task DeleteAsync(string temporaryFileId, CancellationToken cancellationToken = default)
    {
        if (!IsSafeId(temporaryFileId))
        {
            return Task.CompletedTask;
        }

        var paths = GetPaths(temporaryFileId);

        DeleteIfExists(paths.ContentPath);
        DeleteIfExists(paths.MetadataPath);

        return Task.CompletedTask;
    }

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        foreach (var metadataPath in Directory.EnumerateFiles(_rootDirectory, "*.json"))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await using var stream = File.OpenRead(metadataPath);

                var metadata = await JsonSerializer.DeserializeAsync<TemporaryImageMetadata>(stream, cancellationToken: cancellationToken);

                if (metadata is null || metadata.ExpiresAtUtc <= DateTime.UtcNow)
                {
                    var temporaryFileId = Path.GetFileNameWithoutExtension(metadataPath);

                    await DeleteAsync(temporaryFileId, cancellationToken);
                }
            }
            catch (JsonException)
            {
                // Metadata is unreadable/incompatible — treat as expired and delete
                var temporaryFileId = Path.GetFileNameWithoutExtension(metadataPath);

                await DeleteAsync(temporaryFileId, cancellationToken);
            }
        }
    }

    private (string ContentPath, string MetadataPath) GetPaths(string temporaryFileId)
    {
        var safeId = Path.GetFileName(temporaryFileId);

        return (Path.Combine(_rootDirectory, $"{safeId}.bin"), Path.Combine(_rootDirectory, $"{safeId}.json"));
    }

    private static bool IsSafeId(string temporaryFileId)
    {
        return Guid.TryParseExact(temporaryFileId, "N", out _);
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private sealed class TemporaryImageMetadata
    {
        public string Id { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public string ImageType { get; init; } = string.Empty;
        public string OriginalFileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public string Extension { get; init; } = string.Empty;
        public DateTime ExpiresAtUtc { get; init; }
    }
}
