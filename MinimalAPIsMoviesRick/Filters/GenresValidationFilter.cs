﻿
using FluentValidation;
using MinimalAPIsMoviesRick.DTOs;

namespace MinimalAPIsMoviesRick.Filters
{
    public class GenresValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<CreateGenreDTO>>();

            if (validator == null)
            {
                return await next(context);

            }

            var obj = context.Arguments.OfType<CreateGenreDTO>().FirstOrDefault();

            if (obj == null)
            {
                return Results.Problem("The object to validate could not found");

            }

            var validationResult = await validator.ValidateAsync(obj);

            if (!validationResult.IsValid) { 


                return Results.ValidationProblem(validationResult.ToDictionary());
            
            }

            return await next(context); 
        }
    }
}
