using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MusicStoreApi.Entities;

namespace MusicStoreApi.Models.Validators
{
    public class LoginUserDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginUserDtoValidator(ArtistDbContext artistDbContext, IPasswordHasher<User> passwordHasher)
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(p => p.Password).MinimumLength(6);

            RuleFor(e => e.Email)
                .Custom((searchEmail, context) =>
                 {
                     var isValidEmail = artistDbContext.Users.Any(u => u.Email == searchEmail);
                     if (!isValidEmail) context.AddFailure("Invalid username or password"); // There is no precise information that this is an incorrect email address, for the security of data of customers using a given application.
                 });

                
        }
    }
}
