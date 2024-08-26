using AutoMapper;
using Microsoft.Extensions.Options;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
using System.IO.Hashing;
namespace MinimalAPIsMoviesRick.Utilities
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles() {

            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();

            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
                .ForMember(p => p.Picture, Options => Options.Ignore());

            CreateMap<Movie, MovieDTO>() 
                .ForMember(x=> x.Genres, entity =>
                entity.MapFrom(p=>p.GenresMovies.Select(
                    gm =>new GenreDTO 
                    { 
                        id = gm.GenreId,
                        Name= gm.Genre.Name
                    }
                    )))
                .ForMember(x=>x.Actors, entity => 
                              entity.MapFrom(p=>p.ActorMoviess.Select (
                                  am=> new ActorMovieDTO
                                  {
                                      Id=am.ActorId,
                                      Name = am.Actor.Name,
                                      Character =am.Character
                                  }
                                  )));
            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(p => p.Poster, Options => Options.Ignore());

            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<AssignActorMovieDTO, ActorMovie>();

        }
    }
}
