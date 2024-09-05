using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Filters;
using MinimalAPIsMoviesRick.Repositories;

namespace MinimalAPIsMoviesRick.EndPoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("", GetGenre).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

            group.MapGet("/{id:int}", GetById).AddEndpointFilter<TestFilters>();

            //Data from SQL
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>();

            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>();

            group.MapDelete("/{id:int}", Delete);

            return group;
        }



        //Lamda Expressions to Named Methods
        static async Task<Ok<List<GenreDTO>>> GetGenre(IGenresRepository repository,IMapper mapper)
        {
            var geners = await repository.GetAll();
            //var genersDTOs = geners.Select(x=> new GenreDTO { id=x.Id,Name =x.Name}).ToList(); //Linq Method
            var genersDTOs = mapper.Map<List<GenreDTO>>(geners);

            return TypedResults.Ok(genersDTOs);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById( IGenresRepository genresRepository,
            IMapper mapper, int id)
        {

            var genre = await genresRepository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();

                // return Results.NotFound();
            }

            var genreDTO = mapper.Map<GenreDTO>(genre);
            /*    new GenreDTO()
            {
                id = genre.Id,
                Name = genre.Name,
            };
            */
            // return Results.Ok(genre);
            return TypedResults.Ok(genreDTO);
              
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenreDTO, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            /* not use because we use validater in AddEndpointFilter<GenresValidationFilter>(); 
            var validationResult= await validator.ValidateAsync(createGenreDTO);
            if (!validationResult.IsValid) {

                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            
            }

            * /
             
              /*  new Genre
            {
                Name = createGenreDTO.Name,
            };
            */

                        var genre = mapper.Map<Genre>(createGenreDTO);

            var id = await genresRepository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            // return TypedResults.Ok(genre);

            var genreDTO = mapper.Map<GenreDTO>(genre);
                /*new GenreDTO()
            {
                id = genre.Id,
                Name = genre.Name,
            };
                */
            return TypedResults.Created($"/genres/{id}", genreDTO);


        }


        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, 
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            /* not use because we use validater in group.MapPut("/{id:int}", Update).AddEndpointFilter<GenresValidationFilter>();

            var validationResult = await validator.ValidateAsync(createGenreDTO);

            if (!validationResult.IsValid)
            {

                return TypedResults.ValidationProblem(validationResult.ToDictionary());

            }
            */

            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();

            } 
            var genre = mapper.Map<Genre>(createGenreDTO);
              /*  new Genre
            {
                Id = id,
                Name = createGenreDTO.Name,
            };
           */
              genre.Id = id;

            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {
            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();

            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        //Lamda Expressions to Named Methods - End

    }
}
