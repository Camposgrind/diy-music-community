using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Abstractions;

public interface IClaimRepository
{
    Task<BandClaim?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BandClaim>> GetByStatusAsync(ClaimStatus status, CancellationToken cancellationToken = default);
    Task<bool> HasPendingClaimAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default);
    Task AddAsync(BandClaim claim, CancellationToken cancellationToken = default);
    void Update(BandClaim claim);
}
