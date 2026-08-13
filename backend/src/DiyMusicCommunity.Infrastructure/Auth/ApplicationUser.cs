using Microsoft.AspNetCore.Identity;

namespace DiyMusicCommunity.Infrastructure.Auth;

/// <summary>
/// The single user entity for the application. Extends IdentityUser with
/// domain-specific properties. Identity manages email, password hash, and username.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }
}
