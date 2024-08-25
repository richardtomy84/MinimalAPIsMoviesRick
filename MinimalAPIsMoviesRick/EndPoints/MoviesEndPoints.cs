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
             group.MapGet("/{id:int}", GetById);

            group.MapPost("/",create).DisableAntiforgery();
           group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}",Delete);
            group.MapPost("/{id:int}/assignGenres", AssignGenres);
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

        static async Task<Results<Ok<MovieDTO>,NotFound>> GetById(int id, IMoviesRepository repository,IMapper mapper
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

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateMovieDTO createMovieDTO,
            IMoviesRepository repository,IFileStorage fileStorage,IOutputCacheStore outputCacheStore,IMapper mapper)
        {
            var movieDb= await repository.GetById(id);
            if (movieDb is null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate=mapper.Map<Movie>(createMovieDTO);
            movieForUpdate.Id=id;
            movieForUpdate.Poster =movieDb.Poster;

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster,container,
                    createMovieDTO.Poster);
                movieForUpdate.Poster = url;
            }

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-tag",default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent,NotFound>> Delete(int id,IMoviesRepository repository,
                                      IOutputCacheStore outputCacheStore, IFileStorage fileStorage)
        {
            var movieDB= await repository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(movieDB.Poster,container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent,NotFound,BadRequest<string>>> AssignGenres
            (int id,List<int> genersIds,IMoviesRepository moviesRepository,IGenresRepository genresRepository)
        {
            if(! await moviesRepository.Exists(id))
            {
                return TypedResults.NotFound();
            }

            var existingGenres = new List<int>();

            if (genersIds.Count != 0)
            {
                existingGenres = await genresRepository.Exists(genersIds);
            }

            if (genersIds.Count != existingGenres.Count)
            {
                var nonExistingGenres = genersIds.Except(existingGenres).ToList();

                var nonExistingGenresCV = string.Join(",", nonExistingGenres);
                return TypedResults.BadRequest($"The genres of id{nonExistingGenresCV} does not exsist.");

            }

            await moviesRepository.Assign(id,genersIds);
            return TypedResults.NoContent();
        }
    }
}
