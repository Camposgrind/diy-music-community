using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Application.Auth;
using DiyMusicCommunity.Application.Auth.Login;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public sealed class LoginUseCaseTests
{
    [Fact]
    public async Task NonAdminCredentials_Should_ReturnInvalidCredentialsWithoutGeneratingToken()
    {
        var identityService = new Mock<IIdentityService>();
        identityService.Setup(service => service.LoginAsync("member@example.com", null, "Password1!", It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, Guid.NewGuid(), "member", "member@example.com", (IEnumerable<string>)["Member"]));
        var jwtTokenService = new Mock<IJwtTokenService>();
        var useCase = new LoginUseCase(identityService.Object, jwtTokenService.Object, new LoginRequestValidator());

        var result = await useCase.Handle(new LoginRequest { Email = "member@example.com", Password = "Password1!" });

        Assert.True(result.IsFailure);
        Assert.Equal(AuthErrors.Codes.InvalidCredentials, result.Error!.Code);
        jwtTokenService.Verify(service => service.GenerateToken(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task AdminCredentials_Should_ReturnToken()
    {
        var userId = Guid.NewGuid();
        var identityService = new Mock<IIdentityService>();
        identityService.Setup(service => service.LoginAsync("admin@example.com", null, "Password1!", It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, userId, "admin", "admin@example.com", (IEnumerable<string>)["Admin"]));
        var jwtTokenService = new Mock<IJwtTokenService>();
        jwtTokenService.Setup(service => service.GenerateToken(userId, "admin", "admin@example.com", It.IsAny<IEnumerable<string>>()))
            .Returns(("jwt-token", DateTime.UtcNow.AddHours(1)));
        var useCase = new LoginUseCase(identityService.Object, jwtTokenService.Object, new LoginRequestValidator());

        var result = await useCase.Handle(new LoginRequest { Email = "admin@example.com", Password = "Password1!" });

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value!.Token);
    }
}
