using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;

namespace MinimalAPIsMoviesRick.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<List<Actor>> GetByName(string name);
        Task update(Actor actor);
    }
}