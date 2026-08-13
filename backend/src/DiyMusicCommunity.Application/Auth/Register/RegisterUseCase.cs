using DiyMusicCommunity.Application.Abstractions;
using DiyMusicCommunity.Application.Common;

namespace DiyMusicCommunity.Application.Auth.Register;

public sealed class RegisterUseCase
{
    private readonly IIdentityService _identityService;
    private readonly RegisterRequestValidator _validator;

    public RegisterUseCase(IIdentityService identityService, RegisterRequestValidator validator)
    {
        _identityService = identityService;
        _validator = validator;
    }

    public async Task<Result<bool>> Handle(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<bool>.Failure(Error.Validation(AuthErrors.Codes.RegistrationFailed, message));
        }

        var emailTaken = await _identityService.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailTaken)
        {
            return Result<bool>.Failure(AuthErrors.EmailAlreadyTaken());
        }

        var usernameTaken = await _identityService.ExistsByUsernameAsync(request.Username, cancellationToken);
        if (usernameTaken)
        {
            return Result<bool>.Failure(AuthErrors.UsernameAlreadyTaken());
        }

        var (succeeded, errors) = await _identityService.RegisterAsync(
            request.Username,
            request.Email,
            request.Password,
            cancellationToken);

        if (!succeeded)
        {
            return Result<bool>.Failure(AuthErrors.RegistrationFailed(errors));
        }

        return Result<bool>.Success(true);
    }
}
