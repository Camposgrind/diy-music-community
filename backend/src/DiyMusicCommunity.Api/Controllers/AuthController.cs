using DiyMusicCommunity.Application.Auth;
using DiyMusicCommunity.Application.Auth.Login;
using DiyMusicCommunity.Application.Auth.Register;
using DiyMusicCommunity.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace DiyMusicCommunity.Api.Controllers;

/// <summary>
/// Handles user registration and login.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Tags("Auth")]
public sealed class AuthController : ControllerBase
{
    private readonly RegisterUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;

    /// <summary>Initialises a new instance of <see cref="AuthController"/>.</summary>
    public AuthController(RegisterUseCase registerUseCase, LoginUseCase loginUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
    }

    /// <summary>Register a new user account.</summary>
    /// <param name="request">Email, username, and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation error.</response>
    /// <response code="409">Email or username already taken.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _registerUseCase.Handle(request, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.Code switch
            {
                AuthErrors.Codes.EmailAlreadyTaken => Conflict(result.Error),
                AuthErrors.Codes.UsernameAlreadyTaken => Conflict(result.Error),
                _ => BadRequest(result.Error)
            };
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>Login with email or username and password.</summary>
    /// <param name="request">Credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">JWT token and expiry date.</response>
    /// <response code="400">Validation error or invalid credentials.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _loginUseCase.Handle(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
