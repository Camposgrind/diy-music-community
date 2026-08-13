using DiyMusicCommunity.Application.Common;

namespace DiyMusicCommunity.Application.Auth;

/// <summary>
/// Centralises all error codes and factory methods for Auth use cases.
/// No magic strings should appear outside this class.
/// </summary>
public static class AuthErrors
{
    /// <summary>Error code constants — use these wherever a code must be compared (e.g. controller routing).</summary>
    public static class Codes
    {
        public const string EmailAlreadyTaken = "Auth.EmailAlreadyTaken";
        public const string UsernameAlreadyTaken = "Auth.UsernameAlreadyTaken";
        public const string RegistrationFailed = "Auth.RegistrationFailed";
        public const string InvalidCredentials = "Auth.InvalidCredentials";
        public const string EmailOrUsernamRequired = "Auth.EmailOrUsernameRequired";
    }

    public static Error EmailAlreadyTaken()
        => Error.Conflict(Codes.EmailAlreadyTaken, "The email address is already registered.");

    public static Error UsernameAlreadyTaken()
        => Error.Conflict(Codes.UsernameAlreadyTaken, "The username is already taken.");

    public static Error RegistrationFailed(IEnumerable<string> errors)
        => Error.Validation(Codes.RegistrationFailed, string.Join(" ", errors));

    public static Error InvalidCredentials()
        => Error.Validation(Codes.InvalidCredentials, "The email/username or password is incorrect.");

    public static Error EmailOrUsernameRequired()
        => Error.Validation(Codes.EmailOrUsernamRequired, "Either email or username must be provided.");
}
