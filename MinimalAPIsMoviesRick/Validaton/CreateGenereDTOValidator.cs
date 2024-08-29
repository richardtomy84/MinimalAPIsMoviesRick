using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Repositories;

namespace MinimalAPIsMoviesRick.Validaton
{
    public class CreateGenereDTOValidator:AbstractValidator<CreateGenreDTO>
    {
        public CreateGenereDTOValidator(IGenresRepository genresRepository,IHttpContextAccessor httpContextAccessor)
        {
             var  routeValueId= httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;

            if(routeValueId is string routeVlueIdString)
            {
                int.TryParse(routeVlueIdString, out id);
            }

            RuleFor(p=>p.Name).NotEmpty().WithMessage("The field {PropertyName} is required")
                .MaximumLength(150)
                .WithMessage("The Field {PropertyName} should be less than {MaxLength} characters")
                .Must(FirstLetterIsUppercase).WithMessage("The field {PropertyName} should start with upercase")
                .MustAsync(async(name, _) =>{
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
            
                 }).WithMessage(g=>$"A genre with the name{g.Name} already exists ");
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
