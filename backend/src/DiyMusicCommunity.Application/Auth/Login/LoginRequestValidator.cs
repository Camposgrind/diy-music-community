using FluentValidation;

namespace DiyMusicCommunity.Application.Auth.Login;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    private const string EmailOrUsernameRequired = "Either email or username must be provided.";
    private const string PasswordRequired = "Password is required.";

    public LoginRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Username))
            .WithMessage(EmailOrUsernameRequired)
            .OverridePropertyName("EmailOrUsername");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(PasswordRequired);
    }
}
