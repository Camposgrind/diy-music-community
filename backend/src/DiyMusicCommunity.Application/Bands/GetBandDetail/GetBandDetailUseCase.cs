using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Domain.Abstractions;

namespace DiyMusicCommunity.Application.Bands.GetBandDetail;

public sealed class GetBandDetailUseCase
{
    private readonly IBandRepository _bandRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetBandDetailUseCase(IBandRepository bandRepository, IBlobStorageService blobStorageService)
    {
        _bandRepository = bandRepository;
        _blobStorageService = blobStorageService;
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
            LogoImageUrl = await GetReadUrlAsync(band.LogoImageBlobPath, cancellationToken),
            BandImageUrl = await GetReadUrlAsync(band.BandPhotoBlobPath, cancellationToken),
            MusicUrlPortal = band.MusicUrlPortal,
            BandContact = band.BandContact,
            Releases = band.Releases
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

    private async Task<string?> GetReadUrlAsync(string? blobPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
        {
            return null;
        }

        var uri = await _blobStorageService.GenerateReadUriAsync(blobPath, cancellationToken);
        return uri.ToString();
    }
}
