using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
using MinimalAPIsMoviesRick.Filters;
using MinimalAPIsMoviesRick.Repositories;
using MinimalAPIsMoviesRick.Services;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MinimalAPIsMoviesRick.EndPoints
{
    public static class CommentsEndpoints
    {

        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group ) 
        {
            
            group.MapGet("/", GetAll)
               .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));

            group.MapGet("/{id:int}", GetById);
            //group.MapGet("/{id:int}", GetByIdN);

            group.MapPost("/",Create).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>()
                .RequireAuthorization();
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>(); ;
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Results<Ok<List<CommentDTO>>,NotFound>> GetAll(int movieId,ICommentsRepository commentsRepository,
            IMoviesRepository moviesRepository, IMapper mapper)
        {
             if (!await moviesRepository.Exists(movieId))
             {
                return TypedResults.NotFound();
             }

             var comments = await commentsRepository.GetAll(movieId);
            var commentsDTO = mapper.Map<List<CommentDTO>>(comments);
            return TypedResults.Ok(commentsDTO);
            

        }

        static  async Task<Results<Ok<CommentDTO>,NotFound>> GetById(int movieId,int id,
            ICommentsRepository commentsRepository,IMoviesRepository moviesRepository,
            IMapper mapper)
        {
            if(await moviesRepository.Exists(movieId)) {
                return TypedResults.NotFound(); 
            }

            var comment = await commentsRepository.GetById(id);

            if (comment is null) { 
                return TypedResults.NotFound();
            
            }

            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDTO);


        }

        static async Task<Results<Created<CommentDTO>,NotFound, BadRequest<string>>> Create(int movieId,
            CreateCommentDTO createCommentDTO,ICommentsRepository commentsRepository, 
            IMoviesRepository moviesRepository,IMapper mapper,IOutputCacheStore outputCacheStore, IUsersService usersService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();

            }

            var user = await usersService.GetUser();

            if (user is null) {

                return TypedResults.BadRequest("User not found");
            
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieId;
            comment.UserId = user.Id;
            var id= await commentsRepository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comment/{id}", commentDTO);

        }

        static async Task<Results<NoContent,NotFound,ForbidHttpResult>> Update(int movieId, int Id, CreateCommentDTO createCommentDTO, IOutputCacheStore outputCacheStore,
            ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper,IUsersService usersService)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();

            }

            var commentfromDB = await commentsRepository.GetById(Id);

            if(commentfromDB is null)
            {
                return TypedResults.NotFound();

            }

            var user = await usersService.GetUser();

            if (user is null) {
                return TypedResults.NotFound();
            
            }

            if (commentfromDB.UserId !=user.Id)
            {
                return TypedResults.Forbid();
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.Id = Id;
            comment.MovieId = movieId;

            await commentsRepository.Update(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent,NotFound>> Delete(int movieId,int id,ICommentsRepository commentsRepository,
            IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();

            }

            if (!await commentsRepository.Exists(id))
            {
                return TypedResults.NotFound();

            }

            await commentsRepository.Delete(id);

            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();

        }

    }
}
