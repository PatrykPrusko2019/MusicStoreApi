using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using MusicStoreApi.Entities;

namespace MusicStoreApi.Models.Validators
{
    public class ArtistQueryValidator : AbstractValidator<ArtistQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };
        private string[] allowedSortByColumnNames = { nameof(Artist.Name), nameof(Artist.Description), nameof(Artist.KindOfMusic) };
        public ArtistQueryValidator()
        {
            RuleFor(a => a.PageNumber).GreaterThanOrEqualTo(1);

            RuleFor(a => a.PageSize).Custom((searchPageSize, context) =>
            { 
                if (!allowedPageSizes.Contains(searchPageSize))
                {
                    context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
                }

            });

            RuleFor(a => a.SortBy)
                .Must(sortByValue => string.IsNullOrEmpty(sortByValue) || allowedSortByColumnNames.Contains(sortByValue))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");


        }
    }
}
