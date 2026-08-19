namespace DiyMusicCommunity.Application.Abstractions;

public interface IImageFileValidator
{
    ImageValidationResult Validate(byte[] content, string originalFileName, string? declaredContentType);
}

public sealed class ImageValidationResult
{
    public bool IsValid { get; init; }
    public string? ContentType { get; init; }
    public string? Extension { get; init; }
    public long Size { get; init; }
    public string? RejectionReason { get; init; }
}
