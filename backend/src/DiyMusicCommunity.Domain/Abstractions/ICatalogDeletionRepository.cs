namespace DiyMusicCommunity.Domain.Abstractions;

public interface ICatalogDeletionRepository
{
    Task<bool> DeleteBandAsync(Guid bandId, CancellationToken cancellationToken = default);
    Task<bool> DeleteMemberAsync(Guid bandId, Guid memberId, CancellationToken cancellationToken = default);
    Task<bool> DeleteReleaseAsync(Guid releaseId, CancellationToken cancellationToken = default);
    Task<bool> DeleteTrackAsync(Guid releaseId, Guid trackId, CancellationToken cancellationToken = default);
}
