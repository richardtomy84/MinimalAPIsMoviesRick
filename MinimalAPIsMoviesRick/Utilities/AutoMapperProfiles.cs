using AutoMapper;
using Microsoft.Extensions.Options;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
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
        
        }
    }
}
