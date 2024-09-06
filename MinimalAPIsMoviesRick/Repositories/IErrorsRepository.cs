using MinimalAPIsMoviesRick.Entities;

namespace MinimalAPIsMoviesRick.Repositories
{
    public interface IErrorsRepository
    {
        Task<Guid> Create(Error error);
    }
}