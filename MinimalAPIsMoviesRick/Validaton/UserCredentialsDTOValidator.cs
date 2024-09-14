using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;
namespace MinimalAPIsMoviesRick.Validaton
{
    public class UserCredentialsDTOValidator :AbstractValidator<UserCredentialsDTO>
    {
        public UserCredentialsDTOValidator() 
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(256).WithMessage(ValidationUtilities.MaximunLengthMessage)
                .EmailAddress().WithMessage(ValidationUtilities.EmailAddressMessage);

            RuleFor(x => x.Password).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);
        
        
        }
            

    }
}
