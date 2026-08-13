using FluentValidation;

namespace DiyMusicCommunity.Application.Auth.Register;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private const string EmailRequired = "Email is required.";
    private const string EmailInvalidFormat = "Email must be a valid email address.";
    private const string EmailMaxLength = "Email must not exceed 256 characters.";
    private const string UsernameRequired = "Username is required.";
    private const string UsernameMinLength = "Username must be at least 3 characters.";
    private const string UsernameMaxLength = "Username must not exceed 50 characters.";
    private const string PasswordRequired = "Password is required.";
    private const string PasswordMinLength = "Password must be at least 8 characters.";

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(EmailRequired)
            .EmailAddress().WithMessage(EmailInvalidFormat)
            .MaximumLength(256).WithMessage(EmailMaxLength);

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(UsernameRequired)
            .MinimumLength(3).WithMessage(UsernameMinLength)
            .MaximumLength(50).WithMessage(UsernameMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(PasswordRequired)
            .MinimumLength(8).WithMessage(PasswordMinLength);
    }
}
