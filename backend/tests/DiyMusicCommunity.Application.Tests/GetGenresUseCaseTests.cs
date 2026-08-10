using DiyMusicCommunity.Application.Genres.GetGenres;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using Moq;

namespace DiyMusicCommunity.Application.Tests;

public class GetGenresUseCaseTests
{
    private static (GetGenresUseCase UseCase, Mock<IGenreRepository> Repo) BuildSut()
    {
        var repo = new Mock<IGenreRepository>();
        var useCase = new GetGenresUseCase(repo.Object);
        return (useCase, repo);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithAllGenres()
    {
        var (sut, repo) = BuildSut();
        var genres = new List<Genre>
        {
            new(Guid.NewGuid(), "Grindcore"),
            new(Guid.NewGuid(), "Crust")
        };
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(genres);

        var result = await sut.Handle();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task Handle_Should_MapIdAndNameCorrectly()
    {
        var (sut, repo) = BuildSut();
        var id = Guid.NewGuid();
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Genre(id, "Powerviolence")]);

        var result = await sut.Handle();

        var model = result.Value!.Single();
        Assert.Equal(id, model.Id);
        Assert.Equal("Powerviolence", model.Name);
    }

    [Fact]
    public async Task Handle_WhenNoGenres_Should_ReturnSuccessWithEmptyList()
    {
        var (sut, repo) = BuildSut();
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await sut.Handle();

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
