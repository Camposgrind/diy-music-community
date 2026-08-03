using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DiyMusicCommunity.Infrastructure.Repositories;

public sealed class BandRepository : IBandRepository
{
    private readonly AppDbContext _context;

    public BandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Band?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bands.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Band>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bands.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Band band, CancellationToken cancellationToken = default)
    {
        await _context.Bands.AddAsync(band, cancellationToken);
    }

    public void Update(Band band)
    {
        _context.Bands.Update(band);
    }
}
