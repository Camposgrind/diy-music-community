using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Application.Bands.GetBands;
using DiyMusicCommunity.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace DiyMusicCommunity.Api.Controllers;

/// <summary>
/// Public endpoints for browsing and filtering the DIY band catalog.
/// </summary>
[ApiController]
[Route("api/bands")]
[Produces("application/json")]
[Tags("Bands")]
public sealed class BandsController : ControllerBase
{
    private readonly GetBandsUseCase _getBandsUseCase;
    private readonly GetBandDetailUseCase _getBandDetailUseCase;

    /// <summary>Initialises a new instance of <see cref="BandsController"/>.</summary>
    public BandsController(GetBandsUseCase getBandsUseCase, GetBandDetailUseCase getBandDetailUseCase)
    {
        _getBandsUseCase = getBandsUseCase;
        _getBandDetailUseCase = getBandDetailUseCase;
    }

    /// <summary>Search and filter the public band catalog.</summary>
    /// <param name="query">Filters and pagination: name, country, genreId, status, page, pageSize.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Paged list of bands matching the filters.</response>
    /// <response code="400">Validation error — invalid pagination values or filter length exceeded.</response>
    /// <response code="422">Filters matched more than 100 bands — refine the search.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BandListItemModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetBands([FromQuery] GetBandsQuery query, CancellationToken cancellationToken)
    {
        var result = await _getBandsUseCase.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            switch (result.Error!.Code)
            {
                case BandErrors.Codes.TooManyResults:
                    return UnprocessableEntity(result.Error);

                default:
                    return BadRequest(result.Error);
            }
        }

        return Ok(result.Value);
    }

    /// <summary>Get the full profile of a single band, including its releases and members.</summary>
    /// <param name="id">The unique identifier of the band.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Full band profile with releases and members.</response>
    /// <response code="404">No band found with the given id.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BandDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBandDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getBandDetailUseCase.Handle(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}
