namespace DiyMusicCommunity.Application.Auth.Login;

public sealed class LoginRequest
{
    public string? Email { get; init; }
    public string? Username { get; init; }
    public string Password { get; init; } = string.Empty;
}
