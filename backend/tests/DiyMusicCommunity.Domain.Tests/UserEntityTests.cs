using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Tests;

public class UserEntityTests
{
    private static User CreateUser(
        string email = "user@example.com",
        string passwordHash = "hashed",
        string displayName = "Test User") =>
        new(Guid.NewGuid(), email, passwordHash, displayName, DateTime.UtcNow);

    [Fact]
    public void NewUser_Should_HaveDefaultRoleUser()
    {
        var user = CreateUser();

        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void NewUser_Should_StoreProvidedEmailAndDisplayName()
    {
        var user = CreateUser(email: "punk@diy.com", displayName: "Sid");

        Assert.Equal("punk@diy.com", user.Email);
        Assert.Equal("Sid", user.DisplayName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewUser_WithEmptyEmail_Should_ThrowArgumentException(string email)
    {
        Assert.Throws<ArgumentException>(() => CreateUser(email: email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewUser_WithEmptyDisplayName_Should_ThrowArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => CreateUser(displayName: name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NewUser_WithEmptyPasswordHash_Should_ThrowArgumentException(string hash)
    {
        Assert.Throws<ArgumentException>(() => CreateUser(passwordHash: hash));
    }

    [Fact]
    public void NewUser_WithEmptyGuid_Should_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new User(Guid.Empty, "user@example.com", "hash", "Name", DateTime.UtcNow));
    }

    [Fact]
    public void ChangeRole_Should_UpdateUserRole()
    {
        var user = CreateUser();

        user.ChangeRole(UserRole.Moderator);

        Assert.Equal(UserRole.Moderator, user.Role);
    }
}
