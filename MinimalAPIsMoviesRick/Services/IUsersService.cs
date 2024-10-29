using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsMoviesRick.Services
{
    public interface IUsersService
    {
        Task<IdentityUser?> GetUser();
    }
}