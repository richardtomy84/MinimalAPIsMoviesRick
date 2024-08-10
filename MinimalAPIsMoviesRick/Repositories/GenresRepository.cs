using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMoviesRick.Entities;
using System.Data;

namespace MinimalAPIsMoviesRick.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly string connectionString;

        public GenresRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        
        }

        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                /*
                //  var query = connection.Query("select 1").FirstOrDefault(); 
                var query = @"insert into Geners (Name) Values (@Name);
                            select SCOPE_IDENTITY();";

                var id = await connection.QuerySingleAsync<int>(query,genre); */
                var id = await connection.QuerySingleAsync<int>("Genres_create",
                   new { genre.Name  });

                genre.Id = id;
                return id;
            }
           // return Task.FromResult(0);
        }

        public async Task Delete(int id)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                /*
                var query = @"delete from Geners where Id=@id ";
                     await connection.ExecuteAsync(query,new { id }); */
                //Genres_Delete
                await connection.ExecuteAsync("Genres_Delete", new { id }, commandType: CommandType.StoredProcedure); 

            }
        }

        public async Task<bool> Exists(int id)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>(@"Genres_Exist", new {id}, commandType: CommandType.StoredProcedure);
                return exists;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryAsync<Genre>(@"Genres_GetAll",commandType:CommandType.StoredProcedure);
                return genres.ToList();
            }

        }
         
        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                /*
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id,Name  
                                             from Geners where 
                                             id = @Id",  new {id});*/
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>(@"Genres_GetById", new { id }, commandType: CommandType.StoredProcedure);


                return genre;
            }

        }

        public async Task Update(Genre genre)
        {
            using( var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"Genres_Update", new {genre.Id,genre.Name},commandType:CommandType.StoredProcedure);
            }

        }
    }
}
