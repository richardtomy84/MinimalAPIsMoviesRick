using MinimalAPIsMoviesRick.Entities;

namespace MinimalAPIsMoviesRick.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        void Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
        Task update(Actor actor);
    }
}