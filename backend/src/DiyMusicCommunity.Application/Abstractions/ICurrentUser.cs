namespace DiyMusicCommunity.Application.Abstractions;

/// <summary>
/// Provides access to the currently authenticated user's context.
/// Implemented in the Api layer via <c>HttpContext</c>.
/// </summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
