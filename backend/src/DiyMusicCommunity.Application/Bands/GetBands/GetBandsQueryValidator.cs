using FluentValidation;

namespace DiyMusicCommunity.Application.Bands.GetBands;

public sealed class GetBandsQueryValidator : AbstractValidator<GetBandsQuery>
{
    private const string PageMessage = "Page must be greater than or equal to 1.";
    private const string PageSizeMessage = "PageSize must be between 1 and 50.";
    private const string NameLengthMessage = "Name filter must not exceed 200 characters.";
    private const string CountryLengthMessage = "Country filter must not exceed 100 characters.";

    public GetBandsQueryValidator()
    {
        RuleFor(q => q.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage(PageMessage);

        RuleFor(q => q.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage(PageSizeMessage);

        RuleFor(q => q.Name)
            .MaximumLength(200)
            .WithMessage(NameLengthMessage)
            .When(q => q.Name is not null);

        RuleFor(q => q.Country)
            .MaximumLength(100)
            .WithMessage(CountryLengthMessage)
            .When(q => q.Country is not null);
    }
}
