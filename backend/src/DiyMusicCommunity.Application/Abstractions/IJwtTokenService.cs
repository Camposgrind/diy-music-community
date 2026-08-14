namespace DiyMusicCommunity.Application.Abstractions;

/// <summary>
/// Abstracts JWT token generation so Application has no reference to JWT packages.
/// </summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Guid userId, string username, string email, IEnumerable<string> roles);
}
