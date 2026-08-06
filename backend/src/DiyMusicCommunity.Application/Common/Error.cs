namespace DiyMusicCommunity.Application.Common;

public sealed class Error
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;

    // Parameterless constructor required for JSON deserialization
    public Error() { }

    private Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static Error Validation(string code, string message) => new(code, message);
    public static Error NotFound(string code, string message) => new(code, message);
    public static Error Conflict(string code, string message) => new(code, message);
    public static Error UnprocessableEntity(string code, string message) => new(code, message);
}
