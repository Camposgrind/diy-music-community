using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Login_NonAdminCredentials_Should_ReturnInvalidCredentials()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClientWithDb();
        var email = $"member-{Guid.NewGuid():N}@example.com";
        var password = "Password1!";

        var registration = await client.PostAsJsonAsync("/api/auth/register", new
        {
            Username = "member" + Guid.NewGuid().ToString("N")[..8],
            Email = email,
            Password = password
        });
        registration.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Auth.InvalidCredentials");
    }
}
