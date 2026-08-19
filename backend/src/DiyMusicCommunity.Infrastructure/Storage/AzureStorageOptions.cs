namespace DiyMusicCommunity.Infrastructure.Storage;

public sealed class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";

    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public int SasLifetimeDays { get; set; }
}
