using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMoviesRick.DTOs;
using MinimalAPIsMoviesRick.Entities;
using System.Data;

namespace MinimalAPIsMoviesRick.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly string connectionString;
        private readonly HttpContext HttpContext;

        public MoviesRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuration.GetConnectionString("Defaultconnection")!;
            HttpContext = httpContextAccessor.HttpContext!;

        }

        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("Movies_create", new
                {
                    movie.Titile,
                    movie.Poster,
                    movie.ReleaseDate,
                    movie.InTheater
                }, commandType: CommandType.StoredProcedure);

                movie.Id = id;
                return id;

            }
        }

        public async Task<List<Movie>> GetAll(PaginationDTO paginationDTO)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movies = await connection.QueryAsync<Movie>("Movies_GetAll",
                    new { paginationDTO.Page, paginationDTO.RecordsPerPage },
                    commandType: CommandType.StoredProcedure);

                var moviesCount = await connection.QuerySingleAsync<int>("Movies_Count",
                    commandType: CommandType.StoredProcedure);

                HttpContext.Response.Headers.Append("totalAmountOfRecords", moviesCount.ToString());

                return movies.ToList();
            }
        }


        public async Task<Movie?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                //now use multiple query result in Movies_GetById  ( Query related data )
                using (var multi = await
                    connection.QueryMultipleAsync("Movies_GetById", new {id}))
                {
                    var movie =  await multi.ReadFirstAsync<Movie>();
                    var comments = await multi.ReadAsync<Comment>();

                    movie.Comments = comments.ToList();

                    return movie;

                }
                /*
                var movie = await connection.QueryFirstOrDefaultAsync<Movie>("Movies_GetById",
                    new { id }, commandType: CommandType.StoredProcedure); */

            }
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>("Movies_Exists", new { id },
                    commandType: CommandType.StoredProcedure);
                return exists;
            }
        }

        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_Update", new
                {
                    movie.Id,
                    movie.Titile,
                    movie.InTheater,
                    movie.Poster,
                    movie.ReleaseDate

                }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_Delete", new
                {
                    id
                });
            }
        }


    }
}
