using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DiyMusicCommunity.Infrastructure.Repositories;

public sealed class CatalogDeletionRepository : ICatalogDeletionRepository
{
    private readonly AppDbContext _context;

    public CatalogDeletionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteBandAsync(Guid bandId, CancellationToken cancellationToken = default)
    {
        var band = await _context.Bands.SingleOrDefaultAsync(item => item.Id == bandId, cancellationToken);
        if (band is null)
        {
            return false;
        }

        var memberIds = await _context.BandMembers
            .Where(item => item.BandId == bandId)
            .Select(item => item.Id)
            .ToListAsync(cancellationToken);
        var otherBandLinks = await _context.BandMemberOtherBands
            .Where(item => item.OtherBandId == bandId || memberIds.Contains(item.BandMemberId))
            .ToListAsync(cancellationToken);
        var releaseIds = await _context.Releases
            .Where(item => item.BandId == bandId)
            .Select(item => item.Id)
            .ToListAsync(cancellationToken);
        var tracks = await _context.Tracks
            .Where(item => releaseIds.Contains(item.ReleaseId))
            .ToListAsync(cancellationToken);
        var formats = await _context.ReleaseFormats
            .Where(item => releaseIds.Contains(item.ReleaseId))
            .ToListAsync(cancellationToken);
        var releases = await _context.Releases
            .Where(item => item.BandId == bandId)
            .ToListAsync(cancellationToken);
        var members = await _context.BandMembers
            .Where(item => item.BandId == bandId)
            .ToListAsync(cancellationToken);

        _context.BandMemberOtherBands.RemoveRange(otherBandLinks);
        _context.Tracks.RemoveRange(tracks);
        _context.ReleaseFormats.RemoveRange(formats);
        _context.Releases.RemoveRange(releases);
        _context.BandMembers.RemoveRange(members);
        _context.Bands.Remove(band);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteMemberAsync(Guid bandId, Guid memberId, CancellationToken cancellationToken = default)
    {
        var member = await _context.BandMembers.SingleOrDefaultAsync(
            item => item.Id == memberId && item.BandId == bandId,
            cancellationToken);
        if (member is null)
        {
            return false;
        }

        var otherBandLinks = await _context.BandMemberOtherBands
            .Where(item => item.BandMemberId == memberId)
            .ToListAsync(cancellationToken);

        _context.BandMemberOtherBands.RemoveRange(otherBandLinks);
        _context.BandMembers.Remove(member);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteReleaseAsync(Guid releaseId, CancellationToken cancellationToken = default)
    {
        var release = await _context.Releases.SingleOrDefaultAsync(item => item.Id == releaseId, cancellationToken);
        if (release is null)
        {
            return false;
        }

        var tracks = await _context.Tracks
            .Where(item => item.ReleaseId == releaseId)
            .ToListAsync(cancellationToken);
        var formats = await _context.ReleaseFormats
            .Where(item => item.ReleaseId == releaseId)
            .ToListAsync(cancellationToken);

        _context.Tracks.RemoveRange(tracks);
        _context.ReleaseFormats.RemoveRange(formats);
        _context.Releases.Remove(release);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteTrackAsync(Guid releaseId, Guid trackId, CancellationToken cancellationToken = default)
    {
        var track = await _context.Tracks.SingleOrDefaultAsync(
            item => item.Id == trackId && item.ReleaseId == releaseId,
            cancellationToken);
        if (track is null)
        {
            return false;
        }

        if (_context.Database.IsRelational())
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync(cancellationToken);
            await RenumberTracksAsync(releaseId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        else
        {
            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync(cancellationToken);
            await RenumberTracksAsync(releaseId, cancellationToken);
        }

        return true;
    }

    public async Task<bool> DeleteAllTracksAsync(Guid releaseId, CancellationToken cancellationToken = default)
    {
        var releaseExists = await _context.Releases.AnyAsync(item => item.Id == releaseId, cancellationToken);
        if (!releaseExists)
        {
            return false;
        }

        var tracks = await _context.Tracks
            .Where(item => item.ReleaseId == releaseId)
            .ToListAsync(cancellationToken);

        _context.Tracks.RemoveRange(tracks);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task RenumberTracksAsync(Guid releaseId, CancellationToken cancellationToken)
    {
        if (_context.Database.IsRelational())
        {
            await _context.Tracks
                .Where(item => item.ReleaseId == releaseId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.TrackNumber, item => -item.TrackNumber), cancellationToken);
        }

        var tracksQuery = _context.Tracks.Where(item => item.ReleaseId == releaseId);
        List<DiyMusicCommunity.Domain.Entities.Track> tracks;

        if (_context.Database.IsRelational())
        {
            tracks = await tracksQuery.OrderBy(item => -item.TrackNumber).ToListAsync(cancellationToken);
        }
        else
        {
            tracks = await tracksQuery.OrderBy(item => item.TrackNumber).ToListAsync(cancellationToken);
        }

        for (var index = 0; index < tracks.Count; index++)
        {
            _context.Entry(tracks[index]).Property(item => item.TrackNumber).CurrentValue = index + 1;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
