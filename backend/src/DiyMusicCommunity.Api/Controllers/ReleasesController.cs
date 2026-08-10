using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Application.Releases;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using Microsoft.AspNetCore.Mvc;

namespace DiyMusicCommunity.Api.Controllers;

/// <summary>
/// Public endpoints for browsing release details.
/// </summary>
[ApiController]
[Route("api/releases")]
[Produces("application/json")]
[Tags("Releases")]
public sealed class ReleasesController : ControllerBase
{
    private readonly GetReleaseDetailUseCase _getReleaseDetailUseCase;

    /// <summary>Initialises a new instance of <see cref="ReleasesController"/>.</summary>
    public ReleasesController(GetReleaseDetailUseCase getReleaseDetailUseCase)
    {
        _getReleaseDetailUseCase = getReleaseDetailUseCase;
    }

    /// <summary>Get the full detail of a single release, including its tracks and formats.</summary>
    /// <param name="id">The unique identifier of the release.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Full release detail with tracks and formats.</response>
    /// <response code="404">No release found with the given id.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReleaseDetailModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReleaseDetail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getReleaseDetailUseCase.Handle(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}
