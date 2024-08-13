using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Repositories;
using MinimalAPIsMoviesRick.Services;

namespace MinimalAPIsMoviesRick.EndPoints
{
    public static  class MoviesEndPoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapPost("/",create).DisableAntiforgery();
            return group;

        }
        
        static async Task<Created<MovieDTO>> create([FromForm] CreateMovieDTO createMovieDTO,
            IFileStorage fileStorage,IOutputCacheStore outputCacheStore,
            IMapper mapper,IMoviesRepository repository)
        {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Store(container, createMovieDTO.Poster);
                movie.Poster = url;
            }
            
            var id=await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"movies/{id}", movieDTO);
        }
    }
}
