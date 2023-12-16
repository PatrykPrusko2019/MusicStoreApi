using FluentValidation;
using MusicStoreApi.Entities;

namespace MusicStoreApi.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(ArtistDbContext artistDbContext)
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(p => p.Password).MinimumLength(6);

            RuleFor(p => p.ConfirmPassword).Equal(p => p.Password);

            RuleFor(e => e.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = artistDbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });

            RuleFor(f => f.FirstName).NotEmpty();
            RuleFor(f => f.LastName).NotEmpty();


        }
    }
}
