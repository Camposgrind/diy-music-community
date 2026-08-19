using DiyMusicCommunity.Application.Bands;
using DiyMusicCommunity.Application.Bands.GetBandDetail;
using DiyMusicCommunity.Application.Bands.GetBands;
using DiyMusicCommunity.Application.Bands.CatalogManagement;
using DiyMusicCommunity.Application.Bands.CatalogDeletion;
using DiyMusicCommunity.Application.Bands.Images;
using DiyMusicCommunity.Application.Common;
using DiyMusicCommunity.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
    private readonly CatalogManagementUseCase _catalogManagementUseCase;
    private readonly CatalogDeletionUseCase _catalogDeletionUseCase;
    private readonly BandImagesUseCase _bandImagesUseCase;

    /// <summary>Initialises a new instance of <see cref="BandsController"/>.</summary>
    public BandsController(GetBandsUseCase getBandsUseCase, GetBandDetailUseCase getBandDetailUseCase, CatalogManagementUseCase catalogManagementUseCase, CatalogDeletionUseCase catalogDeletionUseCase, BandImagesUseCase bandImagesUseCase)
    {
        _getBandsUseCase = getBandsUseCase;
        _getBandDetailUseCase = getBandDetailUseCase;
        _catalogManagementUseCase = catalogManagementUseCase;
        _catalogDeletionUseCase = catalogDeletionUseCase;
        _bandImagesUseCase = bandImagesUseCase;
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBand([FromBody] BandWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.CreateBand(request, cancellationToken);
        return WriteResult(result, model => CreatedAtAction(nameof(GetBandDetail), new { id = model.Id }, model));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBand(Guid id, [FromBody] BandWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.UpdateBand(id, request, cancellationToken);
        return WriteResult(result, Ok);
    }

    [HttpPost("{bandId:guid}/members")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMember(Guid bandId, [FromBody] MemberWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.CreateMember(bandId, request, cancellationToken);
        return WriteResult(result, model => Created($"/api/bands/{bandId}/members/{model.Id}", model));
    }

    [HttpPut("{bandId:guid}/members/{memberId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMember(Guid bandId, Guid memberId, [FromBody] MemberWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.UpdateMember(bandId, memberId, request, cancellationToken);
        return WriteResult(result, Ok);
    }

    [HttpPost("{bandId:guid}/releases")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRelease(Guid bandId, [FromBody] ReleaseWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.CreateRelease(bandId, request, cancellationToken);
        return WriteResult(result, model => Created($"/api/releases/{model.Id}", model));
    }

    [HttpPut("{bandId:guid}/releases/{releaseId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRelease(Guid bandId, Guid releaseId, [FromBody] ReleaseWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.UpdateRelease(bandId, releaseId, request, cancellationToken);
        return WriteResult(result, Ok);
    }

    [HttpPut("{bandId:guid}/releases/{releaseId:guid}/tracks")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateReleaseTracks(Guid bandId, Guid releaseId, [FromBody] TrackListWriteRequest request, CancellationToken cancellationToken)
    {
        var result = await _catalogManagementUseCase.UpdateReleaseTracks(bandId, releaseId, request, cancellationToken);
        return WriteResult(result, Ok);
    }

    [HttpPost("{bandId:guid}/images/temporary")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadTemporaryImage(Guid bandId, [FromForm] UploadTemporaryBandImageInput request, CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            return BadRequest(BandErrors.InvalidRequest("An image file is required."));
        }

        await using var sourceStream = request.File.OpenReadStream();
        await using var contentStream = new MemoryStream();
        await sourceStream.CopyToAsync(contentStream, cancellationToken);
        var result = await _bandImagesUseCase.UploadTemporaryAsync(bandId, new UploadTemporaryBandImageRequest
        {
            ImageType = request.ImageType,
            OriginalFileName = request.File.FileName,
            DeclaredContentType = request.File.ContentType,
            Content = contentStream.ToArray()
        }, cancellationToken);
        return WriteResult(result, Ok);
    }

    [HttpPost("{bandId:guid}/images/confirm")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ConfirmImage(Guid bandId, [FromBody] ConfirmBandImageInput request, CancellationToken cancellationToken)
    {
        var result = await _bandImagesUseCase.ConfirmAsync(bandId, new ConfirmBandImageRequest(request.ImageType, request.TemporaryFileId), cancellationToken);
        return WriteResult(result, Ok);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBand(Guid id, CancellationToken cancellationToken)
    {
        var result = await _catalogDeletionUseCase.DeleteBand(id, cancellationToken);
        return DeleteResult(result);
    }

    [HttpDelete("{bandId:guid}/members/{memberId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMember(Guid bandId, Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _catalogDeletionUseCase.DeleteMember(bandId, memberId, cancellationToken);
        return DeleteResult(result);
    }

    private IActionResult WriteResult<T>(Result<T> result, Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return result.Error!.Code switch
        {
            BandErrors.Codes.Duplicate => Conflict(result.Error),
            BandErrors.Codes.NotFound or "Member.NotFound" or "Release.NotFound" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }

    private IActionResult DeleteResult(Result<bool> result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return NotFound(result.Error);
    }

    public sealed class ConfirmBandImageInput
    {
        public BandImageType ImageType { get; init; }
        public string TemporaryFileId { get; init; } = string.Empty;
    }

    public sealed class UploadTemporaryBandImageInput
    {
        public IFormFile? File { get; init; }
        public BandImageType ImageType { get; init; }
    }
}
