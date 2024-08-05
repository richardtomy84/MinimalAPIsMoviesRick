using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Repositories;

namespace MinimalAPIsMoviesRick.EndPoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("", GetGenre).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

            group.MapGet("/{id:int}", GetById);

            //Data from SQL
            group.MapPost("/", Create);

            group.MapPut("/{id:int}", Update);

            group.MapDelete("/{id:int}", Delete);

            return group;
        }



        //Lamda Expressions to Named Methods
        static async Task<Ok<List<Genre>>> GetGenre(IGenresRepository repository)
        {
            var geners = await repository.GetAll();
            return TypedResults.Ok(geners);
        }

        static async Task<Results<Ok<Genre>, NotFound>> GetById(int id, IGenresRepository genresRepository)
        {

            var genre = await genresRepository.GetById(id);

            if (genre is null)
            {
                return TypedResults.NotFound();

                // return Results.NotFound();
            }
            // return Results.Ok(genre);
            return TypedResults.Ok(genre);

        }

        static async Task<Created<Genre>> Create(Genre genre, IGenresRepository genresRepository, IOutputCacheStore outputCacheStore)
        {
            var id = await genresRepository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            // return TypedResults.Ok(genre);
            return TypedResults.Created($"/genres/{id}", genre);


        }


        static async Task<Results<NotFound, NoContent>> Update(int id, Genre genre, IGenresRepository repository, IOutputCacheStore outputCacheStore)
        {

            var exists = await repository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();

            }

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
