﻿
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using MinimalAPIsMoviesRick.Repositories;

namespace MinimalAPIsMoviesRick.Filters
{
    public class TestFilters : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            //This is the code that will execute before the endpoint
            /* if we change the order this code not work
            var param1= (int)context.Arguments[0]!;
            var param2=(IGenresRepository)context.Arguments[1]!;
            var param3=(IMapper)context.Arguments[2]!;
            var result = await next(context);

            */
            //here we use data type so no issue ...
            var param1 = context.Arguments.OfType<int>().FirstOrDefault();
            var param2 = context.Arguments.OfType<IGenresRepository>().FirstOrDefault()!;
            var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault()!;
            var result = await next(context);
            //This is the code that will excecute after the endpoint 
            return result;
        }
    }
}
