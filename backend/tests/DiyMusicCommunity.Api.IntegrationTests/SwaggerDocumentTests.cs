using System.Net;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class SwaggerDocumentTests
{
    [Fact]
    public async Task SwaggerDocument_WithBandImageUploadEndpoint_ShouldReturnOk()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
