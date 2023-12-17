using FluentValidation;
using MusicStoreApi.Entities;

namespace MusicStoreApi.Models.Validators
{
    public class AlbumQueryValidator : AbstractValidator<AlbumQuery>
    {
        private string allowedSortByColumnName = nameof(Album.Title);

        public AlbumQueryValidator()
        {
            RuleFor(a => a.SortBy)
                .Must(sortByValue => string.IsNullOrEmpty(sortByValue) || allowedSortByColumnName.Contains(sortByValue))
                .WithMessage($"Sort by is optional, or must be in [{allowedSortByColumnName}]");
        }
    }
}
