using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using DiyMusicCommunity.Domain.Abstractions;
using DiyMusicCommunity.Domain.Entities;

namespace DiyMusicCommunity.Application.Bands.CatalogManagement;

public sealed class CatalogManagementUseCase
{
    private readonly IBandRepository _bandRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReleaseRepository _releaseRepository;

    public CatalogManagementUseCase(IBandRepository bandRepository, IGenreRepository genreRepository, IUnitOfWork unitOfWork, IReleaseRepository releaseRepository)
    {
        _bandRepository = bandRepository;
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
        _releaseRepository = releaseRepository;
    }

    public async Task<Result<BandDetailModel>> CreateBand(BandWriteRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBand(request, cancellationToken);

        if (validationError is not null) 
        { 
            return Result<BandDetailModel>.Failure(validationError); 
        }

        if (await _bandRepository.FindByNameAndCountryAsync(request.Name, request.Country, cancellationToken) is not null) 
        { 
            return Result<BandDetailModel>.Failure(BandErrors.Duplicate("band")); 
        }

        var band = new Band(Guid.NewGuid(), request.Name.Trim(), request.Country.Trim(), request.GenreId, request.Status, DateTime.UtcNow);

        ApplyBandFields(band, request);

        await _bandRepository.AddAsync(band, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await _bandRepository.GetDetailAsync(band.Id, cancellationToken);

        return Result<BandDetailModel>.Success(CatalogDetailMapper.ToBandDetail(detail!));
    }

    public async Task<Result<BandDetailModel>> UpdateBand(Guid bandId, BandWriteRequest request, CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateBand(request, cancellationToken);
        if (validationError is not null) 
        { 
            return Result<BandDetailModel>.Failure(validationError); 
        }

        var band = await _bandRepository.GetByIdAsync(bandId, cancellationToken);
        if (band is null) 
        { 
            return Result<BandDetailModel>.Failure(BandErrors.NotFound(bandId));
        }

        band.Update(request.Name.Trim(), request.Country.Trim(), request.GenreId, request.Status);

        ApplyBandFields(band, request);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await _bandRepository.GetDetailAsync(band.Id, cancellationToken);

        return Result<BandDetailModel>.Success(CatalogDetailMapper.ToBandDetail(detail!));
    }

    public async Task<Result<BandMemberModel>> CreateMember(Guid bandId, MemberWriteRequest request, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetDetailAsync(bandId, cancellationToken);
        if (band is null) 
        { 
            return Result<BandMemberModel>.Failure(BandErrors.NotFound(bandId)); 
        }

        if (string.IsNullOrWhiteSpace(request.Name) || (request.EndYear.HasValue && request.StartYear.HasValue && request.EndYear < request.StartYear)) 
        { 
            return Result<BandMemberModel>.Failure(BandErrors.InvalidRequest("Member name is required and end year cannot precede start year."));
        }

        if (band.Members.Any(member => Same(member.Name, request.Name) && member.StartYear == request.StartYear)) 
        { 
            return Result<BandMemberModel>.Failure(BandErrors.Duplicate("member")); 
        }

        var member = new BandMember(Guid.NewGuid(), bandId, request.Name.Trim(), request.IsCurrent);

        member.Update(request.Name.Trim(), request.Instrument, request.StartYear, request.EndYear, request.IsCurrent);

        band.AddMember(member);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<BandMemberModel>.Success(CatalogDetailMapper.ToMember(member));
    }

    public async Task<Result<BandMemberModel>> UpdateMember(Guid bandId, Guid memberId, MemberWriteRequest request, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetDetailAsync(bandId, cancellationToken);
        if (band is null) 
        { 
            return Result<BandMemberModel>.Failure(BandErrors.NotFound(bandId)); 
        }

        var member = band.Members.SingleOrDefault(item => item.Id == memberId);
        if (member is null) 
        { 
            return Result<BandMemberModel>.Failure(Error.NotFound("Member.NotFound", $"No member with id '{memberId}' was found for this band.")); 
        }

        if (string.IsNullOrWhiteSpace(request.Name) || (request.EndYear.HasValue && request.StartYear.HasValue && request.EndYear < request.StartYear)) 
        { 
            return Result<BandMemberModel>.Failure(BandErrors.InvalidRequest("Member name is required and end year cannot precede start year.")); 
        }

        if (band.Members.Any(item => item.Id != memberId && Same(item.Name, request.Name) && item.StartYear == request.StartYear)) 
        { 
            return Result<BandMemberModel>.Failure(BandErrors.Duplicate("member")); 
        }
        
        member.Update(request.Name.Trim(), request.Instrument, request.StartYear, request.EndYear, request.IsCurrent);
       
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<BandMemberModel>.Success(CatalogDetailMapper.ToMember(member));
    }

    public async Task<Result<ReleaseDetailModel>> CreateRelease(Guid bandId, ReleaseWriteRequest request, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetDetailAsync(bandId, cancellationToken);
        if (band is null) 
        { 
            return Result<ReleaseDetailModel>.Failure(BandErrors.NotFound(bandId)); 
        }

        var error = ValidateRelease(request);
        if (error is not null) 
        { 
            return Result<ReleaseDetailModel>.Failure(error); 
        }
        
        if (band.Releases.Any(release => SameReleaseIdentity(release, request))) 
        { 
            return Result<ReleaseDetailModel>.Failure(BandErrors.Duplicate("release")); 
        }

        var release = new Release(Guid.NewGuid(), bandId, request.Title.Trim(), request.ReleaseType);

        ApplyRelease(release, request);

        band.AddRelease(release);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await _releaseRepository.GetDetailAsync(release.Id, cancellationToken);

        return Result<ReleaseDetailModel>.Success(CatalogDetailMapper.ToReleaseDetail(detail!));
    }

    public async Task<Result<ReleaseDetailModel>> UpdateRelease(Guid bandId, Guid releaseId, ReleaseWriteRequest request, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetDetailAsync(bandId, cancellationToken);
        if (band is null) 
        { 
            return Result<ReleaseDetailModel>.Failure(BandErrors.NotFound(bandId)); 
        }
        
        var release = band.Releases.SingleOrDefault(item => item.Id == releaseId);
        if (release is null) 
        { 
            return Result<ReleaseDetailModel>.Failure(Error.NotFound("Release.NotFound", $"No release with id '{releaseId}' was found for this band.")); 
        }
        
        var error = ValidateRelease(request);
        if (error is not null) 
        { 
            return Result<ReleaseDetailModel>.Failure(error); 
        }

        if (band.Releases.Any(item => item.Id != releaseId && SameReleaseIdentity(item, request))) 
        { 
            return Result<ReleaseDetailModel>.Failure(BandErrors.Duplicate("release")); 
        }

        ApplyRelease(release, request);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await _releaseRepository.GetDetailAsync(release.Id, cancellationToken);
        return Result<ReleaseDetailModel>.Success(CatalogDetailMapper.ToReleaseDetail(detail!));
    }

    private async Task<Error?> ValidateBand(BandWriteRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200 || string.IsNullOrWhiteSpace(request.Country) || request.Country.Length > 100 || request.GenreId == Guid.Empty) 
        { 
            return BandErrors.InvalidRequest("Name, country, and genreId are required; name and country must be within their maximum lengths."); 
        }

        var genres = await _genreRepository.GetAllAsync(cancellationToken);

        return genres.Any(genre => genre.Id == request.GenreId) ? null : BandErrors.InvalidRequest("GenreId must reference an existing genre.");
    }

