using DiyMusicCommunity.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiyMusicCommunity.Api.Controllers;

/// <summary>
/// Sample protected endpoints demonstrating JWT authentication and role-based authorization.
/// </summary>
[ApiController]
[Route("api/diagnostics")]
[Produces("application/json")]
[Tags("Diagnostics")]
public sealed class DiagnosticsController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    /// <summary>Initialises a new instance of <see cref="DiagnosticsController"/>.</summary>
    public DiagnosticsController(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    /// <summary>Returns the current user's identity. Requires a valid JWT token.</summary>
    /// <response code="200">Authenticated user info.</response>
    /// <response code="401">Missing or invalid JWT token.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = _currentUser.UserId,
            Email = _currentUser.Email,
            IsAuthenticated = _currentUser.IsAuthenticated
        });
    }

    /// <summary>Admin-only endpoint. Requires the Admin role.</summary>
    /// <response code="200">Confirmation that the caller has the Admin role.</response>
    /// <response code="401">Missing or invalid JWT token.</response>
    /// <response code="403">Authenticated user does not have the Admin role.</response>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult AdminOnly()
    {
        return Ok(new { Message = "You have Admin access." });
    }
}
