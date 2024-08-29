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



            RuleFor(p=>p.Name).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150)
                .WithMessage(ValidationUtilities.MaximunLengthMessage)
                .Must(ValidationUtilities.FirstLetterIsUppercase).WithMessage(ValidationUtilities.FirstLetterIsUpperCaseMessage)
                .MustAsync(async(name, _) =>{
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
            
                 }).WithMessage(g=>$"A genre with the name{g.Name} already exists ");
        }

   
    }
}