    private static Error? ValidateRelease(ReleaseWriteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 300 || request.Tracks.Any(track => string.IsNullOrWhiteSpace(track.Title) || track.Title.Length > 300))
        {
            return BandErrors.InvalidRequest("Release title and track titles are required, and text lengths must not exceed 300 characters.");
        }

        return null;
    }

    private static void ApplyBandFields(Band band, BandWriteRequest request)
    {
        band.SetLocation(request.Location); 
        band.SetFormationYear(request.FormationYear); 
        band.SetDescription(request.Description); 
        band.SetImages(request.LogoImageUrl, request.BandImageUrl); 
        band.SetMusicUrlPortal(request.MusicUrlPortal); 
        band.SetBandContact(request.BandContact);
    }

    private static void ApplyRelease(Release release, ReleaseWriteRequest request)
    {
        release.Update(request.Title.Trim(), request.ReleaseType, request.ReleaseDate, request.Year, request.LabelText, request.CoverImageUrl);
        release.ReplaceTracks(request.Tracks.Select((track, index) => (track.Title.Trim(), index + 1)).ToList());
    }

    private static bool Same(string left, string right) 
    { 
        return string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase); 
    }

    private static bool SameReleaseIdentity(Release release, ReleaseWriteRequest request) 
    { 
        return Same(release.Title, request.Title) && (request.ReleaseDate.HasValue 
            ? release.ReleaseDate == request.ReleaseDate 
            : !release.ReleaseDate.HasValue && release.Year == request.Year);
    }
}
