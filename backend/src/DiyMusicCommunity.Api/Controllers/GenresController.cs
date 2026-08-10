using DiyMusicCommunity.Application.Genres.GetGenres;
using Microsoft.AspNetCore.Mvc;

namespace DiyMusicCommunity.Api.Controllers;

/// <summary>
/// Public endpoint for retrieving the list of genres (e.g. to populate UI dropdowns).
/// </summary>
[ApiController]
[Route("api/genres")]
[Produces("application/json")]
[Tags("Genres")]
public sealed class GenresController : ControllerBase
{
    private readonly GetGenresUseCase _getGenresUseCase;

    /// <summary>Initialises a new instance of <see cref="GenresController"/>.</summary>
    public GenresController(GetGenresUseCase getGenresUseCase)
    {
        _getGenresUseCase = getGenresUseCase;
    }

    /// <summary>Returns all genres sorted alphabetically, suitable for populating UI dropdowns.</summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Alphabetically sorted list of all genres.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GenreModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGenres(CancellationToken cancellationToken)
    {
        var result = await _getGenresUseCase.Handle(cancellationToken);

        return Ok(result.Value);
    }
}
