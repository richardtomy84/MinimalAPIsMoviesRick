using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMoviesRick.Entities;
using System.Data;

namespace MinimalAPIsMoviesRick.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly string? connetionString;

        public CommentsRepository(IConfiguration configuration)
        {
            connetionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Create(Comment comment)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                var id = await connection.QuerySingleAsync<int>("Comments_Create",
                    new { comment.Body, comment.MovieId },
                    commandType: System.Data.CommandType.StoredProcedure
                    );
                comment.Id = id;
                return id;
            }
        }


        public async Task<List<Comment>> GetAll(int movieId)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                var comments = await connection.QueryAsync<Comment>("Comments_GetAllByMovieId",
                    new { movieId }, commandType: CommandType.StoredProcedure);
                return comments.ToList();
            }
        }

        public async Task<Comment?> GetById(int id)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                var comment = await connection.QueryFirstOrDefaultAsync<Comment>
                    ("comments_GetById", new { id }, commandType: CommandType.StoredProcedure);
                return comment;
            }

        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                var exsists = await connection.QuerySingleAsync<bool>("Comments_Exists",
                    new { id }, commandType: CommandType.StoredProcedure);
                return exsists;
            }
        }

        public async Task Update(Comment comment)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                await connection.ExecuteAsync("Comments_Update", new
                {
                    comment.Body,
                    comment.Id,
                    comment.MovieId,
                },

                    commandType: CommandType.StoredProcedure);

            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                await connection.ExecuteAsync("Comments_Delete", new
                {
                    id
                }, commandType: CommandType.StoredProcedure);
            }
        }


    }
}
