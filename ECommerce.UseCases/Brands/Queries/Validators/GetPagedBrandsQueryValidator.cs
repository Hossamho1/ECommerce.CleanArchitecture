using FluentValidation;

namespace ECommerce.Application.Brands.Queries.Validators;

public sealed class GetPagedBrandsQueryValidator : AbstractValidator<GetPagedBrandsQuery>
{
    public GetPagedBrandsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be greater than or equal to 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));
    }
}
