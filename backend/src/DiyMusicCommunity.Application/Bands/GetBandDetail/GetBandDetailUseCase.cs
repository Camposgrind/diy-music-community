using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Domain.Abstractions;

namespace DiyMusicCommunity.Application.Bands.GetBandDetail;

public sealed class GetBandDetailUseCase
{
    private readonly IBandRepository _bandRepository;
    private readonly IImageUrlResolver _imageUrlResolver;

    public GetBandDetailUseCase(IBandRepository bandRepository, IImageUrlResolver imageUrlResolver)
    {
        _bandRepository = bandRepository;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Result<BandDetailModel>> Handle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetDetailAsync(id, cancellationToken);

        if (band is null)
        {
            return Result<BandDetailModel>.Failure(BandErrors.NotFound(id));
        }

        var model = new BandDetailModel
        {
            Id = band.Id,
            Name = band.Name,
            Country = band.Country,
            Location = band.Location,
            Status = band.Status,
            Genre = band.Genre?.Name,
            FormationYear = band.FormationYear,
            SplitUpYear = band.SplitUpYear,
            Description = band.Description,
            LogoImageUrl = await _imageUrlResolver.ResolveAsync(band.LogoImageBlobPath, cancellationToken),
            BandImageUrl = await _imageUrlResolver.ResolveAsync(band.BandPhotoBlobPath, cancellationToken),
            MusicUrlPortal = band.MusicUrlPortal,
            BandContact = band.BandContact,
            Releases = band.Releases
                .OrderBy(release => release.Year.HasValue ? 0 : 1)
                .ThenBy(release => release.Year)
                .ThenBy(release => release.ReleaseDate.HasValue ? 0 : 1)
                .ThenBy(release => release.ReleaseDate)
                .ThenBy(release => release.Title, StringComparer.OrdinalIgnoreCase)
                .Select(r => new BandReleaseModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    ReleaseType = r.ReleaseType,
                    Year = r.Year
                })
                .ToList()
                .AsReadOnly(),
            Members = band.Members
                .Select(m => new BandMemberModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Instrument = m.Instrument,
                    StartYear = m.StartYear,
                    EndYear = m.EndYear,
                    IsCurrent = m.IsCurrent,
                    IsLastKnownLineup = m.IsLastKnownLineup,
                    OtherBands = m.OtherBands
                        .Select(ob => new BandMemberOtherBandModel
                        {
                            BandId = ob.OtherBandId,
                            BandName = ob.OtherBand?.Name
                        })
                        .ToList()
                        .AsReadOnly()
                })
                .ToList()
                .AsReadOnly()
        };

        return Result<BandDetailModel>.Success(model);
    }
}
