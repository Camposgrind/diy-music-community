using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DiyMusicCommunity.Infrastructure.Repositories;

public sealed class ClaimRepository : IClaimRepository
{
    private readonly AppDbContext _context;

    public ClaimRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BandClaim?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BandClaims.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BandClaim>> GetByStatusAsync(ClaimStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.BandClaims.Where(c => c.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPendingClaimAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default)
    {
        return await _context.BandClaims.AnyAsync(c => c.UserId == userId && c.BandId == bandId && c.Status == ClaimStatus.Pending, cancellationToken);
    }

    public async Task AddAsync(BandClaim claim, CancellationToken cancellationToken = default)
    {
        await _context.BandClaims.AddAsync(claim, cancellationToken);
    }

    public void Update(BandClaim claim)
    {
        _context.BandClaims.Update(claim);
    }
}
