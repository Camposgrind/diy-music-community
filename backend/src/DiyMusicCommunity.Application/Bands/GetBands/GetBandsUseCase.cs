using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Abstractions;

namespace DiyMusicCommunity.Application.Bands.GetBands;

public sealed class GetBandsUseCase
{
    private const int MaxResultsBeforePagination = 100;

    private readonly IBandRepository _bandRepository;
    private readonly GetBandsQueryValidator _validator;

    public GetBandsUseCase(IBandRepository bandRepository, GetBandsQueryValidator validator)
    {
        _bandRepository = bandRepository;
        _validator = validator;
    }

    public async Task<Result<PagedResult<BandListItemModel>>> Handle(
        GetBandsQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<PagedResult<BandListItemModel>>.Failure(BandErrors.InvalidFilter(message));
        }

        var filter = new BandSearchFilter
        {
            Name = query.Name,
            Country = query.Country,
            GenreId = query.GenreId,
            Status = query.Status,
            Page = query.Page,
            PageSize = query.PageSize
        };

        var (items, totalCount) = await _bandRepository.SearchAsync(filter, cancellationToken);

        if (totalCount > MaxResultsBeforePagination)
        {
            return Result<PagedResult<BandListItemModel>>.Failure(
                BandErrors.TooManyResults(MaxResultsBeforePagination));
        }

        var models = items.Select(b => new BandListItemModel
        {
            Id = b.Id,
            Name = b.Name,
            Country = b.Country,
            Genre = b.Genre?.Name ?? string.Empty,
            Status = b.Status.ToString(),
            FormationYear = b.FormationYear
        }).ToList();

        return Result<PagedResult<BandListItemModel>>.Success(new PagedResult<BandListItemModel>(models, query.Page, query.PageSize, totalCount));
    }
}
