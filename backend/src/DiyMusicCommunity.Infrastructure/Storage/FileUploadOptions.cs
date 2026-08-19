namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class FileUploadOptions
{
    public const string SectionName = "FileUpload";

    public int MaxImageSizeMb { get; set; }
    public int TemporaryFileLifetimeMinutes { get; set; }
    public string? TemporaryDirectory { get; set; }
}
