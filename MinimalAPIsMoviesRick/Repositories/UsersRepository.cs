using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MinimalAPIsMoviesRick.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string connectionstring;

        public UsersRepository(IConfiguration configuration)
        {

            connectionstring = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IdentityUser?> GetByEmail(string normalizedEmail)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                return await connection.QuerySingleOrDefaultAsync<IdentityUser>("Users_GetByEmail",
                    new { normalizedEmail }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<string> Create(IdentityUser user)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                user.Id = Guid.NewGuid().ToString();
                await connection.ExecuteAsync("Users_Create",
                    new
                    {
                        user.Id,
                        user.Email,
                        user.NormalizedEmail,
                        user.UserName,
                        user.NormalizedUserName,
                        user.PasswordHash

                    }, commandType: CommandType.StoredProcedure);
                return user.Id;
            }
        }
    }
}
