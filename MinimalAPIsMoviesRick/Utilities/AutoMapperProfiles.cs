using AutoMapper;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
namespace MinimalAPIsMoviesRick.Utilities
{
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles() {

            CreateMap<Genre, GenreDTO>();
            CreateMap<GenreDTO, Genre>();


        
        }
    }
}
