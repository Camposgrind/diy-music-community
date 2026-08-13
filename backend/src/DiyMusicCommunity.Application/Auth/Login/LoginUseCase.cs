using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Application.Common;

namespace DiyMusicCommunity.Application.Auth.Login;

public sealed class LoginUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly LoginRequestValidator _validator;

    public LoginUseCase(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        LoginRequestValidator validator)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _validator = validator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<LoginResponse>.Failure(Error.Validation(AuthErrors.Codes.InvalidCredentials, message));
        }

        var (succeeded, userId, email, roles) = await _identityService.LoginAsync(
            request.Email,
            request.Username,
            request.Password,
            cancellationToken);

        if (!succeeded)
        {
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials());
        }

        var (token, expiresAt) = _jwtTokenService.GenerateToken(userId, email, roles);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt
        });
    }
}
