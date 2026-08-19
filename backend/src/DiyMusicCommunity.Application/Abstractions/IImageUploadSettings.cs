namespace DiyMusicCommunity.Application.Abstractions;

public interface IImageUploadSettings
{
    TimeSpan TemporaryFileLifetime { get; }
}
