using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DiyMusicCommunity.Infrastructure.Repositories;

public sealed class ReleaseRepository : IReleaseRepository
{
    private readonly AppDbContext _context;

    public ReleaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Release?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Releases
            .Include(r => r.Band)
            .Include(r => r.Formats)
            .Include(r => r.Tracks)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
