using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DiyMusicCommunity.Infrastructure.Repositories;

public sealed class ProposalRepository : IProposalRepository
{
    private readonly AppDbContext _context;

    public ProposalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BandProposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BandProposals.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BandProposal>> GetByStatusAsync(ProposalStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.BandProposals.Where(p => p.ReviewStatus == status).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BandProposal>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.BandProposals.Where(p => p.SubmittedByUserId == userId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BandProposal proposal, CancellationToken cancellationToken = default)
    {
        await _context.BandProposals.AddAsync(proposal, cancellationToken);
    }

    public void Update(BandProposal proposal)
    {
        _context.BandProposals.Update(proposal);
    }
}
