using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;

namespace MinimalAPIsMoviesRick.Validaton
{
    public class CreateGenereDTOValidator:AbstractValidator<CreateGenreDTO>
    {
        public CreateGenereDTOValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().WithMessage("The field {PropertyName} is required")
                .MaximumLength(150)
                .WithMessage("The Field {PropertyName} should be less than {MaxLength} characters")
                .Must(FirstLetterIsUppercase).WithMessage("The field {PropertyName} should start with upercase");
        }

        private bool FirstLetterIsUppercase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
