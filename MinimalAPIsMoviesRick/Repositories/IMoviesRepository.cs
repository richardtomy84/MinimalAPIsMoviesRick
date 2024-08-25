using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;

namespace MinimalAPIsMoviesRick.Repositories
{
    public interface IMoviesRepository
    {
        Task Assign(int id, List<int> genresIds);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Movie>> GetAll(PaginationDTO paginationDTO);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
    }
}