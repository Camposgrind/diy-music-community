using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Abstractions;

namespace DiyMusicCommunity.Application.Genres.GetGenres;

public sealed class GetGenresUseCase
{
    private readonly IGenreRepository _genreRepository;

    public GetGenresUseCase(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<Result<IReadOnlyList<GenreModel>>> Handle(
        CancellationToken cancellationToken = default)
    {
        var genres = await _genreRepository.GetAllAsync(cancellationToken);

        var models = genres
            .Select(g => new GenreModel { Id = g.Id, Name = g.Name })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<GenreModel>>.Success(models);
    }
}
