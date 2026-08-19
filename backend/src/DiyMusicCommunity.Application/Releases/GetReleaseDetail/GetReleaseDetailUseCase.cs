using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Application.Abstractions;

namespace DiyMusicCommunity.Application.Releases.GetReleaseDetail;

public sealed class GetReleaseDetailUseCase
{
    private readonly IReleaseRepository _releaseRepository;
    private readonly IImageUrlResolver _imageUrlResolver;

    public GetReleaseDetailUseCase(IReleaseRepository releaseRepository, IImageUrlResolver imageUrlResolver)
    {
        _releaseRepository = releaseRepository;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Result<ReleaseDetailModel>> Handle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var release = await _releaseRepository.GetDetailAsync(id, cancellationToken);

        if (release is null)
        {
            return Result<ReleaseDetailModel>.Failure(ReleaseErrors.NotFound(id));
        }

        var model = new ReleaseDetailModel
        {
            Id = release.Id,
            Title = release.Title,
            ReleaseType = release.ReleaseType,
            ReleaseDate = release.ReleaseDate,
            Year = release.Year,
            LabelText = release.LabelText,
            CoverImageUrl = await _imageUrlResolver.ResolveAsync(release.ReleaseCoverBlobPath, cancellationToken),
            Band = release.Band is null
                ? null
                : new ReleaseBandModel
                {
                    BandId = release.Band.Id,
                    Name = release.Band.Name
                },
            Formats = release.Formats
                .Select(f => f.Format)
                .ToList()
                .AsReadOnly(),
            Tracks = release.Tracks
                .OrderBy(t => t.TrackNumber)
                .Select(t => new ReleaseTrackModel
                {
                    ReleaseId = t.ReleaseId,
                    Title = t.Title,
                    TrackNumber = t.TrackNumber
                })
                .ToList()
                .AsReadOnly()
        };

        return Result<ReleaseDetailModel>.Success(model);
    }
}
