using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Abstractions;

namespace DiyMusicCommunity.Application.Bands.CatalogDeletion;

public sealed class CatalogDeletionUseCase
{
    private readonly ICatalogDeletionRepository _repository;

    public CatalogDeletionUseCase(ICatalogDeletionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> DeleteBand(Guid bandId, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteBandAsync(bandId, cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure(BandErrors.NotFound(bandId));
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteMember(Guid bandId, Guid memberId, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteMemberAsync(bandId, memberId, cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure(Error.NotFound("Member.NotFound", $"No member with id '{memberId}' was found for this band."));
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteRelease(Guid releaseId, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteReleaseAsync(releaseId, cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure(Error.NotFound("Release.NotFound", $"No release with id '{releaseId}' was found."));
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteTrack(Guid releaseId, Guid trackId, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteTrackAsync(releaseId, trackId, cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure(Error.NotFound("Track.NotFound", $"No track with id '{trackId}' was found for this release."));
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAllTracks(Guid releaseId, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAllTracksAsync(releaseId, cancellationToken);
        if (!deleted)
        {
            return Result<bool>.Failure(Error.NotFound("Release.NotFound", $"No release with id '{releaseId}' was found."));
        }

        return Result<bool>.Success(true);
    }
}
