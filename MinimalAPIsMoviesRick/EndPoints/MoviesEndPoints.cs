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
             group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("movies-get"));
            // group.MapGet("/{id:int}", GetById);

            group.MapPost("/",create).DisableAntiforgery();
           
            return group;

        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository repository,IMapper mapper,int page=1,
            int recordsPerPage=10)
        {
            var pagination = new PaginationDTO { Page = page , RecordsPerPage = recordsPerPage};
            var movies = await repository.GetAll(pagination);
            var moviesDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(moviesDTO);

        }

        static async Task<Results<Ok<MovieDTO>,NotFound>> GetById(int id, IMoviesRepository repository,Mapper mapper
            )
        {
            var movie = await repository.GetById(id);
            if (movie == null) {
                return TypedResults.NotFound();
            }

            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
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
