using System.Net;
using System.Net.Http.Json;
using DiyMusicCommunity.Application.Genres.GetGenres;
using FluentAssertions;

namespace DiyMusicCommunity.Api.IntegrationTests;

public sealed class GenresControllerTests
{
    private static (CustomWebApplicationFactory Factory, HttpClient Client) CreateClient()
    {
        var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClientWithDb();
        return (factory, client);
    }

    [Fact]
    public async Task GET_Genres_Should_Return200WithAllGenres()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var response = await client.GetAsync("/api/genres");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<GenreModel>>();
        body.Should().NotBeNull();
        body!.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GET_Genres_Should_ReturnGenresSortedAlphabetically()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var response = await client.GetAsync("/api/genres");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<GenreModel>>();
        body.Should().NotBeNull();
        var names = body!.Select(g => g.Name).ToList();
        names.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GET_Genres_Should_ReturnGenresWithIdAndName()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var response = await client.GetAsync("/api/genres");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<GenreModel>>();
        body.Should().AllSatisfy(g =>
        {
            g.Id.Should().NotBeEmpty();
            g.Name.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GET_Genres_Should_IncludeSeededGenres()
    {
        var (factory, client) = CreateClient();
        using var _ = factory;

        var response = await client.GetAsync("/api/genres");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<GenreModel>>();
        var names = body!.Select(g => g.Name).ToList();
        names.Should().Contain("Grindcore");
        names.Should().Contain("Crust");
        names.Should().Contain("Death Metal");
    }
}
