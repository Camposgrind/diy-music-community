using DiyMusicCommunity.Domain.Entities;

namespace DiyMusicCommunity.Domain.Abstractions;

public interface IGenreRepository
{
    Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken cancellationToken = default);
}
