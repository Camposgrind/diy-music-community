namespace DiyMusicCommunity.Application.Auth.Login;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
