using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;

namespace MinimalAPIsMoviesRick.Validaton
{
    public class CreateMovieDTOValidator : AbstractValidator<CreateMovieDTO>
    {
        public CreateMovieDTOValidator()
        {
            RuleFor(x => x.Titile).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(250).WithMessage(ValidationUtilities.MaximunLengthMessage);
        }
    }
}
