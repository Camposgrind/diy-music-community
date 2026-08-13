using System.Security.Claims;
using DiyMusicCommunity.Application.Abstractions;

namespace DiyMusicCommunity.Api.Services;

/// <summary>
/// Provides the currently authenticated user's context from <see cref="IHttpContextAccessor"/>.
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User;
        }
    }

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email
    {
        get
        {
            return User?.FindFirstValue(ClaimTypes.Email)
                   ?? User?.FindFirstValue("email");
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return User?.Identity?.IsAuthenticated == true;
        }
    }

    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) == true;
    }
}
