namespace DiyMusicCommunity.Application.Abstractions;

/// <summary>
/// Abstracts ASP.NET Core Identity operations so Application has no reference to Identity packages.
/// </summary>
public interface IIdentityService
{
    Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(
        string username,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, Guid UserId, string Email, IEnumerable<string> Roles)> LoginAsync(
        string? email,
        string? username,
        string password,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
