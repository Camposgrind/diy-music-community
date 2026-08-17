using DiyMusicCommunity.Domain.Entities;

namespace DiyMusicCommunity.Domain.Abstractions;

public interface IBandRepository
{
    Task<Band?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Band?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Band?> FindByNameAndCountryAsync(string name, string country, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Band>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Band> Items, int TotalCount)> SearchAsync(BandSearchFilter filter, CancellationToken cancellationToken = default);
    Task AddAsync(Band band, CancellationToken cancellationToken = default);
}
