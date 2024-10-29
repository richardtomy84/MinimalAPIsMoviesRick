using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMoviesRick.Entities;
using System.Data;

namespace MinimalAPIsMoviesRick.Repositories
{
    public class ErrorsRepository : IErrorsRepository
    {
        private readonly string? connetionString;

        public ErrorsRepository(IConfiguration configuration)
        {
            connetionString = configuration.GetConnectionString("DefaultConnection")!;

        }
        //Errors_Create
        public async Task<Guid> Create(Error error)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                error.Id = Guid.NewGuid();
                await connection.ExecuteAsync("Errors_Create", new
                {
                    error.Id,
                    error.ErrorMessage,
                    error.StackTrace,
                    error.Date
                }, commandType: CommandType.StoredProcedure);
                return error.Id;
            }
        }
    }
}
