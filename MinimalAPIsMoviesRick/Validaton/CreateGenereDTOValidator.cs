using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;

namespace MinimalAPIsMoviesRick.Validaton
{
    public class CreateGenereDTOValidator:AbstractValidator<CreateGenreDTO>
    {
        public CreateGenereDTOValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().WithMessage("The field {PropertyName} is required");
        }
    }
}
