using FluentValidation;
using MusicStoreApi.Entities;

namespace MusicStoreApi.Models.Validators
{
    public class SongQueryValidator : AbstractValidator<SongQuery>
    {
        private string allowedSortByColumnName = nameof(Song.Name);

        public SongQueryValidator()
        {
            RuleFor(a => a.SortBy)
                .Must(sortByValue => string.IsNullOrEmpty(sortByValue) || allowedSortByColumnName.Contains(sortByValue))
                .WithMessage($"Sort by is optional, or must be in [{allowedSortByColumnName}]");
        }
    }

}
