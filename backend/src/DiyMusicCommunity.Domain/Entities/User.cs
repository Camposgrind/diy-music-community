using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Entities;

public sealed class User : Entity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string DisplayName { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User(Guid id, string email, string passwordHash, string displayName, DateTime createdAt)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty.", nameof(email));
        }
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
        }
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name cannot be empty.", nameof(displayName));
        }

        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        Role = UserRole.User;
        CreatedAt = createdAt;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }
}
