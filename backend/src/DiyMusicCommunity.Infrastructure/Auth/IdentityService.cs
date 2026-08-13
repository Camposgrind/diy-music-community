using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace DiyMusicCommunity.Infrastructure.Auth;

/// <summary>
/// Implements <see cref="IIdentityService"/> using ASP.NET Core Identity's <see cref="UserManager{TUser}"/>.
/// </summary>
public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(
        string username,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, "User");

        return (true, []);
    }

    public async Task<(bool Succeeded, Guid UserId, string Email, IEnumerable<string> Roles)> LoginAsync(
        string? email,
        string? username,
        string password,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            user = await _userManager.FindByEmailAsync(email);
        }

        if (user is null && !string.IsNullOrWhiteSpace(username))
        {
            user = await _userManager.FindByNameAsync(username);
        }

        if (user is null)
        {
            return (false, Guid.Empty, string.Empty, []);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            return (false, Guid.Empty, string.Empty, []);
        }

        var roles = await _userManager.GetRolesAsync(user);

        return (true, user.Id, user.Email!, roles);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user is not null;
    }
}
