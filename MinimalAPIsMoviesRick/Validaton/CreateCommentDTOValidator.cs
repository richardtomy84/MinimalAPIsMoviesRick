using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;

namespace MinimalAPIsMoviesRick.Validaton
{
    public class CreateCommentDTOValidator : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidator()
        {
            RuleFor(x => x.Body).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);

        
        }
    }
}
