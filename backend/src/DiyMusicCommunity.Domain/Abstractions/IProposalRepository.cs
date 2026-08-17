using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Domain.Abstractions;

public interface IProposalRepository
{
    Task<BandProposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BandProposal>> GetByStatusAsync(ProposalStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BandProposal>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(BandProposal proposal, CancellationToken cancellationToken = default);
}
