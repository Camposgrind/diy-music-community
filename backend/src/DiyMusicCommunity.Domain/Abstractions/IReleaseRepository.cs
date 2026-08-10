using DiyMusicCommunity.Domain.Entities;

namespace DiyMusicCommunity.Domain.Abstractions;

public interface IReleaseRepository
{
    Task<Release?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);
}
