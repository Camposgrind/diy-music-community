using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;
using DiyMusicCommunity.Domain.Enums;
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

    public async Task<Band?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bands
            .Include(b => b.Genre)
            .Include(b => b.Releases)
                .ThenInclude(release => release.Tracks)
            .Include(b => b.Members)
                .ThenInclude(m => m.OtherBands)
                    .ThenInclude(ob => ob.OtherBand)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Band?> FindByNameAndCountryAsync(string name, string country, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToUpper();
        var normalizedCountry = country.Trim().ToUpper();

        return await _context.Bands.FirstOrDefaultAsync(
            band => band.Name.Trim().ToUpper() == normalizedName && band.Country.Trim().ToUpper() == normalizedCountry,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Band>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bands.ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Band> Items, int TotalCount)> SearchAsync(
        BandSearchFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Bands
            .Include(b => b.Genre)
            .Where(b => b.TrustStatus != TrustStatus.Blocked)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(b => b.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter.Country))
        {
            query = query.Where(b => b.Country.ToLower() == filter.Country.ToLower());
        }

        if (filter.GenreId.HasValue)
        {
            query = query.Where(b => b.GenreId == filter.GenreId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(b => b.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Band band, CancellationToken cancellationToken = default)
    {
        await _context.Bands.AddAsync(band, cancellationToken);
    }
}
