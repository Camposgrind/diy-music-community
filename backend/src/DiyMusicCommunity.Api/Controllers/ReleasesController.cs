using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Application.Releases;
using DiyMusicCommunity.Application.Releases.GetReleaseDetail;
using DiyMusicCommunity.Application.Bands.CatalogDeletion;
using Microsoft.AspNetCore.Authorization;
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
    private readonly CatalogDeletionUseCase _catalogDeletionUseCase;

    /// <summary>Initialises a new instance of <see cref="ReleasesController"/>.</summary>
    public ReleasesController(GetReleaseDetailUseCase getReleaseDetailUseCase, CatalogDeletionUseCase catalogDeletionUseCase)
    {
        _getReleaseDetailUseCase = getReleaseDetailUseCase;
        _catalogDeletionUseCase = catalogDeletionUseCase;
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

    [HttpDelete("{releaseId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRelease(Guid releaseId, CancellationToken cancellationToken)
    {
        var result = await _catalogDeletionUseCase.DeleteRelease(releaseId, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }

    [HttpDelete("{releaseId:guid}/tracks/{trackId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrack(Guid releaseId, Guid trackId, CancellationToken cancellationToken)
    {
        var result = await _catalogDeletionUseCase.DeleteTrack(releaseId, trackId, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
